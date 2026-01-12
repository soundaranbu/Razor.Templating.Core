# Razor.Templating.Core

![Build+Test](https://github.com/soundaranbu/Razor.Templating.Core/workflows/Build+Test/badge.svg?branch=main) ![Nuget](https://img.shields.io/nuget/v/Razor.Templating.Core) ![Downloads](https://img.shields.io/nuget/dt/Razor.Templating.Core) 
![Coverage](https://raw.githubusercontent.com/soundaranbu/Razor.Templating.Core/main/test/Razor.Templating.Core.Test/Reports/badge_shieldsio_linecoverage_green.svg)

Using [Razor](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor) for HTML templating has never been easier. Render your .cshtml files to strings easily using this library.

This library uses precompiled Razor views provided by the [Razor SDK](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/sdk).

## Supported Application Types

|                   | .NET 6 & Above | .NET 10 & Above |
| ----------------- | -------------- | --------------- |
| Preferred Version | v2.1.0         | 3.0.0           |
| Console           | &check;        | &check;         |
| Api               | &check;        | &check;         |
| Mvc               | &check;        | &check;         |
| Worker Service    | &check;        | &check;         |
| WPF               | &check;        | &check;         |
| WinForms          | &check;        | &check;         |
| Azure Functions   | &check;        | &check;         |

For older .NET versions, refer to the [wiki](https://github.com/soundaranbu/Razor.Templating.Core/wiki#historic-versions)

## Supported View Features
| MVC Razor View Features         |         |
| ------------------------------- | ------- |
| ViewModel                       | &check; |
| ViewBag                         | &check; |
| ViewData                        | &check; |
| Layouts                         | &check; |
| ViewStarts                      | &check; |
| ViewImports                     | &check; |
| Partial Views                   | &check; |
| Tag Helpers                     | &check; |
| View Components                 | &check; |
| View Localization (Only MVC)    | &check; |
| Dependency Injection into Views | &check; |
| @Url.ContentUrl**               | &cross; |
| @Url.RouteUrl**                 | &cross; |

**Contributors are welcome who can help to enable these unsupported features.

## Applications
- Email Templating
- Report Generation & more

## Performance
Performance of rendering the views to HTML is as fast as rendering the MVC page. The first render may be slightly slower due to the initialization. But, the subsequent renderings are significantly faster. Refer to the benchmark results [here](benchmark\Razor.Template.Core.PerfBenchmark\results\RazorTemplatingCoreBenchMark-report-github.md) and run it for yourself in your own machine to verify the results.

| Method                   |     Mean |    Error |   StdDev |   Gen0 | Allocated |
| ------------------------ | -------: | -------: | -------: | -----: | --------: |
| RenderViewWithModelAsync | 28.73 μs | 0.403 μs | 0.357 μs | 5.8594 |     24 KB |

## Runtime Compilation
From .NET 10, the Razor runtime compilation APIs are [marked as obsolete](https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/10/razor-runtime-compilation-obsolete). As a result, they will be removed from ASP.NET Core in the future. If you're looking to save a Razor view in a database or in a separate folder outside the project and render it on the fly, that scenario is not supported. Microsoft recommends using build-time compilation by including the Razor view in the project and built as part of your app.


## Installing the NuGet package
This library is available as [NuGet package](https://www.nuget.org/packages/Razor.Templating.Core/)

### Using .NET CLI
```bash
dotnet add package Razor.Templating.Core
```
### Using Package Reference .csproj
```bash
<PackageReference Include="Razor.Templating.Core" Version="3.0.0" />
```

## Usage - Render View With Layout
To render a view with a layout, model, ViewData, or ViewBag, call `RenderAsync()` on the `RazorTemplateEngine` static class.

### RenderAsync() method
```csharp
using Razor.Templating.Core;

var model = new ExampleModel()
{
    PlainText = "This text is rendered from Razor Views using Razor.Templating.Core",
    HtmlContent = "<em>You can use it to generate email content, report generation and so on</em>"
};

// Both ViewBag and ViewData should be added to the same dictionary. 
var viewDataOrViewBag = new Dictionary<string, object>();
// ViewData is the same as in MVC.
viewDataOrViewBag["Value1"] = "1";

// ViewBag.Value2 can be written as shown below. There's no change in how it's accessed in a .cshtml file.
viewDataOrViewBag["Value2"] = "2";

var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewDataOrViewBag);
```
Before using this code, see this article for a sample implementation: https://medium.com/@soundaranbu/render-razor-view-cshtml-to-string-in-net-core-7d125f32c79


## Render a View Without a Layout
If you need to render a view without a layout, use the `RenderPartialAsync()` method.

### RenderPartialAsync() method
```cs
var html = await RazorTemplateEngine.RenderPartialAsync("/Views/ExampleView.cshtml", model, viewDataOrViewBag);
```

## Render Views Without Throwing Exceptions
There are `TryRenderAsync()` and `TryRenderPartialAsync()` methods which do not throw an exception if the view doesn't exist. Instead, they return a tuple indicating whether the view exists and the rendered string.

### TryRenderAsync() method
```cs
var (viewExists, renderedView) = await engine.TryRenderAsync("~/Views/Feature/ExampleViewWithoutViewModel.cshtml");
```

### TryRenderPartialAsync() method
```cs
var (viewExists, renderedView) = await engine.TryRenderPartialAsync("~/Views/_ExamplePartialView.cshtml", model);
```


## Razor Views in Library
You can organize Razor view files (.cshtml) in a separate shared Razor Class Library (RCL). See a sample library [here](https://github.com/soundaranbu/Razor.Templating.Core/tree/main/examples/RazorTemplates).

The Razor Class Library's `.csproj` file should look like the example below. The `AddRazorSupportForMvc` property is required.

Also, the RCL should be referenced by the main project or by any project that invokes rendering methods such as `RazorTemplateEngine.RenderAsync()`.
```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFrameworks>net6.0</TargetFrameworks>
    <AddRazorSupportForMvc>true</AddRazorSupportForMvc>
  </PropertyGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
</Project>
```

## Dependency Injection
Dependencies can be injected directly into views using `@inject` in a `.cshtml` file. See a sample application [here](https://github.com/soundaranbu/RazorTemplating/tree/main/examples/Mvc)

In ASP.NET Core, register dependencies as shown below in `Program.cs`
```csharp
...
builder.Services.AddTransient<ExampleService>();
// Add this after registering all other dependencies
builder.Services.AddRazorTemplating();
```
or in console or other applications, add as below
```csharp
using Microsoft.Extensions.DependencyInjection;

// Add dependencies to the service collection
var services = new ServiceCollection();
services.AddTransient<ExampleService>();
// Add RazorTemplating after registering all dependencies
// This is important for the Razor template engine to find injected services.
services.AddRazorTemplating(); 
```

Once the dependencies are registered, we can use either one of these ways:

### Using `RazorTemplateEngine` static class
```cs
var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewServiceInjection.cshtml");
```

### Using `IRazorTemplateEngine`
- Instead of using the `RazorTemplateEngine` static class, you can use the `IRazorTemplateEngine` interface and inject it directly into your class constructor.

```cs
public class MyService {
    private readonly IRazorTemplateEngine _razorTemplateEngine;

    public MyService (IRazorTemplateEngine razorTemplateEngine)
    {
        _razorTemplateEngine = razorTemplateEngine;
    }

    public async Task Index()
    {
        var renderedView = await _razorTemplateEngine.RenderAsync("/Views/Home/Index.cshtml");
        // do something with renderedView
    }
}
```

## Note:
- Please ensure that view paths are unique across all shared template projects.

## Sample Applications
 Please find the sample applications [here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples) 
 
## Support
If you find this helpful, consider supporting development by buying a coffee. Thanks!

[![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86&style=for-the-badge&logo=appveyor)](https://github.com/sponsors/soundaranbu)

#### References:
Thanks to all the great articles and projects that helped bring this library to life!
- https://github.com/Andy9FromSpace/razor-renderer-core
- https://github.com/aspnet/Entropy/tree/master/samples/Mvc.RenderViewToString
- https://www.frakkingsweet.com/razor-template-rendering/
- https://github.com/veccsolutions/RenderRazorConsole
- https://emilol.com/razor-mailer/
- https://codeopinion.com/using-razor-in-a-console-application/
