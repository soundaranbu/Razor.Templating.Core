using ExampleRazorTemplatesLibrary.Models;
using ExampleRazorTemplatesLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core;

namespace ExampleConsoleApp;

public class Program
{
    private async static Task Main(string[] args)
    {
        try
        {
            Console.WriteLine(DateTime.Now);

            await RenderViewWithModelAsync();
            await RenderViewComponentWithoutModelAsync();
            await RenderWithDependencyInjectionAsync();

            Console.WriteLine(DateTime.Now);
        }
        catch (Exception e)
        {

            Console.WriteLine("{0}", e);
        }

        Console.ReadLine();
    }

    private async static Task RenderViewComponentWithoutModelAsync()
    {
        // Render View with View Component
        var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleViewWithViewComponent.cshtml");
        Console.Write(html);
    }

    private async static Task RenderViewWithModelAsync()
    {
        var model = new ExampleModel()
        {
            PlainText = "Some text",
            HtmlContent = "<em>Some emphasized text</em>"
        };
        var viewData = new Dictionary<string, object>();
        viewData["Value1"] = "1";
        viewData["Value2"] = "2";

        var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
        Console.Write(html);
        Console.WriteLine(DateTime.Now);
    }

    private async static Task RenderWithDependencyInjectionAsync()
    {
        // Use service collection
        // Arrange
        var model = new ExampleModel()
        {
            PlainText = "Lorem Ipsium",
            HtmlContent = "<em>Lorem Ipsium</em>"
        };

        // Add dependencies to the service collection and add razor templating to the collection
        var services = new ServiceCollection();
        services.AddTransient<ExampleService>();
        // Add after registering all dependencies
        // this is important for the razor template engine to find the injected services
        services.AddRazorTemplating();
        // Act
        var html1 = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewServiceInjection.cshtml", model);
        Console.WriteLine(html1);
    }
}
