using ExampleRazorTemplatesLibrary.Models;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ExampleConsoleApp.NetCore3_1
{
    class Program
    {
        static async Task Main(string[] args)
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
            }
            catch (System.Exception e)
            {

                System.Console.WriteLine("{0}", e);
            }

            System.Console.ReadLine();
        }
    }
}
