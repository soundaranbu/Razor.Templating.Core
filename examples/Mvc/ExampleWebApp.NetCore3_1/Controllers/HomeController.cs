using ExampleWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Razor.Templating.Core;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ExampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Render View From the Web Application
            var renderedString = await RazorTemplateEngine.RenderAsync("~/Views/Home/Index.cshtml");
            return Ok(renderedString);
        }

        public async Task<IActionResult> Template()
        {
            //Render View From the Template Library Application
            var renderedString = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml");
            return Ok(renderedString);
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
}
