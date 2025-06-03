using JobJournal.Data;
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


        
    }
}
