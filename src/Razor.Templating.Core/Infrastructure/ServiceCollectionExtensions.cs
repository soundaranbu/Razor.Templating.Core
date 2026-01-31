using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Razor.Templating.Core;
using Razor.Templating.Core.Infrastructure;
using System.Diagnostics;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the required Razor templating services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is null</exception>
    /// <exception cref="InvalidOperationException">
    /// This has been called again after the <see cref="RazorTemplateEngine"/> has already been initialized.</exception>
    public static void AddRazorTemplating(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        //ref: https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file#api-incompatibility
        var assembliesBaseDirectory = AppContext.BaseDirectory;

        //in .net 5, RCL assemblies are located next the main executable even if /p:IncludeAllContentForSelfExtract=true is provided while publishing
        //also when .net core 3.1 project is published using .net 5 sdk, above scenario happens
        //so, additionally look for RCL assemblies at the main executable directory as well
        var mainExecutableDirectory = DirectoryHelper.GetMainExecutableDirectory();

        //To add support for MVC application
        var webRootDirectory = DirectoryHelper.GetWebRootDirectory(assembliesBaseDirectory);

        Logger.Log($"Assemblies Base Directory: {assembliesBaseDirectory}");
        Logger.Log($"Main Executable Directory: {mainExecutableDirectory}");
        Logger.Log($"Web Root Directory: {webRootDirectory}");

        var fileProvider = new PhysicalFileProvider(assembliesBaseDirectory);

        // MVC, API applications will have this object already.
        if (!services.Any(x => x.ServiceType == typeof(IWebHostEnvironment)))
        {
            services.TryAddSingleton<IWebHostEnvironment>(new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? Constants.LibraryIdentifier,
                ContentRootPath = assembliesBaseDirectory,
                ContentRootFileProvider = fileProvider,
                WebRootPath = webRootDirectory,
                WebRootFileProvider = new PhysicalFileProvider(webRootDirectory)
            });
        }

        services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.TryAddSingleton<DiagnosticSource>(new DiagnosticListener(Constants.LibraryIdentifier));
        services.TryAddSingleton<DiagnosticListener>(new DiagnosticListener(Constants.LibraryIdentifier));
        services.TryAddSingleton<ConsolidatedAssemblyApplicationPartFactory>();
        services.AddLogging();
        services.AddHttpContextAccessor();

        var builder = services.AddMvcCore().AddRazorViewEngine();

        // If the host app references Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation,
        // enable it so raw .cshtml (from embedded providers) can be compiled at runtime.
        TryAddRazorRuntimeCompilation(builder);

        //ref: https://stackoverflow.com/questions/52041011/aspnet-core-2-1-correct-way-to-load-precompiled-views
        //load view assembly application parts to find the view from shared libraries
        builder.ConfigureApplicationPartManager(manager =>
        {
            var parts = ApplicationPartsManager.GetApplicationParts();
            Logger.Log($"Found {parts.Count} application parts");
            foreach (var part in parts)
            {
                // For MVC projects, application parts are already added by the framework
                if (!manager.ApplicationParts.Any(x => x.GetType() == part.GetType() && x.Name == part.Name))
                {
                    manager.ApplicationParts.Add(part);
                    Logger.Log($"Application part added {part.Name} {part.GetType().Name}");
                }
                else
                {
                    Logger.Log($"Application part already added {part.Name} {part.GetType().Name}");
                }
            }
        });

        services.TryAddTransient<RazorViewToStringRenderer>();
        services.TryAddTransient<IRazorTemplateEngine, RazorTemplateEngineRenderer>();

        // ensure the static class uses the same service collection for building the IRazorTemplateEngine
        // perform at end so no race condition with service registration
        RazorTemplateEngine.UseServiceCollection(services);
    }

    private static void TryAddRazorRuntimeCompilation(IMvcCoreBuilder mvcBuilder)
    {
        // Extension method lives in Microsoft.Extensions.DependencyInjection.RazorRuntimeCompilationMvcCoreBuilderExtensions
        // in Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation.
        const string extensionsTypeName = "Microsoft.Extensions.DependencyInjection.RazorRuntimeCompilationMvcCoreBuilderExtensions, Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation";
        var extensionsType = Type.GetType(extensionsTypeName, throwOnError: false);
        if (extensionsType is null)
        {
            // Package not referenced by the host.
            return;
        }

        // Find the overload that accepts an Action<MvcRazorRuntimeCompilationOptions>
        var addMethodWithConfig = extensionsType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "AddRazorRuntimeCompilation" && m.GetParameters().Length == 2);

        if (addMethodWithConfig is null)
        {
            // Fallback to the simple overload
            var addMethod = extensionsType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "AddRazorRuntimeCompilation" && m.GetParameters().Length == 1);

            if (addMethod is not null)
            {
                addMethod.Invoke(null, new object[] { mvcBuilder });
            }
            return;
        }

        // Get the MvcRazorRuntimeCompilationOptions type
        const string optionsTypeName = "Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation.MvcRazorRuntimeCompilationOptions, Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation";
        var optionsType = Type.GetType(optionsTypeName, throwOnError: false);
        if (optionsType is null)
        {
            return;
        }

        // Create the Action<MvcRazorRuntimeCompilationOptions> delegate
        var configureActionType = typeof(Action<>).MakeGenericType(optionsType);
        var configureMethod = typeof(ServiceCollectionExtensions).GetMethod(
            nameof(ConfigureRuntimeCompilationOptions),
            BindingFlags.NonPublic | BindingFlags.Static);

        if (configureMethod is null)
        {
            return;
        }

        var genericConfigureMethod = configureMethod.MakeGenericMethod(optionsType);
        var configureDelegate = Delegate.CreateDelegate(configureActionType, genericConfigureMethod);

        // Call AddRazorRuntimeCompilation(mvcBuilder, configureAction)
        addMethodWithConfig.Invoke(null, new object[] { mvcBuilder, configureDelegate });
    }

    private static void ConfigureRuntimeCompilationOptions<TOptions>(TOptions options) where TOptions : class
    {
        // Get FileProviders property via reflection
        var fileProvidersProperty = typeof(TOptions).GetProperty("FileProviders");
        if (fileProvidersProperty is null) return;

        var fileProviders = fileProvidersProperty.GetValue(options) as IList<IFileProvider>;
        if (fileProviders is null) return;

        // Collect embedded file providers for assemblies with embedded .cshtml resources
        var embeddedProviders = GetEmbeddedFileProviders();
        foreach (var provider in embeddedProviders)
        {
            fileProviders.Add(provider);
            Logger.Log($"Added embedded file provider for assembly");
        }
    }

    private static List<IFileProvider> GetEmbeddedFileProviders()
    {
        var providers = new List<IFileProvider>();

        // Add embedded providers for any assembly that has embedded .cshtml resources
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            TryAddEmbeddedFileProvider(providers, assembly);
        }

        // Additionally scan the bin directory (Azure Functions / plugin scenarios)
        var executingLocation = Assembly.GetExecutingAssembly().Location;
        if (!string.IsNullOrWhiteSpace(executingLocation))
        {
            var binPath = System.IO.Path.GetDirectoryName(executingLocation);
            if (!string.IsNullOrWhiteSpace(binPath))
            {
                foreach (var dll in System.IO.Directory.GetFiles(binPath, "*.dll", System.IO.SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(dll);
                        TryAddEmbeddedFileProvider(providers, assembly);
                    }
                    catch
                    {
                        // Ignore assemblies that can't be loaded
                    }
                }
            }
        }

        return providers;
    }

    private static void TryAddEmbeddedFileProvider(ICollection<IFileProvider> providers, Assembly assembly)
    {
        try
        {
            if (!assembly.GetManifestResourceNames().Any(n => n.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            providers.Add(new EmbeddedFileProvider(assembly));
        }
        catch
        {
            // Ignore failures (dynamic assemblies, etc.)
        }
    }
}
