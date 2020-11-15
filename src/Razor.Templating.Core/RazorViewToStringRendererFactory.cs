using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
namespace Razor.Templating.Core
{
    internal static class RazorViewToStringRendererFactory
    {
        public static RazorViewToStringRenderer CreateRenderer()
        {
            var services = new ServiceCollection();
            //ref: https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file#api-incompatibility
            var appDirectory = AppContext.BaseDirectory;

            var webRootDirectory = Path.Combine(appDirectory, "wwwroot");
            if (!Directory.Exists(webRootDirectory))
            {
                webRootDirectory = appDirectory;
            }
            var viewAssemblyFiles = Directory.GetFiles(appDirectory, "*.Views.dll");
            var viewAssemblies = new List<Assembly>();
            var fileProvider = new PhysicalFileProvider(appDirectory);
            foreach (var assemblyFile in viewAssemblyFiles)
            {
                viewAssemblies.Add(Assembly.LoadFile(assemblyFile));
            }

            services.AddSingleton<IWebHostEnvironment>(new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Razor.Templating.Core",
                ContentRootPath = appDirectory,
                ContentRootFileProvider = fileProvider,
                WebRootPath = webRootDirectory,
                WebRootFileProvider = new PhysicalFileProvider(webRootDirectory)
            });
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<DiagnosticSource>(new DiagnosticListener("Razor.Templating.Core"));
            services.AddSingleton<DiagnosticListener>(new DiagnosticListener("Razor.Templating.Core"));
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddLogging();
            services.AddHttpContextAccessor();
            var builder = services.AddMvcCore().AddRazorViewEngine();
            //ref: https://stackoverflow.com/questions/52041011/aspnet-core-2-1-correct-way-to-load-precompiled-views
            //load view assembly application parts to find the view from shared libraries
            foreach (var viewAssembly in viewAssemblies)
            {
                builder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(viewAssembly));
            }

            services.Configure<MvcRazorRuntimeCompilationOptions>(o =>
            {
                o.FileProviders.Add(fileProvider);
            });
            services.AddSingleton<RazorViewToStringRenderer>();

            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<RazorViewToStringRenderer>();
        }
    }

    internal class HostingEnvironment : IWebHostEnvironment
    {
        public HostingEnvironment()
        {
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
