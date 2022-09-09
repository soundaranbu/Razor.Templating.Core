using ExampleRazorTemplatesLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core;
using System.Threading.Tasks;

namespace ExampleConsoleApp.Net6_0
{
    public class Program
    {
        async static Task Main(string[] args)
        {
            try
            {                
                System.Console.WriteLine(DateTime.Now);
                var model = new ExampleModel()
                {
                    PlainText = "Some text",
                    HtmlContent = "<em>Some emphasized text</em>"
                };

                var viewData = new Dictionary<string, object>();
                viewData["Value1"] = "1";
                viewData["Value2"] = "2";

                var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
                System.Console.Write(html);
                System.Console.WriteLine(DateTime.Now);


                // Render View with View Component
                html = await RazorTemplateEngine.RenderAsync("/Views/ExampleViewWithViewComponent.cshtml");
                System.Console.Write(html);
                System.Console.WriteLine(DateTime.Now);

            }
            catch (System.Exception e)
            {

                System.Console.WriteLine("{0}", e);
            }

            System.Console.ReadLine();
        }
    }
}
