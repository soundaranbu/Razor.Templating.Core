using ExampleRazorTemplatesLibrary.Models;
using ExampleRazorTemplatesLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core;

namespace ExampleConsoleApp.Net6_0
{
    public class Program
    {
        async static Task Main(string[] args)
        {
            try
            {
                Console.WriteLine(DateTime.Now);

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

                // Render View with View Component
                html = await RazorTemplateEngine.RenderAsync("/Views/ExampleViewWithViewComponent.cshtml");
                Console.Write(html);

                // Use service collection
                // Arrange
                var model1 = new ExampleModel()
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

                Console.WriteLine(DateTime.Now);
            }
            catch (Exception e)
            {

                Console.WriteLine("{0}", e);
            }

            Console.ReadLine();
        }
    }
}
