<img src="https://raw.githubusercontent.com/soundaranbu/RazorTemplating/master/src/Razor.Templating.Core/assets/icon.png" width="70" height="70" /> 

# Razor Templating 

![Build+Test](https://github.com/soundaranbu/RazorTemplating/workflows/Build+Test/badge.svg?branch=master) ![Nuget](https://img.shields.io/nuget/v/Razor.Templating.Core) ![Downloads](https://img.shields.io/nuget/dt/Razor.Templating.Core)

Using [Razor](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-3.1) for HTML templating was never been so easy like this.
  - Works for **.NET Core 3.0, 3.1**, .NET 5.0 (coming soon)
  - Works in **Console, Worker Service, ASP.NET Core, Desktop(WPF, WinForms in .NET Core) Applications** Independently

This project makes use of [Razor SDK](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/sdk?view=aspnetcore-3.1) for precompiling the views.

## Supported Application Types

App Type\.NET Versions | .NET Core 3.0 | .NET Core 3.1 | .NET 5

|App Type/.NET Versions|.NET Core 3.0|.NET Core 3.1|.NET 5
|---|---|---|
|Console|&check;|&check;|&check;|
|Api|&check;|&check;|&check;|
|Mvc|&check;|&check;|&check;|
|Worker Service|&check;|&check;|&check;|
|WinForms|&check;|&check;|&check;|
|WCF|&check;|&check;|&check;|


## Applications
- Email Templating
- Report Generation & so on

## Installing Nuget Package
This library is available as [Nuget package](https://www.nuget.org/packages/Razor.Templating.Core/)

##### Using .NET CLI
```bash
dotnet add package Razor.Templating.Core
```
##### Using Package Reference .csproj
```bash
<PackageReference Include="Razor.Templating.Core" Version="1.2.1" />
```

## Simple Usage:
```csharp
using Razor.Templating.Core;

var model = new ExampleModel()
{
    PlainText = "This text is rendered from Razor Views using Razor.Templating.Core",
    HtmlContent = "<em>You can use it to generate email content, report generation and so on</em>"
};

var viewData = new Dictionary<string, object>();
viewData["Value1"] = "1";
viewData["Value2"] = "2";

var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
```
Before applying this code, follow this article for working implementation: https://medium.com/@soundaranbu/render-razor-view-cshtml-to-string-in-net-core-7d125f32c79

## Razor Views in Library
 Razor view files(.cshtml) can be organized in a separate shared libary. Sample library can be found [here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples/ExampleAppRazorTemplates)
 
## Sample Applications
 Please find the sample applications [here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples) 
 
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
