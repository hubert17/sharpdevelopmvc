using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet10MvcApi.Data;
using Dotnet10MvcApi.Helpers;
using Dotnet10MvcApi.Models;
using Dotnet10MvcApi.Models.Entities;
using Dotnet10MvcApi.Services;

namespace Dotnet10MvcApi.Controllers.Api
{
    [ApiController]
    [Route("")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly TokenManager _tokenManager;

        public AccountController(ApplicationDbContext db, TokenManager tokenManager)
        {
            _db = db;
            _tokenManager = tokenManager;
        }

        // Helper: Create admin account if it does not exist
        private async Task EnsureAdminCreatedAsync()
        {
            var hasAdmin = await _db.Users.AnyAsync(x => x.Roles == UserAccount.DEFAULT_ADMIN_ROLENAME);
            if (!hasAdmin)
            {
                UserAccount.CreatePasswordHash(UserAccount.DEFAULT_ADMIN_LOGIN, out byte[] passwordHash, out byte[] passwordSalt);
                var admin = new UserAccount
                {
                    Id = Guid.NewGuid(),
                    UserName = UserAccount.DEFAULT_ADMIN_LOGIN,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    Roles = UserAccount.DEFAULT_ADMIN_ROLENAME
                };
                _db.Users.Add(admin);
                await _db.SaveChangesAsync();
            }
        }

        // POST /TOKEN
        [HttpPost]
        [Route("TOKEN")]
        public async Task<IActionResult> GetToken(
            [FromQuery] string? username, 
            [FromQuery] string? password,
            [FromForm] string? formUsername,
            [FromForm] string? formPassword)
        {
            var inputUsername = username ?? formUsername;
            var inputPassword = password ?? formPassword;

            if (string.IsNullOrWhiteSpace(inputUsername) || string.IsNullOrWhiteSpace(inputPassword))
            {
                return BadRequest("Username and password are required.");
            }

            await EnsureAdminCreatedAsync();

            // Match original special admin username override logic
            if (inputUsername.Contains("@"))
            {
                var prefix = inputUsername.Split('@')[0];
                if (prefix.Equals(UserAccount.DEFAULT_ADMIN_LOGIN, StringComparison.OrdinalIgnoreCase))
                {
                    inputUsername = UserAccount.DEFAULT_ADMIN_LOGIN;
                }
            }

            var cleanUsername = inputUsername.Trim().ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == cleanUsername);

            if (user == null || !user.IsActive)
            {
                return BadRequest("Login failed");
            }

            bool valid = UserAccount.VerifyPasswordHash(inputPassword, user.PasswordSalt, user.PasswordHash);
            if (!valid)
            {
                return BadRequest("Login failed");
            }

            // If it is admin and they are logging in with a username that has an email format, check original block
            if (inputUsername.Equals(UserAccount.DEFAULT_ADMIN_LOGIN, StringComparison.OrdinalIgnoreCase) && 
                (username?.Contains("@") == true || formUsername?.Contains("@") == true))
            {
                return BadRequest("Please change your password");
            }

            // Update last login
            user.LastLogin = DateTime.Now;
            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            var userRoles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var token = _tokenManager.CreateToken(user.UserName, userRoles);
            var refreshToken = GenerateRefreshTokenString();

            // Save Refresh Token
            var existingToken = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.UserName == user.UserName);
            if (existingToken != null)
            {
                existingToken.Token = refreshToken;
                existingToken.Created = DateTime.UtcNow;
                _db.Entry(existingToken).State = EntityState.Modified;
            }
            else
            {
                _db.RefreshTokens.Add(new RefreshToken
                {
                    UserName = user.UserName,
                    Token = refreshToken,
                    Created = DateTime.UtcNow
                });
            }
            await _db.SaveChangesAsync();

            return Ok(new
            {
                userId = user.UserName,
                userName = user.UserName,
                userRoles = userRoles,
                token = token,
                refreshToken = refreshToken
            });
        }

        // POST /TOKENREFRESH
        [HttpPost]
        [Route("TOKENREFRESH")]
        public async Task<IActionResult> GetRefreshToken(
            [FromQuery] string? token, 
            [FromQuery] string? refreshToken,
            [FromForm] string? formToken,
            [FromForm] string? formRefreshToken)
        {
            var inputToken = token ?? formToken;
            var inputRefreshToken = refreshToken ?? formRefreshToken;

            if (string.IsNullOrWhiteSpace(inputToken) || string.IsNullOrWhiteSpace(inputRefreshToken))
            {
                return BadRequest("Token and refresh token are required.");
            }

            try
            {
                var principal = _tokenManager.GetPrincipalFromExpiredToken(inputToken);
                if (principal?.Identity?.Name == null)
                {
                    return BadRequest("Invalid Token");
                }

                var username = principal.Identity.Name;
                var userRoles = principal.Claims
                    .Where(x => x.Type == ClaimTypes.Role)
                    .Select(s => s.Value)
                    .ToArray();

                // Validate refresh token from database
                var savedToken = await _db.RefreshTokens.FirstOrDefaultAsync(r => 
                    r.UserName.ToLower() == username.ToLower() && r.Token == inputRefreshToken);

                if (savedToken != null)
                {
                    var newAccessToken = _tokenManager.CreateToken(username, userRoles);
                    var newRefreshToken = GenerateRefreshTokenString();

                    savedToken.Token = newRefreshToken;
                    savedToken.Created = DateTime.UtcNow;
                    _db.Entry(savedToken).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    return Ok(new
                    {
                        userId = username,
                        userName = username,
                        userRoles = userRoles,
                        token = newAccessToken,
                        refreshToken = newRefreshToken
                    });
                }
            }
            catch { }

            return BadRequest("Invalid Token or Refresh Token");
        }

        // POST /TOKENLOGOUT
        [HttpPost]
        [Route("TOKENLOGOUT")]
        public async Task<IActionResult> SignOutToken(
            [FromQuery] string? token,
            [FromForm] string? formToken)
        {
            var inputToken = token ?? formToken;
            if (string.IsNullOrWhiteSpace(inputToken))
            {
                return BadRequest("Token is required.");
            }

            try
            {
                var principal = _tokenManager.GetPrincipalFromExpiredToken(inputToken);
                if (principal?.Identity?.Name != null)
                {
                    var username = principal.Identity.Name;
                    var tokenToRemove = await _db.RefreshTokens.FirstOrDefaultAsync(x => 
                        x.UserName.ToLower() == username.ToLower());

                    if (tokenToRemove != null)
                    {
                        _db.RefreshTokens.Remove(tokenToRemove);
                        await _db.SaveChangesAsync();
                        return Ok("Refresh Token successfully removed. Account has been signed out.");
                    }
                }
            }
            catch { }

            return BadRequest("Invalid Token");
        }

        // POST /api/account/register
        [HttpPost]
        [Route("api/account/register")]
        public async Task<IActionResult> RegisterUser(
            [FromQuery] string? username, 
            [FromQuery] string? password, 
            [FromQuery] string? role,
            [FromForm] string? formUsername,
            [FromForm] string? formPassword,
            [FromForm] string? formRole)
        {
            var inputUsername = username ?? formUsername;
            var inputPassword = password ?? formPassword;
            var inputRole = string.IsNullOrWhiteSpace(role) ? (formRole ?? "") : role;

            if (string.IsNullOrWhiteSpace(inputUsername) || string.IsNullOrWhiteSpace(inputPassword))
            {
                return BadRequest("Username and password are required.");
            }

            foreach (var r in inputRole.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (r.Trim().Equals(UserAccount.DEFAULT_ADMIN_ROLENAME, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Creating an admin account is forbidden");
                }
            }

            var cleanUsername = inputUsername.Trim().ToLower();
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.UserName == cleanUsername);
            if (existingUser != null)
            {
                return BadRequest("Account already exists");
            }

            UserAccount.CreatePasswordHash(inputPassword, out byte[] passwordHash, out byte[] passwordSalt);
            var newUser = new UserAccount
            {
                Id = Guid.NewGuid(),
                UserName = cleanUsername,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedOn = DateTime.Now,
                IsActive = true,
                Roles = System.Text.RegularExpressions.Regex.Replace(inputRole, @"\s+", "")
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok(new 
            { 
                userId = newUser.UserName,
                message = "Account successfully created" 
            });
        }

        // POST /api/account/registerbyadmin
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserAccount.DEFAULT_ADMIN_ROLENAME)]
        [HttpPost]
        [Route("api/account/registerbyadmin")]
        public async Task<IActionResult> RegisterWithRole(
            [FromQuery] string? username, 
            [FromQuery] string? password, 
            [FromQuery] string? comma_separated_roles,
            [FromForm] string? formUsername,
            [FromForm] string? formPassword,
            [FromForm] string? formRoles)
        {
            var inputUsername = username ?? formUsername;
            var inputPassword = password ?? formPassword;
            var inputRoles = string.IsNullOrWhiteSpace(comma_separated_roles) ? (formRoles ?? "") : comma_separated_roles;

            if (string.IsNullOrWhiteSpace(inputUsername) || string.IsNullOrWhiteSpace(inputPassword))
            {
                return BadRequest("Username and password are required.");
            }

            var cleanUsername = inputUsername.Trim().ToLower();
            var existingUser = await _db.Users.FirstOrDefaultAsync(x => x.UserName == cleanUsername);
            if (existingUser != null)
            {
                return BadRequest("Account already registered");
            }

            UserAccount.CreatePasswordHash(inputPassword, out byte[] passwordHash, out byte[] passwordSalt);
            var newUser = new UserAccount
            {
                Id = Guid.NewGuid(),
                UserName = cleanUsername,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedOn = DateTime.Now,
                IsActive = true,
                Roles = System.Text.RegularExpressions.Regex.Replace(inputRoles, @"\s+", "")
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok(new 
            { 
                userId = newUser.UserName,
                message = "Account successfully created" 
            });
        }

        // POST /api/account/changepassword
        [HttpPost]
        [Route("api/account/changepassword")]
        public async Task<IActionResult> ChangePassword(
            [FromQuery] string? username, 
            [FromQuery] string? newPassword, 
            [FromQuery] string? currentPassword,
            [FromForm] string? formUsername,
            [FromForm] string? formNewPassword,
            [FromForm] string? formCurrentPassword)
        {
            var inputUsername = username ?? formUsername;
            var inputNewPassword = newPassword ?? formNewPassword;
            var inputCurrentPassword = string.IsNullOrWhiteSpace(currentPassword) ? (formCurrentPassword ?? "") : currentPassword;

            if (string.IsNullOrWhiteSpace(inputUsername) || string.IsNullOrWhiteSpace(inputNewPassword))
            {
                return BadRequest("Username and new password are required.");
            }

            var cleanUsername = inputUsername.Trim().ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == cleanUsername);
            if (user == null)
            {
                return BadRequest("Password change failed");
            }

            var forceChangeIfAdmin = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var identityRoles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);
                forceChangeIfAdmin = identityRoles.Contains(UserAccount.DEFAULT_ADMIN_ROLENAME);
            }

            var validPassword = forceChangeIfAdmin || UserAccount.VerifyPasswordHash(inputCurrentPassword, user.PasswordSalt, user.PasswordHash);
            if (validPassword)
            {
                UserAccount.CreatePasswordHash(inputNewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _db.Entry(user).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return Ok("Password successfully changed");
            }

            return BadRequest("Password change failed");
        }

        // GET /api/account/me
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("api/account/me")]
        public async Task<IActionResult> GetUserProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }

            // Return user info, clearing sensitive fields
            return Ok(new
            {
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.CreatedOn,
                    user.LastLogin,
                    user.IsActive,
                    user.Roles
                }
            });
        }

        private string GenerateRefreshTokenString()
        {
            var randomBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "");
        }
    }
}
