using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;   // ⬅️ add this
using Microsoft.AspNetCore.Mvc;
using HR.Web.Models;

namespace HR.Web.Controllers;

[Authorize] // ⬅️ protect the whole controller (so "/" forces login)
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]  // ⬅️ optional: let anyone view Privacy
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]  // ⬅️ important: keep Error reachable without auth
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
