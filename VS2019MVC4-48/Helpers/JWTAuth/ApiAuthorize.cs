using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace JWTAuth
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            string token;
            var errMsg = "Invalid token | ";

            if (TryRetrieveToken(actionContext.Request, out token))
            {
                try
                {
                    var now = DateTime.UtcNow;
                    const string secret = JWTAuth.TokenManager.secret;
                    var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secret));

                    SecurityToken securityToken;
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        LifetimeValidator = this.LifetimeValidator,
                        IssuerSigningKey = securityKey
                    };

                    //extract and assign the user of the jwt
                    Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                    HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                    return true;
                }
                catch (SecurityTokenValidationException e)
                {
                    errMsg += e.Message;
                }
                catch (Exception ex)
                {
                    errMsg += ex.Message;
                }

            }

            // Token is invalid, return Unauthorized response
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, errMsg);
            return false;
        }

        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }
    }
}