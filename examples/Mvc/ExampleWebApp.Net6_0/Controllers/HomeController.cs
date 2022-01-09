using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExampleWebApp.Net6_0.Models;
using Razor.Templating.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ExampleWebApp.Net6_0.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger, IActionContextAccessor actionContext)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        //var html = await RazorTemplateEngine.RenderAsync("~/Views/Home/Index.cshtml");
        //return Content(html);

        return View();
    }

    public async Task<IActionResult> RenderRcl()
    {
        var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithTagHelpers.cshtml");
        return Content(html);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}