using JobJournal.Data;
using JobJournal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobJournal.Controllers
{
    [Authorize]
    public class CvController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CvController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var viewModel = new CvBuilderViewModel
            {
                CvProfile = await _context.CvProfiles.FirstOrDefaultAsync(p => p.UserId == userId),
                Educations = await _context.Educations.Where(e => e.UserId == userId).ToListAsync(),
                Experiences = await _context.Experiences.Where(e => e.UserId == userId).ToListAsync(),
                Projects = await _context.Projects.Where(p => p.UserId == userId).ToListAsync(),
                Skills = await _context.Skills.Where(s => s.UserId == userId).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCvProfile(CvProfile cvProfile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                cvProfile.UserId = userId;

                var existingProfile = await _context.CvProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

                if (existingProfile == null)
                {
                    _context.CvProfiles.Add(cvProfile);
                }
                else
                {
                    existingProfile.FirstName = cvProfile.FirstName;
                    existingProfile.LastName = cvProfile.LastName;
                    existingProfile.Designation = cvProfile.Designation;
                    existingProfile.Address = cvProfile.Address;
                    existingProfile.City = cvProfile.City;
                    existingProfile.Phone = cvProfile.Phone;
                    existingProfile.Summary = cvProfile.Summary;
                    existingProfile.PhotoUrl = cvProfile.PhotoUrl;
                    _context.CvProfiles.Update(existingProfile);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddEducation(Education newEducation)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                newEducation.UserId = userId;
                _context.Educations.Add(newEducation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var educationToDelete = await _context.Educations.FindAsync(id);
            if (educationToDelete != null)
            {
                _context.Educations.Remove(educationToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddExperience(Experience newExperience)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                newExperience.UserId = userId;
                _context.Experiences.Add(newExperience);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var experienceToDelete = await _context.Experiences.FindAsync(id);
            if (experienceToDelete != null)
            {
                _context.Experiences.Remove(experienceToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(Project newProject)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                newProject.UserId = userId;
                _context.Projects.Add(newProject);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var projectToDelete = await _context.Projects.FindAsync(id);
            if (projectToDelete != null)
            {
                _context.Projects.Remove(projectToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Action method to handle adding a new skill entry.
        [HttpPost]
        public async Task<IActionResult> AddSkill(Skill newSkill)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                newSkill.UserId = userId;
                _context.Skills.Add(newSkill);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skillToDelete = await _context.Skills.FindAsync(id);
            if (skillToDelete != null)
            {
                _context.Skills.Remove(skillToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
