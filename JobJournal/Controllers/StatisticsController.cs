using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobJournal.Data;
using JobJournal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobJournal.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StatisticsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string range = "LastYear")
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            DateTime startDate;
            DateTime endDate = DateTime.Today;

            switch (range.ToLower())
            {
                case "lastweek":
                    startDate = endDate.AddDays(-7);
                    break;
                case "lastmonth":
                    startDate = endDate.AddMonths(-1);
                    break;
                case "lastyear":
                    startDate = endDate.AddYears(-1);
                    break;
                case "alltime":
                    startDate = DateTime.MinValue;
                    break;
                default:
                    startDate = endDate.AddYears(-1);
                    range = "LastYear";
                    break;
            }

            var applications = await _context.JobInfos
                                            .Where(j => j.userId == userId && j.appliedTime >= startDate && j.appliedTime <= endDate)
                                            .ToListAsync();

            var statusCounts = applications
                                .GroupBy(j => j.applicationStatus != null ? j.applicationStatus.ToString() : "Unknown Status")
                                .Select(g => new { Status = g.Key, Count = g.Count() })
                                .OrderBy(s => s.Status)
                                .ToList();

            ViewBag.StatusLabels = statusCounts.Select(s => s.Status).ToList();
            ViewBag.StatusData = statusCounts.Select(s => s.Count).ToList();

            var applicationsOverTime = applications
                                        .Where(j => j.appliedTime != DateTime.MinValue)
                                        .GroupBy(j => new { j.appliedTime.Year, j.appliedTime.Month })
                                        .Select(g =>
                                        {
                                            var monthDate = new DateTime(g.Key.Year, g.Key.Month, 1);
                                            return new
                                            {
                                                Month = monthDate,
                                                MonthLabel = monthDate.ToString("MMM 🪚yyyy"),
                                                Count = g.Count()
                                            };
                                        })
                                        .OrderBy(x => x.Month)
                                        .ToList();

            ViewBag.TimeLabels = applicationsOverTime.Select(x => x.MonthLabel).ToList();
            ViewBag.TimeData = applicationsOverTime.Select(x => x.Count).ToList();

            ViewBag.SelectedRange = range;

            return View();
        }
    }
}
