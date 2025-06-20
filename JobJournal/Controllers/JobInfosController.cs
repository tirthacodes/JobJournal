using JobJournal.Data;
using JobJournal.Data.Enums;
using JobJournal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Security.Claims;

namespace JobJournal.Controllers
{
    [Authorize]
    public class JobInfosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public JobInfosController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(string searchTerm, ApplicationStatus? statusFilter)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobInfos = _context.JobInfos.Where(j => j.userId == currentUserId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                jobInfos = jobInfos.Where(j => j.companyName.ToLower().Contains(searchTerm.ToLower())
                                            || j.role.ToLower().Contains(searchTerm.ToLower())
                                            || (j.jobSummary != null && j.jobSummary.ToLower().Contains(searchTerm.ToLower()))
                                            || (j.notes != null && j.notes.ToLower().Contains(searchTerm.ToLower())));

                ViewData["CurrentFilter"] = searchTerm;
            }

            if (statusFilter.HasValue)
            {
                jobInfos = jobInfos.Where(j => j.applicationStatus == statusFilter.Value);
                ViewData["CurrentStatusFilter"] = statusFilter.Value.ToString(); 
            }

            var interviewJobs = jobInfos
                .Where(j => j.applicationStatus == ApplicationStatus.InterviewScheduled)
                .OrderBy(j => j.appliedTime) 
                .AsEnumerable(); 

            var otherJobs = jobInfos
                .Where(j => j.applicationStatus != ApplicationStatus.InterviewScheduled)
                .OrderByDescending(j => j.appliedTime) 
                .AsEnumerable(); 

            var sortedJobInfos = interviewJobs.Concat(otherJobs);

            return View(await Task.FromResult(sortedJobInfos));
        }




        //For creating JobInfo
        // GET
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(JobInfo jobInfo)
        {

            if (!ModelState.IsValid)
            {
                TempData["JobCreationFailedMessage"] = "Job application creation failed!";
                return View(jobInfo);
            }

            jobInfo.userId = _userManager.GetUserId(User);

            _context.JobInfos.Add(jobInfo);
            await _context.SaveChangesAsync();
            TempData["JobCreatedMessage"] = "Job application saved successfully!";
            return RedirectToAction(nameof(Index));
        }




        //Get Edit request
        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var jobInfo = await _context.JobInfos.FindAsync(id);
            if(jobInfo == null)
            {
                return NotFound();
            }

            return View(jobInfo);
        }


        // POST: JobInfos/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobInfo jobInfo)
        {
            if (id != jobInfo.id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["JobEditFailedMessage"] = "Job application Edit Failed!";
                return View(jobInfo);
            }

            try
            {
                _context.Update(jobInfo);
                await _context.SaveChangesAsync();
                TempData["JobEditedMessage"] = "Job application edited successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.JobInfos.Any(e => e.id == id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }



        //Get Delete request
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobInfo = await _context.JobInfos.FindAsync(id);
            if (jobInfo == null)
            {
                return NotFound();
            }

            return View(jobInfo);
        }

        //Post Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobInfo = await _context.JobInfos.FindAsync(id);
            if (jobInfo != null)
            {
                _context.JobInfos.Remove(jobInfo);
                await _context.SaveChangesAsync();
                TempData["JobDeletedMessage"] = "Job application deleted successfully!";
            }
            else
            {
                TempData["JobDeleteFailedMessage"] = "Job application failed to delete!";
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: JobInfos/ExportExcel
        public async Task<IActionResult> ExportExcel(string searchTerm, ApplicationStatus? statusFilter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobInfosQuery = from j in _context.JobInfos
                                where j.userId == userId
                                select j;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                jobInfosQuery = jobInfosQuery.Where(j => j.companyName.ToLower().Contains(searchTerm.ToLower())
                                                    || j.role.ToLower().Contains(searchTerm.ToLower())
                                                    || (j.jobSummary != null && j.jobSummary.ToLower().Contains(searchTerm.ToLower()))
                                                    || (j.notes != null && j.notes.ToLower().Contains(searchTerm.ToLower())));
            }

            if (statusFilter.HasValue)
            {
                jobInfosQuery = jobInfosQuery.Where(j => j.applicationStatus == statusFilter.Value);
            }

            var jobApplications = await jobInfosQuery.OrderByDescending(j => j.appliedTime).ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Job Applications");

                worksheet.Cells[1, 1].Value = "Company Name";
                worksheet.Cells[1, 2].Value = "Role";
                worksheet.Cells[1, 3].Value = "Job Summary";
                worksheet.Cells[1, 4].Value = "Application Status";
                worksheet.Cells[1, 5].Value = "Applied Via";
                worksheet.Cells[1, 6].Value = "Applied Time";
                worksheet.Cells[1, 7].Value = "Notes";

                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                }

                for (int i = 0; i < jobApplications.Count; i++)
                {
                    var job = jobApplications[i];
                    int row = i + 2;

                    worksheet.Cells[row, 1].Value = job.companyName;
                    worksheet.Cells[row, 2].Value = job.role;
                    worksheet.Cells[row, 3].Value = job.jobSummary;
                    worksheet.Cells[row, 4].Value = job.applicationStatus.ToString();
                    worksheet.Cells[row, 5].Value = job.appliedVia;
                    worksheet.Cells[row, 6].Value = job.appliedTime.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 7].Value = job.notes;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                var fileName = $"JobJournal_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileName);
            }
        }
    }
}
