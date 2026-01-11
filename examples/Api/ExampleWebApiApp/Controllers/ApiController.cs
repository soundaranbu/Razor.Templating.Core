using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Razor.Templating.Core;
using System.Text.Json;

namespace WebApi.Controllers;
[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{

    private readonly ILogger<ApiController> _logger;
    private readonly ApplicationPartManager _applicationPartManager;
    private readonly IRazorTemplateEngine _engine;

    public ApiController(ILogger<ApiController> logger, ApplicationPartManager applicationPartManager, IRazorTemplateEngine engine)
    {
        _logger = logger;
        _applicationPartManager = applicationPartManager;
        _engine = engine;
    }

    [HttpGet("application-parts")]
    public object GetApplicationParts()
    {
        var response = new {
            ApplicationParts = JsonSerializer.Serialize(_applicationPartManager.ApplicationParts),
            FeatureProviders = JsonSerializer.Serialize(_applicationPartManager.FeatureProviders)
        };

        return response;
    }

    [HttpGet("render")]
    public async Task<IActionResult> Index()
    {
        //Render View From the Web Application
        var renderedString = await _engine.RenderAsync("/Views/Home/Index.cshtml");
        return Ok(renderedString);
    }
}
