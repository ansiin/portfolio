using App.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Areas.Admin.ViewModels;
using WebApp.Helpers;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class UsersController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderBy(user => user.Email)
            .ToListAsync();

        var items = new List<AdminUserOverviewItemViewModel>(users.Count);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            items.Add(new AdminUserOverviewItemViewModel
            {
                Email = user.Email ?? "-",
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.OrderBy(role => role).ToList()
            });
        }

        var vm = new AdminUserOverviewViewModel
        {
            PageTitle = UiText.T("Users"),
            Heading = UiText.T("UsersAndRoles"),
            Description = "Review accounts and the roles currently assigned in the system.",
            UserCount = items.Count,
            RoleCount = await _roleManager.Roles.CountAsync(),
            Users = items
        };

        return View(vm);
    }
}
