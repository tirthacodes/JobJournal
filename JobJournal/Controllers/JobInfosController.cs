using JobJournal.Data;
using JobJournal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobJournal.Controllers
{
    public class JobInfosController : Controller
    {
        private readonly AppDbContext _context;

        public JobInfosController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var jobs = await _context.JobInfos.Include(j => j.user).ToListAsync();
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
                // Show all validation errors in console
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    var errors = entry.Value.Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                    }
                }

                return View(jobInfo);
            }

            jobInfo.userId = 1;
            jobInfo.appliedTime = DateTime.Now;

            _context.JobInfos.Add(jobInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }





    }
}
