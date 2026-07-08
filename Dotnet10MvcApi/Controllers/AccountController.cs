using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet10MvcApi.Data;
using Dotnet10MvcApi.Models;

namespace Dotnet10MvcApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool rememberme = false, string returnUrl = "/")
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["alert"] = "Username and password are required.";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            var cleanUsername = username.Trim().ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == cleanUsername);

            if (user != null && user.IsActive && UserAccount.VerifyPasswordHash(password, user.PasswordSalt, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Roles)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberme,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Update last login
                user.LastLogin = DateTime.Now;
                _db.Entry(user).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                if (cleanUsername == UserAccount.DEFAULT_ADMIN_LOGIN && password == UserAccount.DEFAULT_ADMIN_LOGIN)
                {
                    return RedirectToAction("ChangePassword");
                }

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            TempData["alert"] = "Invalid username or password";
            return RedirectToAction("Login", new { ReturnUrl = returnUrl });
        }

        public async Task<IActionResult> Logoff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["alertbox"] = "New password cannot be empty.";
                return RedirectToAction("ChangePassword");
            }

            var cleanUsername = User.Identity?.Name?.Trim().ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == cleanUsername);

            if (user != null && UserAccount.VerifyPasswordHash(currentPassword, user.PasswordSalt, user.PasswordHash))
            {
                UserAccount.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _db.Entry(user).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                TempData["alertbox"] = "Password changed successfully.";
                return RedirectToAction("Logoff");
            }

            TempData["alertbox"] = "Failed to change password. Verification failed.";
            return RedirectToAction("ChangePassword");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string role = "")
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["alertbox"] = "Username and password are required.";
                return RedirectToAction("Register");
            }

            var cleanUsername = username.Trim().ToLower();
            var cleanRole = string.IsNullOrWhiteSpace(role) ? "user" : role.Trim().ToLower();

            if (cleanRole == UserAccount.DEFAULT_ADMIN_ROLENAME)
            {
                cleanRole = "user";
            }

            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.UserName == cleanUsername);
            if (existingUser != null)
            {
                TempData["alertbox"] = "Username already exists.";
                return RedirectToAction("Register");
            }

            UserAccount.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var newUser = new UserAccount
            {
                Id = Guid.NewGuid(),
                UserName = cleanUsername,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedOn = DateTime.Now,
                IsActive = true,
                Roles = cleanRole
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            TempData["alert"] = $"Account successfully created. Welcome {username}!";
            return RedirectToAction("Login");
        }

        [Authorize(Roles = UserAccount.DEFAULT_ADMIN_ROLENAME)]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = UserAccount.DEFAULT_ADMIN_ROLENAME)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string username)
        {
            if (username == UserAccount.DEFAULT_ADMIN_LOGIN)
            {
                TempData["alert"] = "Admin account cannot be deactivated.";
            }
            else
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username.Trim().ToLower());
                if (user != null)
                {
                    user.IsActive = false;
                    _db.Entry(user).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    TempData["alertbox"] = $"{username} is now deactivated.";
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = UserAccount.DEFAULT_ADMIN_ROLENAME)]
        [Route("account.csv")]
        public async Task<IActionResult> GetUsersCSV()
        {
            var users = await _db.Users.ToListAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Id,UserName,CreatedOn,IsActive,Roles");
            foreach (var user in users)
            {
                csv.AppendLine($"{user.Id},{user.UserName},{user.CreatedOn:yyyy-MM-dd HH:mm:ss},{user.IsActive},{user.Roles}");
            }
            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "account.csv");
        }
    }
}
