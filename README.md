# Razor Templating <img src="https://raw.githubusercontent.com/soundaranbu/RazorTemplating/master/src/Razor.Templating.Core/assets/icon.png" width="35" height="35" />

![Build+Test](https://github.com/soundaranbu/RazorTemplating/workflows/Build+Test/badge.svg?branch=master) ![Nuget](https://img.shields.io/nuget/v/Razor.Templating.Core)

Using [Razor](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-3.1) for HTML templating was never been so easy like this.
  - Works for **.NET Core 3.0, 3.1**
  - Works in Console Application, ASP.NET Core Application Independently

This project makes use of [Razor SDK](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/sdk?view=aspnetcore-3.1) for precompiling the views.

# Applications
- Email Templating
- Report Generation & so on

# Simple Usage:
```csharp
using Razor.Templating.Core;

var model = new ExampleModel()
{
    PlainText = "Some text",
    HtmlContent = "<em>Some emphasized text</em>"
};

var viewData = new Dictionary<string, object>();
viewData["Value1"] = "1";
viewData["Value2"] = "2";

var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
```

# Razor Views in Library
 Razor view files(.cshtml) can be organized in a separate shared libary. Sample library can be found [here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples/ExampleAppRazorTemplates)
 
# Sample Applications
 Please find the sample applications [here](https://github.com/soundaranbu/RazorTemplating/tree/master/examples) 
 
# Note:
- Please ensure that the views path is always unique among all the shared template projects.

#### References:
Thanks to all the great articles which helped to bring this library out!
- https://github.com/Andy9FromSpace/razor-renderer-core
- https://github.com/aspnet/Entropy/tree/master/samples/Mvc.RenderViewToString
- https://www.frakkingsweet.com/razor-template-rendering/
- https://github.com/veccsolutions/RenderRazorConsole
- https://emilol.com/razor-mailer/
- https://codeopinion.com/using-razor-in-a-console-application/
