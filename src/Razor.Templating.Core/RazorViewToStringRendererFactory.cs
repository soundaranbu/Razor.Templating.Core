using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Razor.Templating.Core.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Razor.Templating.Core
{
    internal class RazorViewToStringRendererFactory
    {
        private static IServiceCollection? _serviceCollection;

        /// <summary>
        /// Get's the new instance of ServiceCollection class, if doesn't exists
        /// </summary>
        public static IServiceCollection ServiceCollection
        {
            get
            {
                if (_serviceCollection is null)
                {
                    return new ServiceCollection();
                }

                return _serviceCollection;
            }
            set
            {
                _serviceCollection = value;
            }
        }
        /// <summary>
        /// Returns the instance of IServiceScopeFactory
        /// </summary>
        /// <returns></returns>
        public IServiceScopeFactory CreateRendererServiceScopeFactory()
        {
            var services = ServiceCollection;

            //ref: https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file#api-incompatibility
            var assembliesBaseDirectory = AppContext.BaseDirectory;

            //in .net 5, RCL assemblies are located next the main executable even if /p:IncludeAllContentForSelfExtract=true is provided while publishing
            //also when .net core 3.1 project is published using .net 5 sdk, above scenario happens
            //so, additionally look for RCL assemblies at the main executable directory as well
            var mainExecutableDirectory = GetMainExecutableDirectory();

            //To add support for MVC application
            var webRootDirectory = GetWebRootDirectory(assembliesBaseDirectory);

            Logger.Log($"Assemblies Base Directory: {assembliesBaseDirectory}");
            Logger.Log($"Main Executable Directory: {mainExecutableDirectory}");
            Logger.Log($"Web Root Directory: {webRootDirectory}");

            var fileProvider = new PhysicalFileProvider(assembliesBaseDirectory);

            services.TryAddSingleton<IWebHostEnvironment>(new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? Constants.LibraryIdentifier,
                ContentRootPath = assembliesBaseDirectory,
                ContentRootFileProvider = fileProvider,
                WebRootPath = webRootDirectory,
                WebRootFileProvider = new PhysicalFileProvider(webRootDirectory)
            });
            services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.TryAddSingleton<DiagnosticSource>(new DiagnosticListener(Constants.LibraryIdentifier));
            services.TryAddSingleton<DiagnosticListener>(new DiagnosticListener(Constants.LibraryIdentifier));
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddLogging();
            services.AddHttpContextAccessor();
            var builder = services.AddMvcCore().AddRazorViewEngine();

            //ref: https://stackoverflow.com/questions/52041011/aspnet-core-2-1-correct-way-to-load-precompiled-views
            //load view assembly application parts to find the view from shared libraries
            builder.ConfigureApplicationPartManager(manager =>
            {
                var parts = ApplicationPartsManager.GetApplicationParts();
                Logger.Log($"Found {parts.Count} application parts");
                foreach (var part in parts)
                {
                    // For MVC projects, application parts are already added by the framework
                    if (!manager.ApplicationParts.Any(x => x.Name == part.Name))
                    {
                        manager.ApplicationParts.Add(part);
                        Logger.Log($"Application part added {part.Name}");
                    }
                    else
                    {
                        Logger.Log($"Application part already added {part.Name}");
                    }
                }
            });

            services.Configure<MvcRazorRuntimeCompilationOptions>(o =>
            {
                o.FileProviders.Add(fileProvider);
            });
            services.TryAddTransient<RazorViewToStringRenderer>();

            return services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
        }

        /// <summary>
        /// Returns the path of the main executable file using which the application is started
        /// </summary>
        /// <returns></returns>
        private string? GetMainExecutableDirectory()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }

        /// <summary>
        /// Get the web root directory where the static content resides. This is to add support for MVC applications
        /// If the webroot directory doesn't exist, set the path to assembly base directory.
        /// </summary>
        /// <param name="assembliesBaseDirectory"></param>
        /// <returns></returns>
        private string GetWebRootDirectory(string assembliesBaseDirectory)
        {
            var webRootDirectory = Path.Combine(assembliesBaseDirectory, "wwwroot");
            if (!Directory.Exists(webRootDirectory))
            {
                webRootDirectory = assembliesBaseDirectory;
            }

            return webRootDirectory;
        }
    }
}
