using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
                    Logger.Log($"Adding ApplicationParts for pre-compiled view assembly {relatedAssembly.FullName}");
                    // we can safely say that this assembly is auto generated pre compiled RCL
                    if (relatedAssembly.FullName?.Contains(".Views,") ?? false)
                    {
                        AddApplicationParts(ref applicationParts, relatedAssembly);
                    }
                }

                if (relatedAssemblies.Any())
                {
                    Logger.Log($"Adding ApplicationParts for main assembly {assembly.GetName()}");
                    // For View Component Feature and others to work, we need to add the application part of the main RCL assembly.
                    AddApplicationParts(ref applicationParts, assembly);
                }
            }

            return applicationParts;
        }

        private static void AddApplicationParts(ref List<ApplicationPart> applicationParts, Assembly assembly)
        {
            var applicationPartFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
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
    }
}
