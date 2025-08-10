using System.Configuration;
using CsvHelper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuth
{
	public static class TokenManager
	{		
		public static string secret = ConfigurationManager.AppSettings["APIsecret"];
		
		public static string CreateToken(string username, string[] roles = null, int expireMinutes = 20)
		{		
			//Set issued at date
			DateTime issuedAt = DateTime.UtcNow;
			//set the time when it expires
			DateTime expires = DateTime.UtcNow.AddMinutes(expireMinutes);
		
			var tokenHandler = new JwtSecurityTokenHandler();
			
			// Make jwt payload compatible with newer .NET Web API
			tokenHandler.OutboundClaimTypeMap[ClaimTypes.NameIdentifier] = "sub";			
			tokenHandler.OutboundClaimTypeMap[ClaimTypes.Name] = "name";
		  
			//create an identity and add claims to the user which we want to log in  
			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
			claims.Add(new Claim(ClaimTypes.Name, username));
			if (roles != null) 
			{
				foreach (var role in roles) 
				{
					claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
				}        	
			}     	    
	        
			var claimsIdentity = new ClaimsIdentity(claims);        
			var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secret));
			var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);		
		
			//create the jwt
			var token = tokenHandler.CreateJwtSecurityToken(
				               subject: claimsIdentity, 
				               notBefore: issuedAt, 
				               expires: expires, 
				               signingCredentials: signingCredentials
			               );
		    
			var tokenString = tokenHandler.WriteToken(token);
		
			return tokenString;
		}

		public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			try 
			{
				var tokenValidationParameters = new TokenValidationParameters 
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = false,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secret))
				};

				SecurityToken securityToken;
				var tokenHandler = new JwtSecurityTokenHandler();
				// Make jwt payload compatible with newer .NET Web API
				tokenHandler.InboundClaimTypeMap = new Dictionary<string, string>
					{
					    { "name", System.Security.Claims.ClaimTypes.Name },
					    { "sub", System.Security.Claims.ClaimTypes.NameIdentifier },
					    { "role", System.Security.Claims.ClaimTypes.Role },
					};
			
				var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

				return principal;
			} 
			catch
			{
				return null;
			}

		}
	}
}