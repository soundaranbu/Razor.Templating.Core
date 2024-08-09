﻿using ExampleRazorTemplatesLibrary.Models;
using ExampleRazorTemplatesLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core;

namespace ExampleConsoleApp.Net6_0
{
    public class Program
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

        private static async Task RenderViewComponentWithoutModelAsync()
        {
            // Render View with View Component
            var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleViewWithViewComponent.cshtml");
            Console.Write(html);
        }

        private static async Task RenderViewWithModelAsync()
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

        private static async Task RenderWithDependencyInjectionAsync()
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


}
