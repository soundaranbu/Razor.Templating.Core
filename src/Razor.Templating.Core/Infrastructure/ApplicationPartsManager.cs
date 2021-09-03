using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;

namespace Razor.Templating.Core.Infrastructure
{
    internal class ApplicationPartsManager
    {
        /// <summary>
        /// Get all the application parts that are available in the published project
        /// What is application part? https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?view=aspnetcore-5.0
        /// </summary>
        /// <returns></returns>
        public static List<ApplicationPart> GetApplicationParts()
        {
            var rclAssemblies = GetRCLAssemblies();
            var applicationParts = new List<ApplicationPart>();
            var seenAssemblies = new HashSet<Assembly>();
            foreach (var assembly in rclAssemblies)
            {
                if (!seenAssemblies.Add(assembly))
                {
                    continue;
                }

                AddApplicationParts(ref applicationParts, assembly);
            }

            return applicationParts;
        }

        private static void AddApplicationParts(ref List<ApplicationPart> applicationParts, Assembly assembly)
        {
            var applicationPartFactory = ConsolidatedAssemblyApplicationPartFactory.GetApplicationPartFactory(assembly);
            var assemblyApplicationParts = applicationPartFactory.GetApplicationParts(assembly);

            foreach (var assemblyApplicationPart in assemblyApplicationParts)
            {
                if (!applicationParts.Any(applicationPart => applicationPart.Name == assemblyApplicationPart.Name))
                {
                    applicationParts.Add(assemblyApplicationPart);
                }
                else
                {
                    Logger.Log($"ApplicationPart {assemblyApplicationPart.Name} already added");
                }
            }
        }

        /// <summary>
        /// Get all assemblies in the bin directory.
        /// This is specifically for Azure functions
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetAllBinDirectoryAssemblies()
        {
            // for single file apps, this returns null
            var executingAssemblyLocation = Assembly.GetExecutingAssembly()?.Location;
            var assemblies = new List<Assembly>();
            Logger.Log($"Executing Assembly Bin Path: {executingAssemblyLocation}");
            if (!string.IsNullOrEmpty(executingAssemblyLocation))
            {
                var binPath = Path.GetDirectoryName(executingAssemblyLocation);
                var dllFiles = Directory.GetFiles(binPath, "*.dll", SearchOption.TopDirectoryOnly);
                Logger.Log($"Found {dllFiles?.Count()} dll files in executing assembly path");
                foreach (string dll in dllFiles ?? new string[] { })
                {
                    try
                    {
                        Assembly loadedAssembly = Assembly.LoadFile(dll);
                        assemblies.Add(loadedAssembly);
                    }
                    catch (Exception e)
                    {
                        Logger.Log($"Error while loading dll {dll}");
                    }
                }
            }

            return assemblies;
        }

        private static List<Assembly> GetAllAssemblies()
        {
            // In ASP.NET Core MVC, the main entry assembly has ApplicationPartAttibute using which the RCL libraries
            // are identified and their application parts are loaded. This attibute is added only when the project sdk
            // is set to Microsoft.NET.Sdk.Web
            // But for other types of projects whose sdk is generally Microsoft.NET.Sdk there won't be any ApplicationPartAttribute added
            // Hence we need to look over all the assemblies and see if they are RCL assemblies
            // Thanks to ASP.NET Core source code https://github.com/dotnet/aspnetcore/tree/v5.0.5/src/Mvc/Mvc.Core/src/ApplicationParts
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            // To support Azure Functions
            var binAssemblies = GetAllBinDirectoryAssemblies();
            if (binAssemblies?.Any() ?? false)
            {
                allAssemblies.AddRange(binAssemblies);
            }

            return allAssemblies;
        }

        private static List<Assembly> GetRCLAssemblies()
        {
            var rclAssemblies = new List<Assembly>();
            var allAssemblies = GetAllAssemblies();

            //foreach (var assembly in allAssemblies.Where(x => x.FullName?.Contains("ExampleRazorTemplatesLibrary") is true))
            foreach (var assembly in allAssemblies)
            {
                var hasAnyMvcReference = assembly.GetReferencedAssemblies().Select(x => x.Name).Intersect(RCLReferences).Any();
                if (hasAnyMvcReference)
                {
                    rclAssemblies.Add(assembly);
                }
            }

            return rclAssemblies;
        }


        private static List<string> RCLReferences = new()
        {
            "Microsoft.AspNetCore.Mvc",
            "Microsoft.AspNetCore.Mvc.Abstractions",
            "Microsoft.AspNetCore.Mvc.ApiExplorer",
            "Microsoft.AspNetCore.Mvc.Core",
            "Microsoft.AspNetCore.Mvc.Cors",
            "Microsoft.AspNetCore.Mvc.DataAnnotations",
            "Microsoft.AspNetCore.Mvc.Formatters.Json",
            "Microsoft.AspNetCore.Mvc.Formatters.Xml",
            "Microsoft.AspNetCore.Mvc.Localization",
            "Microsoft.AspNetCore.Mvc.NewtonsoftJson",
            "Microsoft.AspNetCore.Mvc.Razor",
            "Microsoft.AspNetCore.Mvc.RazorPages",
            "Microsoft.AspNetCore.Mvc.TagHelpers",
            "Microsoft.AspNetCore.Mvc.ViewFeatures"
        };
    }
}
