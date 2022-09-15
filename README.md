<img src="https://raw.githubusercontent.com/soundaranbu/RazorTemplating/master/src/Razor.Templating.Core/assets/icon.png" width="70" height="70" /> 

# Razor.Templating.Core

![Build+Test](https://github.com/soundaranbu/RazorTemplating/workflows/Build+Test/badge.svg?branch=master) ![Nuget](https://img.shields.io/nuget/v/Razor.Templating.Core) ![Downloads](https://img.shields.io/nuget/dt/Razor.Templating.Core) 
![Coverage](https://raw.githubusercontent.com/soundaranbu/RazorTemplating/master/test/Razor.Templating.Core.Test/Reports/badge_shieldsio_linecoverage_green.svg)

Using [Razor](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-3.1) for HTML templating was never been so easy like this. Render your .cshtml files into string easily using this library.

This project makes use of [Razor SDK](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/sdk?view=aspnetcore-3.1) for precompiling the views.

## Supported Application Types

|                  | .NET Core 3.0 | .NET Core 3.1 | .NET 5  | .NET 6  |
|------------------|---------------|---------------|---------|---------|
| Preferred Version|   v1.6.0      |     v1.6.0    |  v1.6.0 | v1.7.1  | 
| Console          | &check;       | &check;       | &check; | &check; |
| Api              | &check;       | &check;       | &check; | &check; |
| Mvc              | &check;       | &check;       | &check; | &check; |
| Worker Service   | &check;       | &check;       | &check; | &check; |
| WPF              | &check;       | &check;       | &check; | &check; |
| WinForms         | &check;       | &check;       | &check; | &check; |
| Azure Functions  | &check;       | &check;       | &check; | &check; |


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
| Dependency Injection into Views   | &check;       |
| @Url.ContentUrl**                 | &cross;       |
| @Url.RouteUrl**                   | &cross;       |

**Contributors are welcome who can help to enable these unsupported features.

## Applications
- Email Templating
- Report Generation & more

## Installing Nuget Package
This library is available as [Nuget package](https://www.nuget.org/packages/Razor.Templating.Core/)

##### Using .NET CLI
```bash
dotnet add package Razor.Templating.Core
```
##### Using Package Reference .csproj
```bash
<PackageReference Include="Razor.Templating.Core" Version="1.7.1" />
```

## Simple Usage:
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

## Razor Views in Library
 Razor view files(.cshtml) can be organized in a separate shared Razor Class Libary(RCL). Sample library can be found [here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples/Templates/ExampleAppRazorTemplates)

 The Razor Class Library's `.csproj` file should look something like below. Whereas, `AddRazorSupportForMvc` property is important.

 Also, RCL should be referenced to the main project or where the `RazorTemplateEngine.RenderAsync` method is invoked.
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

## Dependency Injection [Since `v1.4.0`]
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
var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewServiceInjection.cshtml");
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

## Sample Applications
 Please find the sample applications [here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples) 
 
## Support
If you find this helpful, consider supporting the development of this library by sponsoring one or more coffee ;) Thanks!

[![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/soundaranbu)
## Note:
- Please ensure that the views path is always unique among all the shared template projects.

#### References:
Thanks to all the great articles and projects which helped to bring this library out!
- https://github.com/Andy9FromSpace/razor-renderer-core
- https://github.com/aspnet/Entropy/tree/master/samples/Mvc.RenderViewToString
- https://www.frakkingsweet.com/razor-template-rendering/
- https://github.com/veccsolutions/RenderRazorConsole
- https://emilol.com/razor-mailer/
- https://codeopinion.com/using-razor-in-a-console-application/
