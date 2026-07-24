using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dotnet10MvcApi.Models;
using Dotnet10MvcApi.Models.ViewModels;

namespace Dotnet10MvcApi.Controllers.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
    [AllowAnonymous]
    public IActionResult About()
    {
        ViewBag.Message = "Your application description page.";
        return View();
    }

    public IActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";
        return View();
    }

    public IActionResult Help()
    {
        return View();
    }

    [Authorize]
    public IActionResult ForAuthUser()
    {
        ViewBag.Message = "Authorized user page.";
        return View("About");
    }

    [Authorize(Roles = "admin")]
    public IActionResult ForRoleUser()
    {
        ViewBag.Message = "Authorized ADMIN page.";
        return View("About");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class PingerController : Controller
{
    public IActionResult Index()
    {
        return Content(DateTime.Now.ToString());
    }
}

