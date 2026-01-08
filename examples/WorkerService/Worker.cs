using Razor.Templating.Core;
using ExampleRazorTemplatesLibrary.Models;

namespace WorkerService;

public class Worker(ILogger<Worker> logger, IRazorTemplateEngine engine) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation(DateTime.Now.ToString());
            var model = new ExampleModel()
            {
                PlainText = "Some text",
                HtmlContent = "<em>Some emphasized text</em>"
            };

            var viewData = new Dictionary<string, object>();
            viewData["Value1"] = "1";
            viewData["Value2"] = "2";

            var html = await engine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);

            logger.LogInformation(html);
            logger.LogInformation(DateTime.Now.ToString());
        }
        catch (System.Exception e)
        {
            logger.LogError(e, "Error rendering template");
        }
    }
}
