using JWTAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace ASPNETWebApp48.Controllers.Api
{
    public class AccountController : ApiController
	{    	
        [HttpPost]
        [Route("TOKEN")]
        public IHttpActionResult GetToken(string username, string password)
        {
            var valid = UserAccount.Authenticate(username, password);
            if (valid)
            {
                var user = UserAccount.GetCurrentUser();
                if (username.Contains("@"))
                    if (username.Split('@')[0].ToLower().Equals(UserAccount.DEFAULT_ADMIN_LOGIN.ToLower()))
                         return BadRequest("Please change your password");

                var userRoles = user.Roles.Split(',');
                var data = new 
                { 
                	userId = user.UserName,
                	userName = user.UserName,
                	userRoles = userRoles,
                	token = JWTAuth.TokenManager.CreateToken(username, userRoles),
                    refreshToken = JWTAuth.RefreshTokenManager.GenerateRefreshToken(username)
                };
                return Ok(data);
            }

            return BadRequest("Login failed");
        }

        [HttpPost]
        [Route("TOKENREFRESH")]
        public IHttpActionResult GetRefreshToken(string token, string refreshToken)
        {
            try
            {
                var principal = JWTAuth.TokenManager.GetPrincipalFromExpiredToken(token);
                var username = principal.Identity.Name;
                var userRoles = principal.FindAll(System.Security.Claims.ClaimTypes.Role).Select(s => s.Value).ToArray();

                if (JWTAuth.RefreshTokenManager.IsValid(username, refreshToken))
                {
                    var data = new
                    {
                        userId = username,
                        userName = username,
                        userRoles = userRoles,
                        token = JWTAuth.TokenManager.CreateToken(username, userRoles),
                        refreshToken = JWTAuth.RefreshTokenManager.GenerateRefreshToken(username)
                    };
                    return Ok(data);
                }
            }
            catch { }

            return BadRequest("Invalid Token or Refresh Token");
        }


        [HttpPost]
        [Route("api/account/register")]
        public IHttpActionResult RegisterUser(string username, string password, string role = "") // You can add more parameter here ex LastName, FirstName etc
        {
            foreach (var r in role.Split(',').Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                if (r.Trim().ToLower() == UserAccount.DEFAULT_ADMIN_ROLENAME)
                    return BadRequest("Creating an admin account is forbidden");
            }

            var user = UserAccount.GetUserByUserName(username);
            if (user != null)
                return BadRequest("Account already exists");

            var newUser = UserAccount.Create(username, password, role);
            if (newUser == null)
                return BadRequest("Account registration failed");

            return Ok(new { User = newUser, Message = "Account successfully created" });
        }

        [ApiAuthorize(Roles = UserAccount.DEFAULT_ADMIN_ROLENAME)]
        [HttpPost]
        [Route("api/account/registerbyadmin")]
        public IHttpActionResult RegisterWithRole(string username, string password, string comma_separated_roles = "")
        {
            var userRoles = UserAccount.GetUserRoles(User.Identity.Name);
            var isAdmin = userRoles != null && userRoles.Contains(UserAccount.DEFAULT_ADMIN_ROLENAME);
            if (!isAdmin)
                return BadRequest("Access forbidden. For administrator only");

            var user = UserAccount.GetUserByUserName(username);
            if (user != null)
                return BadRequest("Account already registered");

            var newUser = UserAccount.Create(username, password, comma_separated_roles);
            if (newUser != null)
                return Ok(new { User = newUser, Message = "Account successfully created" });
            else
                return BadRequest("Account registration failed");
        }

        [HttpPost]
        [Route("api/account/changepassword")]
        public IHttpActionResult ChangePassword(string username, string newPassword, string currentPassword = "")
        {
            var forceChangeIfAdmin = false;

            try
            {
                forceChangeIfAdmin = Array.IndexOf(UserAccount.GetUserRoles(User.Identity.Name), UserAccount.DEFAULT_ADMIN_ROLENAME) > -1;
            }
            catch { }

            var success = UserAccount.ChangePassword(username, currentPassword, newPassword, forceChangeIfAdmin);
            if (success)
                return Ok("Password successfully changed");
            else
                return BadRequest("Password change failed");
        }

        [Authorize]
        [HttpGet]
        [Route("api/account/me")]
        public IHttpActionResult GetUserProfile()
        {
            return Ok(new { user = UserAccount.GetUserByUserName(User.Identity.Name) });
        }


    }



}