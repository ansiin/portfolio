using System.Net;
using DentalSaaS.Domain.Entities;
using DentalSaaS.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DentalSaaS.Web.Controllers;

[Route("account")]
public sealed class AccountController : Controller
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;

    public AccountController(UserManager<AppUser> users, SignInManager<AppUser> signIn)
    {
        _users = users;
        _signIn = signIn;
    }

    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginViewModel { ReturnUrl = returnUrl });

    [AllowAnonymous]
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _users.FindByEmailAsync(model.Email.Trim().ToLowerInvariant());
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        var result = await _signIn.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet("register")]
    public IActionResult Register()
        => View(new RegisterViewModel());

    [AllowAnonymous]
    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new AppUser
        {
            UserName = model.Email.Trim().ToLowerInvariant(),
            Email = model.Email.Trim().ToLowerInvariant(),
            DisplayName = model.DisplayName.Trim()
        };

        var create = await _users.CreateAsync(user, model.Password);
        if (!create.Succeeded)
        {
            foreach (var error in create.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await _signIn.SignInAsync(user, isPersistent: true);
        TempData["Success"] = "Account created successfully.";
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword()
        => View(new ForgotPasswordViewModel());

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _users.FindByEmailAsync(model.Email.Trim().ToLowerInvariant());
        if (user is null)
        {
            TempData["Success"] = "If the account exists, a reset link is now available.";
            return RedirectToAction(nameof(Login));
        }

        var token = await _users.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);
        var resetUrl = Url.Action(nameof(ResetPassword), "Account", new { email = user.Email, token = encodedToken }, Request.Scheme);

        TempData["Success"] = "Reset link generated for local environment.";
        TempData["GeneratedResetUrl"] = resetUrl;
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet("reset-password")]
    public IActionResult ResetPassword(string email, string token)
    {
        return View(new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _users.FindByEmailAsync(model.Email.Trim().ToLowerInvariant());
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid reset request.");
            return View(model);
        }

        var decoded = WebUtility.UrlDecode(model.Token);
        var reset = await _users.ResetPasswordAsync(user, decoded, model.Password);
        if (!reset.Succeeded)
        {
            foreach (var error in reset.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        TempData["Success"] = "Password reset successful. Please sign in.";
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet("access-denied")]
    public IActionResult AccessDenied()
        => View();
}
