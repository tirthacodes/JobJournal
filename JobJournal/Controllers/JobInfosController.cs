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

        // POST
        [HttpPost]
        public async Task<IActionResult> Create(JobInfo jobInfo)
        {
            if (ModelState.IsValid)
            {
                // hardcoding atm, later will be from logged in user
                jobInfo.userId = 1;
                jobInfo.appliedTime = DateTime.Now;

                _context.JobInfos.Add(jobInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(jobInfo);
        }




    }
}
