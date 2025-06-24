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



        private bool JobInfoExists(int id)
        {
            return _context.JobInfos.Any(e => e.id == id);
        }


        public JobInfosController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(string searchTerm, ApplicationStatus? statusFilter)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobInfos = _context.JobInfos
                                   .Include(j => j.Images)
                                   .Where(j => j.userId == currentUserId);

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

            var interviewJobs = await jobInfos
                .Where(j => j.applicationStatus == ApplicationStatus.InterviewScheduled)
                .OrderBy(j => j.appliedTime)
                .ToListAsync();

            var otherJobs = await jobInfos
                .Where(j => j.applicationStatus != ApplicationStatus.InterviewScheduled)
                .OrderByDescending(j => j.appliedTime)
                .ToListAsync();

            var sortedJobInfos = interviewJobs.Concat(otherJobs);

            return View(sortedJobInfos);
        }




        // GET: JobInfos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobInfos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("companyName,role,jobSummary,applicationStatus,appliedVia,appliedTime,notes")] JobInfo jobInfo,
            string? imageData,      
            string? imageFileName,  
            string? imageContentType) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            jobInfo.userId = userId;

            if (ModelState.IsValid)
            {
                if (jobInfo.Images == null)
                {
                    jobInfo.Images = new List<JobImage>();
                }

                if (!string.IsNullOrEmpty(imageData))
                {
                    var jobImage = new JobImage
                    {
                        ImageData = imageData,       
                        FileName = imageFileName,    
                        ContentType = imageContentType, 
                        Order = 0                    
                    };

                    jobInfo.Images.Add(jobImage);
                }

                _context.Add(jobInfo); 
                await _context.SaveChangesAsync(); 
                TempData["JobCreatedMessage"] = "Job application created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["JobCreationFailedMessage"] = "Job application creation failed!";
            return View(jobInfo);
        }




        // GET: JobInfos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobInfo = await _context.JobInfos
                                        .Include(j => j.Images)
                                        .FirstOrDefaultAsync(m => m.id == id);

            if (jobInfo == null)
            {
                return NotFound();
            }
            return View(jobInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobInfo jobInfo)
        {
            if (id != jobInfo.id)
            {
                return NotFound();
            }

            var existingJobInfo = await _context.JobInfos
                                                .Include(j => j.Images)
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(j => j.id == id);

            if (existingJobInfo == null)
            {
                return NotFound();
            }

            existingJobInfo.companyName = jobInfo.companyName;
            existingJobInfo.role = jobInfo.role;
            existingJobInfo.jobSummary = jobInfo.jobSummary;
            existingJobInfo.applicationStatus = jobInfo.applicationStatus;
            existingJobInfo.appliedVia = jobInfo.appliedVia;
            existingJobInfo.appliedTime = jobInfo.appliedTime;
            existingJobInfo.notes = jobInfo.notes;

            _context.Entry(existingJobInfo).State = EntityState.Modified;

            var submittedExistingImageIds = jobInfo.Images?
                                                .Where(si => si.Id != 0 && !string.IsNullOrEmpty(si.ImageData))
                                                .Select(si => si.Id)
                                                .ToHashSet() ?? new HashSet<int>();

            var currentDbImages = await _context.JobImages
                                                .Where(img => img.JobInfoId == existingJobInfo.id)
                                                .ToListAsync();

            foreach (var dbImage in currentDbImages)
            {
                if (!submittedExistingImageIds.Contains(dbImage.Id))
                {
                    _context.JobImages.Remove(dbImage);
                }
            }

            foreach (var submittedImage in jobInfo.Images ?? new List<JobImage>())
            {
                if (submittedImage.Id == 0)
                {
                    if (!string.IsNullOrEmpty(submittedImage.ImageData))
                    {
                        string fullImageData = submittedImage.ImageData;
                        if (!fullImageData.StartsWith("data:") && !string.IsNullOrEmpty(submittedImage.ContentType))
                        {
                            fullImageData = $"data:{submittedImage.ContentType};base64," + fullImageData;
                        }
                        submittedImage.ImageData = fullImageData;
                        submittedImage.JobInfoId = existingJobInfo.id;
                        _context.JobImages.Add(submittedImage);
                    }
                }
                else
                {
                    var imageInDb = currentDbImages.FirstOrDefault(i => i.Id == submittedImage.Id);
                    if (imageInDb != null)
                    {
                        string fullImageData = submittedImage.ImageData;
                        // Ensuring existing images being updated also get the data URI prefix
                        if (!fullImageData.StartsWith("data:") && !string.IsNullOrEmpty(submittedImage.ContentType))
                        {
                            fullImageData = $"data:{submittedImage.ContentType};base64," + fullImageData;
                        }

                        imageInDb.ImageData = fullImageData; 
                        imageInDb.FileName = submittedImage.FileName;
                        imageInDb.ContentType = submittedImage.ContentType;
                        imageInDb.Order = submittedImage.Order;

                        _context.Entry(imageInDb).State = EntityState.Modified;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["JobEditFailedMessage"] = "Job application Edit Failed! Please check your inputs.";
                return View(existingJobInfo);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["JobEditedMessage"] = "Job application updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobInfoExists(jobInfo.id))
                {
                    return NotFound();
                }
                else
                {
                    TempData["JobEditFailedMessage"] = "Job application Edit Failed due to concurrency conflict!";
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["JobEditFailedMessage"] = $"Job application Edit Failed! Error: {ex.Message}";
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

            var jobInfo = await _context.JobInfos
                                        .Include(j => j.Images) 
                                        .FirstOrDefaultAsync(m => m.id == id); 

            if (jobInfo == null)
            {
                return NotFound();
            }

            return View(jobInfo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Also fetching the JobInfo including its related Images
            var jobInfo = await _context.JobInfos
                                        .Include(j => j.Images) 
                                        .FirstOrDefaultAsync(m => m.id == id); 

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
