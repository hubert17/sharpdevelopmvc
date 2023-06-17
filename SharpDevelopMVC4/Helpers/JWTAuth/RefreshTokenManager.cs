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
	public static class RefreshTokenManager
    {
		// All about Refresh Token
		public static string GenerateRefreshToken(string username)
		{
			const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

			using (var rng = RandomNumberGenerator.Create())
			{
				var randomBytes = new byte[32]; // tokenLength
				rng.GetBytes(randomBytes);

				var rToken = new string(randomBytes.Select(b => allowedChars[b % allowedChars.Length]).ToArray());

				Save(username, rToken);

				return rToken;
			}
		}

		public static bool IsValid(string username, string refreshToken)
        {
			return Read().Any(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase) && x.Token == refreshToken);
		}

		private static void Save(string username, string refreshToken)
        {
			try
			{
				var requestTokens = Read();
				if (requestTokens.Any(a => a.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)))
					requestTokens.Single(x => x.UserName == username).Token = refreshToken;
				else
					requestTokens.Add(new RefreshTokenModel(username, refreshToken));

				//requestTokens.Where(x => x.UserName == username).ToList().ForEach(x =>
				//{
				//	x.Token = rToken;
				//});
				Write(requestTokens);
			}
			catch { }
		}

		private static List<RefreshTokenModel> Read()
		{
			var csvFile = GetRefreshTokenCsvFile();
			if (File.Exists(csvFile))
			{
				using (var reader = new StreamReader(csvFile))
				using (var csv = new CsvReader(reader))
				{
					return csv.GetRecords<RefreshTokenModel>().ToList();
				}
			}

			return new List<RefreshTokenModel>();
		}

		private static void Write(List<RefreshTokenModel> records)
		{
			var csvFile = GetRefreshTokenCsvFile();
			using (var writer = new StreamWriter(csvFile))
			using (var csv = new CsvWriter(writer))
			{
				csv.WriteRecords(records);
			}
		}

		private static string GetRefreshTokenCsvFile()
		{
			const string REFRESHTOKEN_CSV_FILE = @"refresh-tokens.csv";
			return Path.IsPathRooted(REFRESHTOKEN_CSV_FILE) ? REFRESHTOKEN_CSV_FILE : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", REFRESHTOKEN_CSV_FILE);
		}


	}

	public class RefreshTokenModel
	{
		public RefreshTokenModel() { }
		public RefreshTokenModel(string userName, string refreshToken)
        {
			UserName = userName;
			Token = refreshToken;
        }

		public string UserName { get; set; }
		public string Token { get; set; }
	}
}