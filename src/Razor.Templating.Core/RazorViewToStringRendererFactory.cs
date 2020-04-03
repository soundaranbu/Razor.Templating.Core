using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
namespace Razor.Templating.Core
{
    public static class RazorViewToStringRendererFactory
    {
        public static RazorViewToStringRenderer CreateRenderer()
        {
            var services = new ServiceCollection();
            var appDirectory = Directory.GetCurrentDirectory();
            var viewAssemblyFiles = Directory.GetFiles(appDirectory, "*.Views.dll");
            var viewAssemblies = new List<Assembly>();
            foreach (var assemblyFile in viewAssemblyFiles)
            {
                viewAssemblies.Add(Assembly.LoadFile(assemblyFile));
            }
            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly().GetName().Name 
            });
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<DiagnosticSource>(new DiagnosticListener("Microsoft.AspNetCore"));
            services.AddLogging();
            var builder = services.AddMvcCore()
                                  .AddRazorViewEngine(o =>
                                  {
                                      o.FileProviders.Add(new PhysicalFileProvider(appDirectory));
                                  });
            //https://stackoverflow.com/questions/52041011/aspnet-core-2-1-correct-way-to-load-precompiled-views
            //load view assembly application parts to find the view from shared libraries
            foreach (var viewAssembly in viewAssemblies)
            {
                builder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(viewAssembly));
            }

            services.AddSingleton<RazorViewToStringRenderer>();
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<RazorViewToStringRenderer>();
        }
    }
}
