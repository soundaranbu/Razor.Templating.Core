using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Razor.Templating.Core;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace HtmlToPdf.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly ILogger<PdfController> _logger;
        public PdfController(ILogger<PdfController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            var html = await RazorTemplateEngine.RenderAsync("~/Views/PdfTemplate.cshtml");

            var rs = new LocalReporting()
                        .KillRunningJsReportProcesses()
                        .UseBinary(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? JsReportBinary.GetBinary() : jsreport.Binary.Linux.JsReportBinary.GetBinary())
                        .Configure(cfg => cfg.AllowedLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory())
                        .AsUtility()
                        .Create();

            var generatedPdf = await rs.RenderAsync(new RenderRequest
            {
                Template = new Template
                {
                    Recipe = Recipe.ChromePdf,
                    Engine = Engine.None,
                    Content = html,
                    Chrome = new Chrome
                    {
                        MarginTop = "10",
                        MarginBottom = "10",
                        MarginLeft = "50",
                        MarginRight = "50"
                    }
                }
            });

            return File(generatedPdf.Content, generatedPdf.Meta.ContentType, "GeneratedPdfFile.pdf");
        }
    }
}
