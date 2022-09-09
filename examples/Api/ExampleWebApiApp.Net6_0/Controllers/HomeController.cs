using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Logging;
using Razor.Templating.Core;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExampleWebApiApp.Net6_0.Controllers;

public class HomeController : Controller
{

    private readonly ILogger<ApiController> _logger;
    private readonly ApplicationPartManager _applicationPartManager;
    private readonly IRazorTemplateEngine _engine;

    public HomeController(ILogger<ApiController> logger, ApplicationPartManager applicationPartManager, IRazorTemplateEngine engine)
    {
        _logger = logger;
        _applicationPartManager = applicationPartManager;
        _engine = engine;
    }

    public object GetApplicationParts()
    {
        var response = new {
            ApplicationParts = JsonSerializer.Serialize(_applicationPartManager.ApplicationParts),
            FeatureProviders = JsonSerializer.Serialize(_applicationPartManager.FeatureProviders)
        };

        return response;
    }

    public IActionResult Index()
    {
        //Render View From the Web Application
        return View();
    }

    public async Task<IActionResult> Render()
    {
        //Render View From the Web Application
        var renderedString = await _engine.RenderAsync("/Views/Home/Index.cshtml");
        return Ok(renderedString);
    }

    public async Task<IActionResult> Render1()
    {
        //Render View From the Web Application
        var renderedString = await _engine.RenderAsync("~/Views/Feature/ExampleViewWithoutViewModel.cshtml");
        return Ok(renderedString);
    }
}
