using ExampleWebApp.Net6_0.Models;
using Microsoft.AspNetCore.Mvc;
using Razor.Templating.Core;
using System.Diagnostics;

namespace ExampleWebApp.Net6_0.Controllers;

public class HomeController : Controller
{
    private readonly IRazorTemplateEngine _engine;

    public HomeController(IRazorTemplateEngine engine)
    {
        _engine = engine;
    }

    public async Task<IActionResult> Index()
    {
        var html = await _engine.RenderAsync("Index");
        return Content(html);
    }

    public async Task<IActionResult> RenderRcl()
    {
        var html = await _engine.RenderAsync("~/Views/ExampleViewWithTagHelpers.cshtml");
        return Content(html);
    }
    public async Task<IActionResult> RenderPartialTest()
    {
        var testval = "This will get added to the partial view";
        var html = await _engine.RenderPartialAsync("~/Views/Home/_PartialTest.cshtml", testval);
        return Content(html);
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