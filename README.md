# Razor.Templating.Core

![Build+Test](https://github.com/soundaranbu/RazorTemplating/workflows/Build+Test/badge.svg?branch=main) ![Nuget](https://img.shields.io/nuget/v/Razor.Templating.Core) ![Downloads](https://img.shields.io/nuget/dt/Razor.Templating.Core) 
![Coverage](https://raw.githubusercontent.com/soundaranbu/RazorTemplating/main/test/Razor.Templating.Core.Test/Reports/badge_shieldsio_linecoverage_green.svg)

Using [Razor](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-3.1) for HTML templating was never been so easy like this. Render your .cshtml files into string easily using this library.

This project makes use of [Razor SDK](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/sdk?view=aspnetcore-3.1) for precompiling the views.

## Supported Application Types

|                  | .NET Core 3 to .NET 5  | .NET 6 & Above|
|------------------|------------------------|---------------|
| Preferred Version|   v1.6.0               | 2.1.0    | 
| Console          | &check;                | &check;       |
| Api              | &check;                | &check;       |
| Mvc              | &check;                | &check;       |
| Worker Service   | &check;                | &check;       |
| WPF              | &check;                | &check;       |
| WinForms         | &check;                | &check;       |
| Azure Functions  | &check;                | &check;       |


## Supported View Features
| MVC Razor View Features           |               |
|---------------------------------- |---------------|
| ViewModel                         | &check;       |
| ViewBag                           | &check;       |
| ViewData                          | &check;       |
| Layouts                           | &check;       |
| ViewStarts                        | &check;       |
| ViewImports                       | &check;       |
| Partial Views                     | &check;       |
| Tag Helpers                       | &check;       |
| View Components (.NET 5 +)        | &check;       |
| View Localization (Only MVC)      | &check;       |
| Dependency Injection into Views   | &check;       |
| @Url.ContentUrl**                 | &cross;       |
| @Url.RouteUrl**                   | &cross;       |

**Contributors are welcome who can help to enable these unsupported features.

## Applications
- Email Templating
- Report Generation & more

## Installing Nuget Package
This library is available as [Nuget package](https://www.nuget.org/packages/Razor.Templating.Core/)

### Using .NET CLI
```bash
dotnet add package Razor.Templating.Core
```
### Using Package Reference .csproj
```bash
<PackageReference Include="Razor.Templating.Core" Version="2.1.0" />
```

## Usage - Render View With Layout
To render a view with layout, model, viewdata or viewbag, then call the `RenderAsync()` on the `RazorTemplateEngine` static class

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
// ViewData is same as mvc
viewDataOrViewBag["Value1"] = "1";

// ViewBag.Value2 can be written as below. There's no change on how it's accessed in .cshtml file
viewDataOrViewBag["Value2"] = "2";

var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewDataOrViewBag);
```
Before applying this code, follow this article for sample implementation: https://medium.com/@soundaranbu/render-razor-view-cshtml-to-string-in-net-core-7d125f32c79


## Render View Without Layout
In case if there's a need to render a view without layout, use `RenderParitalAsync()` method.

### RenderPartialAsync() method
```cs
var html = await RazorTemplateEngine.RenderPartialAsync("/Views/ExampleView.cshtml", model, viewDataOrViewBag);
```

## Render Views Without Throwing Exception
There are `TryRenderAsync()` and `TryRenderPartialAsync` methods which will not throw exception if the view doesn't exist.
Instead they return a tuple to indicate whether the view exists and the rendered string.

### TryRenderAsync() method
```cs
var (viewExists, renderedView) = await engine.TryRenderAsync("~/Views/Feature/ExampleViewWithoutViewModel.cshtml");
```

### TryRenderPartialAsync() method
```cs
var (viewExists, renderedView) = await engine.TryRenderPartialAsync("~/Views/_ExamplePartialView.cshtml", model);
```


## Razor Views in Library
We can organize the Razor view files(.cshtml) in a separate shared Razor Class Libary(RCL). Please find a sample library [here](https://github.com/soundaranbu/Razor.Templating.Core/tree/main/examples/Templates/ExampleAppRazorTemplates)

 The Razor Class Library's `.csproj` file should look something like below. Whereas, `AddRazorSupportForMvc` property is mandatory.

 Also, RCL should be referenced by the main project or where any of the rendering methods like `RazorTemplateEngine.RenderAsync()` are invoked.
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
Dependencies can be injected directly into views using `@inject` in .csthml file. Refer [sample application here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples/Mvc)

In ASP.NET Core, add dependency like below in `Startup.cs -> ConfigureServices`
```csharp
...
services.AddTransient<ExampleService>();
//add after registering all the dependencies
services.AddRazorTemplating();
```
or in console or other applications, add as below
```csharp
using Microsoft.Extensions.DependencyInjection;

// Add dependencies to the service collection
var services = new ServiceCollection();
services.AddTransient<ExampleService>();
// Add RazorTemplating after registering all dependencies
// this is important for the razor template engine to find the injected services
services.AddRazorTemplating(); 
```

Once the dependencies are registered, we can use either one of these ways:

### Using `RazorTemplateEngine` static class
```cs
var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewServiceInjection.cshtml");
```

### Using `IRazorTemplateEngine`
- Instead of using the `RazorTemplateEngine` static class, it's also possible to use the `IRazorTemplateEngine` interface to inject dependency directly into the constructor.

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
 
## How to render razor views from absolute path
We can make use of ASP.NET Core's inbuilt [RazorRuntimeCompilation](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-compilation?view=aspnetcore-6.0&tabs=visual-studio) to render any .cshtml inside or outside of the project.

As of `v1.7.0+`, we can achieve this as below:
```csharp
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Razor.Templating.Core;

var services = new ServiceCollection();
services.AddMvcCore().AddRazorRuntimeCompilation();
services.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
{
    opts.FileProviders.Add(new PhysicalFileProvider(@"D:\PathToRazorViews")); // This will be the root path
});
services.AddRazorTemplating();

var html = await RazorTemplateEngine.RenderAsync("/Views/Home/Rcl.cshtml"); // relative path to the root
```

Please note this may become slightly better in the future versions of our library.

## Note:
- Please ensure that the views path is always unique among all the shared template projects.

## Sample Applications
 Please find the sample applications [here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples) 
 
## Support
If you find this helpful, consider supporting the development of this library by sponsoring one or more coffee ;) Thanks!

[![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86&style=for-the-badge&logo=appveyor)](https://github.com/sponsors/soundaranbu)

#### References:
Thanks to all the great articles and projects which helped to bring this library out!
- https://github.com/Andy9FromSpace/razor-renderer-core
- https://github.com/aspnet/Entropy/tree/master/samples/Mvc.RenderViewToString
- https://www.frakkingsweet.com/razor-template-rendering/
- https://github.com/veccsolutions/RenderRazorConsole
- https://emilol.com/razor-mailer/
- https://codeopinion.com/using-razor-in-a-console-application/
