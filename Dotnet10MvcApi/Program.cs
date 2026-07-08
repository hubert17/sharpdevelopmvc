#pragma warning disable CA1416

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Dotnet10MvcApi.Data;
using Dotnet10MvcApi.Helpers;
using Dotnet10MvcApi.Models;
using Scalar.AspNetCore;
using OpenApi = Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure DBContext based on Provider setting
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "Jet";
if (dbProvider.Equals("Jet", StringComparison.OrdinalIgnoreCase))
{
    var appDataPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
    AppDomain.CurrentDomain.SetData("DataDirectory", appDataPath);

    var connString = builder.Configuration.GetConnectionString("JetConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseJet(connString));
}
else if (dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
{
    var connString = builder.Configuration.GetConnectionString("PostgreSqlConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connString));
}

// 2. Configure JWT Bearer Authentication
var secret = builder.Configuration["JwtSettings:Secret"] ?? "f848bcae3399961afba711f8ced6fc3c";
var issuer = builder.Configuration["JwtSettings:Issuer"] ?? "Dotnet10MvcApi";
var audience = builder.Configuration["JwtSettings:Audience"] ?? "Dotnet10MvcApi";

// 2. Configure Authentication (Cookie + JWT Bearer)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logoff";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});

// 3. Register standard services and native OpenAPI
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Dotnet 10 MVC & API";
        document.Info.Version = "v1";

        // Add JWT Bearer Security Scheme (HTTP Bearer type is preferred in OpenAPI v3)
        var securityScheme = new OpenApi.OpenApiSecurityScheme
        {
            Type = OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = OpenApi.ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
        };

        document.Components ??= new OpenApi.OpenApiComponents();
        if (document.Components.SecuritySchemes == null)
        {
            document.Components.SecuritySchemes = new System.Collections.Generic.Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();
        }
        document.Components.SecuritySchemes.Add("Bearer", securityScheme);

        // Apply security requirement globally to all endpoints
        var requirement = new OpenApi.OpenApiSecurityRequirement
        {
            {
                new OpenApi.OpenApiSecuritySchemeReference("Bearer", document),
                new System.Collections.Generic.List<string>()
            }
        };

        document.Security ??= new System.Collections.Generic.List<OpenApi.OpenApiSecurityRequirement>();
        document.Security.Add(requirement);

        return Task.CompletedTask;
    });
});

builder.Services.AddScoped<TokenManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 4. Set up physical static files (so wwwroot/index.html is served at /)
app.UseDefaultFiles();
app.UseStaticFiles();

// 5. Setup OpenAPI/Scalar UI
app.MapOpenApi();
app.MapScalarApiReference();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 6. Map controllers (APIs + MVC routing)
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Redirect legacy /swagger path to /scalar/v1
app.MapGet("/swagger", context =>
{
    context.Response.Redirect("/scalar/v1");
    return System.Threading.Tasks.Task.CompletedTask;
});

// 7. Auto-migrate database and seed tables on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // This will run the migration (creating RefreshTokens table)
        db.Database.Migrate();

        // Create [#Dual] table needed by EF Core Jet provider for Any() and other scalar queries
        try
        {
            db.Database.ExecuteSqlRaw("CREATE TABLE [#Dual] (Id INT)");
            db.Database.ExecuteSqlRaw("INSERT INTO [#Dual] (Id) VALUES (1)");
            Console.WriteLine("Created [#Dual] table successfully.");
        }
        catch { /* Already exists */ }

        // Print existing tables for diagnostics
        var conn = db.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open) conn.Open();
        var dt = conn.GetSchema("Tables");
        Console.WriteLine("TABLES IN ACCESS DATABASE:");
        foreach (System.Data.DataRow row in dt.Rows)
        {
            var tableName = row["TABLE_NAME"].ToString();
            var tableType = row["TABLE_TYPE"].ToString();
            if (tableType == "TABLE")
            {
                Console.WriteLine($"- {tableName}");
            }
        }

        // Seed Product table if empty
        if (!db.Products.Any())
        {
            db.Products.AddRange(Product.SeedData());
            db.SaveChanges();
            Console.WriteLine("Seeded Products table successfully.");
        }

        // Seed Songs table if empty
        if (!db.Songs.Any())
        {
            Song.Seed(db, clearSongTable: false);
            Console.WriteLine("Seeded Songs table successfully from Billboard CSV.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database Migration/Seeding Warning: {ex.Message}");
    }
}

app.Run();
