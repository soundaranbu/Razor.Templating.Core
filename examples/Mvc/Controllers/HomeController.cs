using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using Razor.Templating.Core;
using System.Diagnostics;

namespace Mvc.Controllers
{
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

        /// <summary>
        /// Add /Home/Inject to the URL to view this  page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Inject()
        {
            //Render View From the Web Application with Injected Service
            var renderedString = await RazorTemplateEngine.RenderAsync("~/Views/Home/Inject.cshtml");
            return Ok(renderedString);
        }

        public async Task<IActionResult> Template()
        {
            //Render View From the Template Library Application
            var renderedString = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml");
            return Ok(renderedString);
        }

        /// <summary>
        /// Add /Home/ViewComponent to the URL to view this page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ViewComponent()
        {
            //Render View From the Web Application with View Component
            var renderedString = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithViewComponent.cshtml");
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
