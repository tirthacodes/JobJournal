using JobJournal.Data;
using JobJournal.Data.Enums;
using JobJournal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
