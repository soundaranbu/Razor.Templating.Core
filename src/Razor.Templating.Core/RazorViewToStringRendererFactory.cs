using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Razor.Templating.Core.Infrastructure;
using System;
using System.Collections.Generic;
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

            Log($"Assemblies Base Directory: {assembliesBaseDirectory}");
            Log($"Main Executable Directory: {mainExecutableDirectory}");
            Log($"Web Root Directory: {webRootDirectory}");

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
                var parts = GetApplicationParts();
                Log($"Found {parts.Count} application parts");
                foreach (var part in parts)
                {
                    // For MVC projects, application parts are already added by the framework
                    if (!manager.ApplicationParts.Any(x => x.Name == part.Name))
                    {
                        manager.ApplicationParts.Add(part);
                        Log($"Application part added {part.Name}");
                    }
                    else
                    {
                        Log($"Application part already added {part.Name}");
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
        /// Looks for Razor Class Library(RCL) assemblies at the given directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>Absolute paths of the RCL assemblies</returns>
        private List<string> GetRazorClassLibraryAssemblyFilesPath(string directory)
        {
            return Directory.GetFiles(directory, "*.Views.dll").ToList();
        }

        /// <summary>
        /// Get all the application parts that are available in the published project
        /// What is application part? https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?view=aspnetcore-5.0
        /// </summary>
        /// <returns></returns>
        private static List<ApplicationPart> GetApplicationParts()
        {
            // In ASP.NET Core MVC, the main entry assembly has ApplicationPartAttibute using which the RCL libraries
            // are identified and their application parts are loaded. This attibute is added only when the project attibute
            // is set to Microsoft.NET.Sdk.Web
            // But for other types of projects whose sdk is generally Microsoft.NET.Sdk there won't be any ApplicationPartAttribute added
            // Hence we need to look over all the assemblies and see if they are RCL assemblies
            // Thanks to ASP.NET Core source code https://github.com/dotnet/aspnetcore/tree/v5.0.5/src/Mvc/Mvc.Core/src/ApplicationParts
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var applicationParts = new List<ApplicationPart>();
            var seenAssemblies = new HashSet<Assembly>();
            foreach (var assembly in allAssemblies)
            {
                if (!seenAssemblies.Add(assembly))
                {
                    continue;
                }

                // RCL libraries are decorated with RelatedAssemblyAttribute for the compiled Views assembly
                // Example: ExampleRazorTemplatesLibrary.dll will contain [assembly: RelatedAssembly("ExampleRazorTemplatesLibrary.Views")]
                var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, false);
                foreach (var relatedAssembly in relatedAssemblies)
                {
                    // we can safely say that this assembly is auto generated pre compiled RCL
                    if (relatedAssembly.ManifestModule.Name?.EndsWith(".Views.dll") ?? false)
                    {
                        var applicationPartFactory = ApplicationPartFactory.GetApplicationPartFactory(relatedAssembly);
                        var viewAssemblyApplicationParts = applicationPartFactory.GetApplicationParts(relatedAssembly);
                        applicationParts.AddRange(viewAssemblyApplicationParts);
                    }
                }

                if (relatedAssemblies.Any())
                {
                    // For View Component Feature and others to work, we need to add the application part of the main RCL assembly.
                    var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                    var mainViewAssemblyApplicationParts = partFactory.GetApplicationParts(assembly);
                    applicationParts.AddRange(mainViewAssemblyApplicationParts);
                }
            }

            return applicationParts;
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

        private void Log(string message)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Constants.LibraryIdentifier}: {message}");
            Console.ResetColor();
#endif
        }
    }
}
