using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Admin
{
    [Authorize]
    public class ManageUserRolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUserRolesModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ApplicationUser User { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public List<string> UserRoles { get; set; }

        [BindProperty]
        public List<string> SelectedRoles { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            User = await _userManager.FindByIdAsync(userId);
            if (User == null) return NotFound();

            AllRoles = _roleManager.Roles.ToList();
            UserRoles = new List<string>(await _userManager.GetRolesAsync(User));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId)
        {
            User = await _userManager.FindByIdAsync(userId);
            if (User == null) return NotFound();

            // Get current roles and remove all
            var currentRoles = await _userManager.GetRolesAsync(User);
            await _userManager.RemoveFromRolesAsync(User, currentRoles);

            // Add selected roles
            if (SelectedRoles != null && SelectedRoles.Count > 0)
            {
                await _userManager.AddToRolesAsync(User, SelectedRoles);
            }

            return RedirectToPage("Users");
        }
    }
}
