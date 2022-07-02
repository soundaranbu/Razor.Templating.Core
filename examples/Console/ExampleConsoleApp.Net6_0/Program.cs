using ExampleRazorTemplatesLibrary.Models;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core;
using System.Diagnostics;

namespace ExampleConsoleApp.Net6_0
{
    class Program
    {
        async static Task Main(string[] args)
        {

            await Test2();
            System.Console.ReadLine();
        }

        static async Task Test2()
        {
            var services = new ServiceCollection();
            services.AddTransient<BlobRazorViewSource>();
            services.AddTransient<DbRazorViewProvider>();
            services.AddDbContext<TestDatabaseContext>(opt => opt.UseInMemoryDatabase("test"));

            services.AddOptions<MvcRazorRuntimeCompilationOptions>()
                .Configure<DbRazorViewProvider>((option, source) =>
                {
                    option.FileProviders.Add(source);
                });

            //services.Configure<MvcRazorRuntimeCompilationOptions>(option =>
            //{
            //    option.FileProviders.Add(new BlobRazorViewSource(""));
            //});

            services.AddRazorTemplating();

            var watch = new Stopwatch();
            watch.Start();

            //var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml");
            var html = await RazorTemplateEngine.RenderAsync("mysampleview.cshtml");
            watch.Stop();
            Console.WriteLine(html);
            Console.WriteLine(watch.ElapsedMilliseconds);

            watch.Reset();
            watch.Start();
            html = await RazorTemplateEngine.RenderAsync("mysampleview.cshtml");
            watch.Stop();
            Console.WriteLine(html);
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        static async Task Test1()
        {
            try
            {
                RazorTemplateEngine.Initialize();

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
        }
    }


}
