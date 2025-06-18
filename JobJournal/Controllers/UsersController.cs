using JobJournal.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JobJournal.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context; 
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View(currentUser);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}