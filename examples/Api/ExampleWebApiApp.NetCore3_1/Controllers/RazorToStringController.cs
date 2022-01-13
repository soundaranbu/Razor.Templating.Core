using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Razor.Templating.Core;
using System.Threading.Tasks;

namespace ExampleWebApiApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RazorToStringController : ControllerBase
    {
        private readonly ILogger<RazorToStringController> _logger;

        public RazorToStringController(ILogger<RazorToStringController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var renderedString = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml");
            return Ok(renderedString);
        }

        [HttpGet("/TagHelpers")]
        public async Task<IActionResult> TagHelpers()
        {
            var renderedString = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithTagHelpers.cshtml");
            return Ok(renderedString);
        }
    }
}
