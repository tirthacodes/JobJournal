using JobJournal.Data;
using JobJournal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var jobs = await _context.JobInfos
                .Where(j => j.userId == userId)
                .Include(j => j.user)
                .ToListAsync();
            return View(jobs);
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
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Any())
                    {
                        Console.WriteLine($"Error in {state.Key}:");
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"- {error.ErrorMessage}");
                        }
                    }
                }
                return View(jobInfo);
            }

            jobInfo.userId = _userManager.GetUserId(User);

            _context.JobInfos.Add(jobInfo);
            await _context.SaveChangesAsync();
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
                return View(jobInfo);

            try
            {
                _context.Update(jobInfo);
                await _context.SaveChangesAsync();
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
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
