using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

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


    }



}