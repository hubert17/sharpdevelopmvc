using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class UserAccountCSV
{
    // Change this to your desired default admin login and password
    public const string DEFAULT_ADMIN_LOGIN = "admin";
    // File path of your Account CSV file
    public const string ACCOUNT_CSV_FILE = @"account.csv";

    #region UserAccountRepository

    public string UserName { get; set; }
    [JsonIgnore]
    public byte[] PasswordHash { get; set; }
    [JsonIgnore]
    public byte[] PasswordSalt { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    public string Roles { get; set; } // comma-separated 

    public const string DEFAULT_ADMIN_ROLENAME = "admin";

    public static UserAccountCSV Authenticate(string userName, string userPassword)
    {
        var accounts = CreateAdmin();

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userPassword))
            return null;

        if (userName.Contains("@") && userName.StartsWith(DEFAULT_ADMIN_LOGIN, StringComparison.OrdinalIgnoreCase))
        {
        	var u = userName.Split('@')[0];
        	userName = u == DEFAULT_ADMIN_LOGIN ? u : userName;
        }			           

        userName = userName.Trim().ToLower();
        var user = GetAccount(userName);
        if (user == null || !user.IsActive)
            return null;

        bool valid = VerifyPasswordHash(userPassword, user.PasswordSalt, user.PasswordHash);
        if (valid)
        {
            accounts.Where(x => x.UserName == userName).ToList().ForEach(x =>
            {
                x.LastLogin = DateTime.Now;
            });
            WriteAccountCSV(accounts);
            CurrentUser = user; // Set current user
            return user;
        }

        return null;
    }

    public static UserAccountCSV Create(string userName, string userPassword, string userRoles = "", bool requiresActivation = false)
    {
        if (string.IsNullOrWhiteSpace(userPassword))
            return null;
        if (string.IsNullOrWhiteSpace(userName) || userName.Any(Char.IsWhiteSpace))
            return null;

        var user = new UserAccountCSV();
        user.UserName = userName.Trim().ToLower();

        var accounts = ReadAccountCSV();
        var userExists = accounts.FirstOrDefault(x => x.UserName == user.UserName) != null;
        if (userExists)
            return null;

        // Create PasswordHash
        using (var hmac = new System.Security.Cryptography.HMACSHA1()) //HMACSHA512
        {
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userPassword));
        }

        user.Roles = System.Text.RegularExpressions.Regex.Replace(userRoles, @"\s+", "");
        user.CreatedOn = DateTime.Now;
        user.LastLogin = default(DateTime);
        user.IsActive = !requiresActivation;

        accounts.Add(user);
        WriteAccountCSV(accounts);

        user.PasswordSalt = null;
        user.PasswordHash = null;
        return user;
    }

    public static bool ChangePassword(string userName, string userPassword = "", string newPassword = "", bool forceChange = false)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            return false;

        if (forceChange == false && string.IsNullOrWhiteSpace(userPassword))
            return false;

        var accounts = ReadAccountCSV();
        var user = accounts.FirstOrDefault(x => x.UserName == userName.Trim());
        if (user == null)
            return false;

        var validPassword = !forceChange ? VerifyPasswordHash(userPassword, user.PasswordSalt, user.PasswordHash) : true;
        if (validPassword)
        {
            // Overwrite with new PasswordHash
            using (var hmac = new System.Security.Cryptography.HMACSHA1()) // HMACSHA512
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPassword));
            }

            accounts.Where(x => x.UserName == userName).ToList().ForEach(x =>
            {
                x.PasswordSalt = user.PasswordSalt;
                x.PasswordHash = user.PasswordHash;
            });
            WriteAccountCSV(accounts);

            return true;
        }
        else
            return false;

    }

    public static List<UserAccountCSV> GetAll()
    {
        var accounts = ReadAccountCSV();
        accounts.ForEach(x =>
        {
            x.PasswordHash = null;
            x.PasswordSalt = null;
        });
        return accounts;
    }

    public static List<UserAccountCSV> GetAllUsersInRole(string role)
    {
        var users = ReadAccountCSV().Where(x => x.Roles.Split(',').Contains(role)).ToList();
        return users;
    }

    public static UserAccountCSV GetUserByUserName(string userName)
    {
        if (userName.Contains("@") && userName.Split('@')[0] == DEFAULT_ADMIN_LOGIN)
            userName = DEFAULT_ADMIN_LOGIN;

        var user = GetAccount(userName);
        if(user != null) 
        {
         	user.PasswordHash = null;
        	user.PasswordSalt = null;   	
        }
        return user;
    }

    public static UserAccountCSV GetCurrentUser()
    {
        return CurrentUser;
    }

    public static string[] GetUserRoles(string userName)
    {
        var user = GetAccount(userName);
        if (user != null)
            return user.Roles.Split(',');
        else
            return new string[] { string.Empty };
    }

    public static string SetUserActivation(string userName, bool isActive)
    {
        var accounts = ReadAccountCSV();
        var user = accounts.SingleOrDefault(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (user != null)
        {
            accounts.Where(x => x.UserName == userName).ToList().ForEach(x =>
            {
                x.IsActive = isActive;
            });
            WriteAccountCSV(accounts);
            return "success";
        }
        else
            return "failed";
    }

    public static string GetCsvFile()
    {
    	return Path.IsPathRooted(ACCOUNT_CSV_FILE) ? ACCOUNT_CSV_FILE : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", ACCOUNT_CSV_FILE);
    }

    #endregion

    #region private methods
    
    private static UserAccountCSV CurrentUser = null;

    private static List<UserAccountCSV> ReadAccountCSV()
    {
        var csvFile = GetCsvFile();
        if (File.Exists(csvFile))
        {
            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.CultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                return csv.GetRecords<UserAccountCSV>().ToList();
            }
        }

        return new List<UserAccountCSV>();
    }

    private static void WriteAccountCSV(List<UserAccountCSV> records)
    {
        var csvFile = GetCsvFile();
        using (var writer = new StreamWriter(csvFile))
        using (var csv = new CsvWriter(writer))
        {
            csv.Configuration.CultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            csv.WriteRecords(records);
        }
    }

    private static UserAccountCSV GetAccount(string userName)
    {
        return ReadAccountCSV().FirstOrDefault(x => x.UserName == userName);
    }
    
    private static List<UserAccountCSV> CreateAdmin()
    {
        var accounts = ReadAccountCSV();
        var hasAdmin = accounts.Any(x => x.Roles == DEFAULT_ADMIN_LOGIN);
        if (!hasAdmin)
        {
            Create(DEFAULT_ADMIN_LOGIN, DEFAULT_ADMIN_LOGIN, DEFAULT_ADMIN_ROLENAME);
            accounts = ReadAccountCSV();
        }

        return accounts;
    }    
    
    private static bool VerifyPasswordHash(string userPassword, byte[] passwordSalt, byte[] passwordHash)
    {
        // Verify PasswordHash
        using (var hmac = new System.Security.Cryptography.HMACSHA1(passwordSalt)) // HMACSHA512
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userPassword));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                    return false;
            }
        }

        return true;
    }    

    #endregion
}


