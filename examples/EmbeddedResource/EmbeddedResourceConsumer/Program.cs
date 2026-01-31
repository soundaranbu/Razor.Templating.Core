using EmbeddedResourceTemplates;
using EmbeddedResourceTemplates.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Razor.Templating.Core;

// Force load the assembly into the AppDomain BEFORE AddRazorTemplating() is called.
// This ensures ApplicationPartsManager.GetRclAssemblies() discovers it.
_ = typeof(EmbeddedResourceProjectMarker).Assembly;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddRazorTemplating();
    })
    .Build();

var model = new ExampleModel
{
    PlainText = "Hello from embedded resource!",
    HtmlContent = "<strong>This is bold text from the model.</strong>"
};

// ViewBag and ViewData are the same dictionary internally
var viewData = new Dictionary<string, object>();
viewData["Value1"] = $"Loading at {DateTime.Now.ToString("u")}";
viewData["Value2"] = "from caller ViewData";

var viewPath = "/Views/Embedded/EmbeddedResourceView.cshtml";

var html = await RazorTemplateEngine.RenderAsync(viewPath, model, viewData);

Console.WriteLine("Rendered embedded view:");
Console.WriteLine(html);

