using ASPNETWebApp48.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

[Table("Users")]
public partial class UserAccount
{
    // Change this to your desired default admin login and password
    public const string DEFAULT_ADMIN_LOGIN = "admin";
    // Change this to your DbContext class
    private static MyApp48DbContext _db = new MyApp48DbContext();


    #region UserAccountRepository
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    //Login info
    [Required]
    [StringLength(50)]
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

    private static UserAccount CurrentUser = null;

    private static UserAccount GetSingleUser(string userName)
    {
        return _db.Users.SingleOrDefault(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
    }

    public static bool Authenticate(string userName, string userPassword)
    {
        CreateAdmin(); // Comment out this line if you already have admin account

        if (userName.Contains("@"))
            if (userName.Split('@')[0].ToLower().Equals(DEFAULT_ADMIN_LOGIN.ToLower()))
                userName = DEFAULT_ADMIN_LOGIN;

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userPassword))
            return false;

        var user = GetSingleUser(userName);
        if (user == null)
            return false;
        if (!user.IsActive)
            return false;

        bool valid = VerifyPasswordHash(userPassword, user.PasswordSalt, user.PasswordHash);
        if (valid)
        {
            user.LastLogin = DateTime.Now;
            _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            CurrentUser = user; // Set current user
            return true;
        }

        return false;
    }

    private static bool VerifyPasswordHash(string userPassword, byte[] passwordSalt, byte[] passwordHash)
    {
        // Verify PasswordHash
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
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

    public static Guid? Create(string userName, string userPassword, string userRoles = "", bool requiresActivation = false)
    {
        if (string.IsNullOrWhiteSpace(userPassword))
            return null;
        if (string.IsNullOrWhiteSpace(userName) || userName.Any(Char.IsWhiteSpace))
            return null;

        var user = new UserAccount
        {
            Id = Guid.NewGuid(),
            UserName = userName.Trim().ToLower()
        };        

        var userExists = _db.Users.Any(x => x.UserName == user.UserName);
        if (userExists)
            return null;

        // Create PasswordHash
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userPassword));
        }

        user.Roles = System.Text.RegularExpressions.Regex.Replace(userRoles, @"\s+", "");
        user.CreatedOn = DateTime.Now;
        user.IsActive = !requiresActivation;

        _db.Users.Add(user);
        _db.SaveChanges();

        return user.Id;
    }

    private static void CreateAdmin()
    {
        var hasAdmin = _db.Users.Any(x => x.Roles == DEFAULT_ADMIN_ROLENAME);
        if (!hasAdmin)
        {
            Create(DEFAULT_ADMIN_LOGIN, DEFAULT_ADMIN_LOGIN, DEFAULT_ADMIN_ROLENAME);
        }
    }

    public static bool ChangePassword(string userName, string userPassword = "", string newPassword = "", bool forceChange = false)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            return false;

        if (forceChange == false && string.IsNullOrWhiteSpace(userPassword))
            return false;

        var user = GetSingleUser(userName);
        if (user == null)
            return false;

        var validPassword = !forceChange ? VerifyPasswordHash(userPassword, user.PasswordSalt, user.PasswordHash) : true;
        if (validPassword)
        {
            // Overwrite with new PasswordHash
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPassword));
            }

            _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return true;
        }
        else
            return false;

    }

    public static List<UserAccount> GetAll()
    {
        var users = _db.Users.ToList();
        users.ForEach(x =>
        {
            x.PasswordHash = null;
            x.PasswordSalt = null;
        });
        return users;
    }

    public static List<UserAccount> GetAllUsersInRole(string role)
    {
        var users = _db.Users.Where(x => x.Roles.Split(',').Contains(role)).ToList();
        users.ForEach(x =>
        {
            x.PasswordHash = null;
            x.PasswordSalt = null;
        });
        return users;
    }

    public static UserAccount GetUserById(Guid userId)
    {
        var user = _db.Users.Find(userId);
        return user;
    }

    public static UserAccount GetUserByUserName(string userName)
    {
        if (userName.Contains("@"))
            if (userName.Split('@')[0].ToLower().Equals(DEFAULT_ADMIN_LOGIN.ToLower()))
                userName = DEFAULT_ADMIN_LOGIN;

        return GetSingleUser(userName);
    }

    public static UserAccount GetCurrentUser()
    {
        return CurrentUser;
    }

    public static string[] GetUserRoles(Guid userId)
    {
        var user = GetUserById(userId);
        if (user != null)
            return user.Roles.Split(',');
        else
            return new string[] { string.Empty };
    }

    public static string[] GetUserRoles(string userName)
    {
        var user = GetUserByUserName(userName);
        return GetUserRoles(user.Id);
    }

    public static string SetUserActivation(string userName, bool isActive)
    {
        var user = GetSingleUser(userName);
        if (user != null)
        {
            user.IsActive = isActive;
            _db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return "success";
        }
        else
            return "failed";
    }
    
    #endregion

}


