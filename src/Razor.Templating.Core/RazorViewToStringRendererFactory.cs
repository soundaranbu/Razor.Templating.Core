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
    internal static class RazorViewToStringRendererFactory
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
        /// Returns the instance of RazorViewToStringRenderer by resolving all the dependencies required to 
        /// successfully render the razor views to string.
        /// </summary>
        /// <returns></returns>
        public static RazorViewToStringRenderer CreateRenderer()
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

            var viewAssemblyFiles = GetRazorClassLibraryAssemblyFilesPath(assembliesBaseDirectory, mainExecutableDirectory);
            //ref: https://stackoverflow.com/questions/52041011/aspnet-core-2-1-correct-way-to-load-precompiled-views
            //load view assembly application parts to find the view from shared libraries
            builder.AddViewAssemblyApplicationParts(viewAssemblyFiles);

            services.Configure<MvcRazorRuntimeCompilationOptions>(o =>
            {
                o.FileProviders.Add(fileProvider);
            });
            services.TryAddSingleton<RazorViewToStringRenderer>();

            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<RazorViewToStringRenderer>();
        }

        /// <summary>
        /// Returns the path of the main executable file using which the application is started
        /// </summary>
        /// <returns></returns>
        private static string? GetMainExecutableDirectory()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }

        /// <summary>
        /// Looks for Razor Class Library(RCL) assemblies at the given directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>Absolute paths of the RCL assemblies</returns>
        private static List<string> GetRazorClassLibraryAssemblyFilesPath(string directory)
        {
            return Directory.GetFiles(directory, "*.Views.dll").ToList();
        }

        /// <summary>
        /// Get the all the RCL assembly file paths from all possible locations
        /// </summary>
        /// <param name="assembliesBaseDirectory"></param>
        /// <param name="mainExecutableDirectory"></param>
        /// <returns></returns>
        private static List<string> GetRazorClassLibraryAssemblyFilesPath(string assembliesBaseDirectory, string? mainExecutableDirectory)
        {
            Log("Finding razor assemblies from Assembly Base Directory");
            var viewAssemblyFiles = GetRazorClassLibraryAssemblyFilesPath(assembliesBaseDirectory);

            // if RCL assemblies are found at the main executable directory, add them as well.
            if (mainExecutableDirectory?.Length > 0 && Directory.Exists(mainExecutableDirectory) && !mainExecutableDirectory.Equals(assembliesBaseDirectory))
            {
                Log("Finding razor assemblies from Main Executable Directory");
                viewAssemblyFiles.AddRange(GetRazorClassLibraryAssemblyFilesPath(mainExecutableDirectory));
            }
            Log($"Found {viewAssemblyFiles.Count} RCL assemblies");
            Log($"Found {viewAssemblyFiles.Distinct().Count()} distinct RCL assemblies");
            Log($"Following RCL assemblies were found: {string.Join(Environment.NewLine, viewAssemblyFiles.Distinct())}");
            return viewAssemblyFiles.Distinct().ToList();
        }

        /// <summary>
        /// Get the web root directory where the static content resides. This is to add support for MVC applications
        /// If the webroot directory doesn't exist, set the path to assembly base directory.
        /// </summary>
        /// <param name="assembliesBaseDirectory"></param>
        /// <returns></returns>
        private static string GetWebRootDirectory(string assembliesBaseDirectory)
        {
            var webRootDirectory = Path.Combine(assembliesBaseDirectory, "wwwroot");
            if (!Directory.Exists(webRootDirectory))
            {
                webRootDirectory = assembliesBaseDirectory;
            }

            return webRootDirectory;
        }


        /// <summary>
        /// Loads the RCL assemblies to the application parts.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="viewAssemblyFiles"></param>
        private static void AddViewAssemblyApplicationParts(this IMvcCoreBuilder builder, List<string> viewAssemblyFiles)
        {
            foreach (var assemblyFile in viewAssemblyFiles)
            {
                var viewAssembly = Assembly.LoadFile(assemblyFile);

                builder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(viewAssembly));
            }
        }


        private static void Log(string message)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Constants.LibraryIdentifier}: {message}");
            Console.ResetColor();
#endif
        }
    }
}
