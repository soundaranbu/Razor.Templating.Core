using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Razor.Templating.Core.Infrastructure
{
    internal static class ApplicationPartsManager
    {
        /// <summary>
        /// Get all the application parts that are available in the published project
        /// What is application part? https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?view=aspnetcore-5.0
        /// </summary>
        /// <returns></returns>
        public static List<ApplicationPart> GetApplicationParts()
        {
            var rclAssemblies = GetRclAssemblies();
            var applicationParts = new List<ApplicationPart>();
            foreach (var assembly in rclAssemblies.Distinct())
            {
                AddApplicationParts(ref applicationParts, assembly);
            }

            return applicationParts;
        }

        /// <summary>
        /// To get the consolidated application parts for an assembly
        /// </summary>
        /// <param name="applicationParts"></param>
        /// <param name="assembly"></param>
        private static void AddApplicationParts(ref List<ApplicationPart> applicationParts, Assembly assembly)
        {
            var applicationPartFactory = ConsolidatedAssemblyApplicationPartFactory.GetApplicationPartFactory(assembly);
            var assemblyApplicationParts = applicationPartFactory.GetApplicationParts(assembly);
            applicationParts.AddRange(assemblyApplicationParts);
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
            if (string.IsNullOrEmpty(executingAssemblyLocation))
            {
                return assemblies;
            }

            var binPath = Path.GetDirectoryName(executingAssemblyLocation);
            var dllFiles = Directory.GetFiles(binPath!, "*.dll", SearchOption.TopDirectoryOnly);
            Logger.Log($"Found {dllFiles?.Length} dll files in executing assembly path");

            foreach (var dll in dllFiles ?? Array.Empty<string>())
            {
                try
                {
                    var loadedAssembly = Assembly.LoadFrom(dll);
                    assemblies.Add(loadedAssembly);
                }
                catch (Exception e)
                {
                    Logger.Log($"Error while loading dll {dll}. Error {e.Message}");
                }
            }

            return assemblies;
        }

        /// <summary>
        /// Get all assemblies used in the project to filter out the Razor Class Library
        /// Background: In ASP.NET Core MVC, the main entry assembly has ApplicationPartAttribute using which the RCL libraries
        /// are identified and their application parts are loaded. This attribute is added only when the project sdk
        /// is set to Microsoft.NET.Sdk.Web
        /// But for other types of projects whose sdk is generally Microsoft.NET.Sdk there won't be any ApplicationPartAttribute added
        /// Hence we need to look over all the assemblies and see if they are RCL assemblies
        /// Thanks to ASP.NET Core source code https://github.com/dotnet/aspnetcore/tree/v5.0.5/src/Mvc/Mvc.Core/src/ApplicationParts
        /// In .NET 6, there won't be any separate view assembly produced
        /// </summary>
        /// <returns></returns>
        private static List<Assembly> GetAllAssemblies()
        {

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            // To support Azure Functions
            var binAssemblies = GetAllBinDirectoryAssemblies();
            var assemblies = binAssemblies.ToList();
            if (assemblies?.Any() ?? false)
            {
                allAssemblies.AddRange(assemblies);
            }

            return allAssemblies;
        }

        /// <summary>
        /// Get the list of all razor class libraries excluding non-RCL libraries
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetRclAssemblies()
        {
            var rclAssemblies = new List<Assembly>();
            var allAssemblies = GetAllAssemblies();

            foreach (var assembly in allAssemblies)
            {
                var hasAnyMvcReference = assembly.GetReferencedAssemblies().Select(x => x.Name).Intersect(RclReferences).Any();
                if (hasAnyMvcReference)
                {
                    rclAssemblies.Add(assembly);
                }
            }

            return rclAssemblies;
        }
        
        /// <summary>
        /// If any of the following references are added to library, then the library is said to RCL library
        /// </summary>
        private static readonly List<string> RclReferences = new()
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
