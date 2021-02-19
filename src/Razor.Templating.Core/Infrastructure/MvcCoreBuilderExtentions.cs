using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class MvcCoreBuilderExtentions
    {
        /// <summary>
        /// Loads the RCL assemblies to the application parts.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="viewAssemblyFiles"></param>
        internal static void AddViewAssemblyApplicationParts(this IMvcCoreBuilder builder, List<string> viewAssemblyFiles)
        {
            foreach (var assemblyFile in viewAssemblyFiles)
            {
                var viewAssembly = Assembly.LoadFile(assemblyFile);

                builder.PartManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(viewAssembly));
            }
        }
    }
}
