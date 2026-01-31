using EmbeddedRazorTemplates;
using EmbeddedResourceTemplates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Razor.Templating.Core;

// Force load the assembly into the AppDomain BEFORE AddRazorTemplating() is called.
// This ensures ApplicationPartsManager.GetRclAssemblies() discovers it.
_ = typeof(EmbeddedResourceProjectMarker).Assembly;
_ = typeof(EmbeddedRazorTemplatesProjectMarker).Assembly;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddRazorTemplating();
    })
    .Build();


// ViewBag and ViewData are the same dictionary internally
var viewData = new Dictionary<string, object>();
viewData["Value1"] = $"Loading at {DateTime.Now.ToString("u")}";
viewData["Value2"] = "from caller ViewData";

ShowRazorTemplateModel();
ShowEmbeddedResourceModel();


// local helper functions

async void ShowRazorTemplateModel()
{
    var model = new EmbeddedRazorTemplates.Models.ExampleModel()
    {
        PlainText = "Hello from embedded resource in a class library!",
        HtmlContent = "<strong>This is bold text from the model.</strong>"
    };

    var viewPath = "/RazorViews/EmbeddedResourceView.cshtml";
    var html = await RazorTemplateEngine.RenderAsync(viewPath, model, viewData);

    Console.WriteLine("Rendered embedded view from class library:");
    Console.WriteLine(html);

}

async void ShowEmbeddedResourceModel()
{
    var model = new EmbeddedResourceTemplates.Models.ExampleModel()
    {
        PlainText = "Hello from embedded resource in a razor web app!",
        HtmlContent = "<strong>This is bold text from the model.</strong>"
    };

    var viewPath = "/Views/Embedded/EmbeddedResourceView.cshtml";
    var html = await RazorTemplateEngine.RenderAsync(viewPath, model, viewData);

    Console.WriteLine("Rendered embedded view from razor web app:");
    Console.WriteLine(html);
}

