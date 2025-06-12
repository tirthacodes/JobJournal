using JobJournal.Data.Enums;
using JobJournal.Data;
using JobJournal.Models;
using JobJournal.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace JobJournal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new DashboardViewModel();

            if (userId != null)
            {
                var userJobInfos = _context.JobInfos.Where(j => j.userId == userId);

                // Calculating date ranges
                var today = DateTime.Today;
                // here the startOfWeek considering Sunday of the current week
                var diff = (7 + (today.DayOfWeek - DayOfWeek.Sunday)) % 7;
                var startOfWeek = today.AddDays(-1 * diff);

                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                model.ApplicationsThisWeek = await userJobInfos
                    .Where(j => j.appliedTime >= startOfWeek && j.appliedTime <= today.AddDays(1).AddTicks(-1))
                    .CountAsync();

                model.ApplicationsThisMonth = await userJobInfos
                    .Where(j => j.appliedTime >= startOfMonth && j.appliedTime <= today.AddDays(1).AddTicks(-1))
                    .CountAsync();

                model.InterviewsScheduled = await userJobInfos
                    .Where(j => j.applicationStatus == ApplicationStatus.InterviewScheduled)
                    .CountAsync();

                model.OffersReceived = await userJobInfos
                    .Where(j => j.applicationStatus == ApplicationStatus.Accepted)
                    .CountAsync();

                model.FollowUpsDue = await userJobInfos
                    .Where(j => j.applicationStatus == ApplicationStatus.FollowUp)
                    .CountAsync();

                model.TotalApplications = await userJobInfos.CountAsync();
            }

            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
