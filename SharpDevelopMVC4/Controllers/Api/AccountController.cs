﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using JWTAuth;

namespace ASPNETWebApp45.Controllers.Api
{
    public class AccountController : ApiController
	{    	
        [HttpPost]
        [Route("TOKEN")]
        public IHttpActionResult GetToken(string username, string password)
        {        			
            var user = UserAccountCSV.Authenticate(username, password);
            if (user != null)
            {     
            	if (username.ToLower() == UserAccountCSV.DEFAULT_ADMIN_LOGIN.ToLower() && password == UserAccountCSV.DEFAULT_ADMIN_LOGIN.ToLower())
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
                var userRoles = principal.Claims.Where(x => x.Type == System.Security.Claims.ClaimTypes.Role).Select(s => s.Value).ToArray();

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
        [Route("TOKENLOGOUT")]
        public IHttpActionResult SignOutToken(string token = "")
        {
            try
            {
                var principal = JWTAuth.TokenManager.GetPrincipalFromExpiredToken(token);
            	var username = principal.Identity.Name;

                if (JWTAuth.RefreshTokenManager.Remove(username))
					return Ok("Refresh Token successfully removed. Account has been signed out.");
            }
            catch { }

            return BadRequest("Invalid Token");
        }  
        
		[HttpPost]
		[Route("api/account/register")]
		public IHttpActionResult RegisterUser(string username, string password, string role = "") // You can add more parameter here ex LastName, FirstName etc
		{
			foreach (var r in role.Split(',').Where(r => !string.IsNullOrWhiteSpace(r))) 
			{
				if (r.Trim().ToLower() == "admin")
					return BadRequest("Creating an admin account is forbidden.");
			}
        	
			var user = UserAccountCSV.GetUserByUserName(username);
			if (user != null)
				return BadRequest("Account already exists");

			var newUser = UserAccountCSV.Create(username, password, role);
			if (newUser == null) 
				return BadRequest("Account registration failed");

			return Ok(new { User = newUser, Message = "Account successfully created" });				
		}

		[ApiAuthorize]
        [HttpPost]
        [Route("api/account/registerbyadmin")]
        public IHttpActionResult RegisterWithRole(string username, string password, string comma_separated_roles = "")
        {
        	 var userRoles = UserAccountCSV.GetUserRoles(User.Identity.Name);
        	 var isAdmin = userRoles != null && userRoles.Contains("admin");
        	 if(!isAdmin)
        		 return BadRequest("Access forbidden. For administrator only.");
        	
            var user = UserAccountCSV.GetUserByUserName(username);
            if(user != null)
            	return BadRequest("Account already registered!");

            var newUser = UserAccountCSV.Create(username, password, comma_separated_roles);
            if (newUser != null)
                return Ok(new { User = newUser, Message = "Account successfully created" });
            else
                return BadRequest("Account registration failed");
        }   

        [AllowAnonymous]
        [HttpPost]
        [Route("api/account/changepassword")]
        public IHttpActionResult ChangePassword(string username, string newPassword, string currentPassword = "")
        {
            var forceChangeIfAdmin = false;

            try 
            {
             	forceChangeIfAdmin = Array.IndexOf(UserAccountCSV.GetUserRoles(User.Identity.Name), UserAccountCSV.DEFAULT_ADMIN_ROLENAME) > -1;           	
            } 
            catch {}

            var success = UserAccountCSV.ChangePassword(username, currentPassword, newPassword, forceChangeIfAdmin);
            if (success)
				return Ok("Password successfully changed");
            else
                return BadRequest("Password change failed");
        }        
        
		[ApiAuthorize]
		[HttpGet]
		[Route("api/account/me")]
		public IHttpActionResult GetUserProfile()
		{
			return Ok(new { user = UserAccountCSV.GetUserByUserName(User.Identity.Name)});
		}

    }



}