# Razor Templating

Using RazorEngine for HTML templating has never been so easy like this.
This can be used is any type of dotnet core application
#Example:
```csharp
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

------
#### References:
- https://github.com/Andy9FromSpace/razor-renderer-core
- https://github.com/aspnet/Entropy/tree/master/samples/Mvc.RenderViewToString
- https://www.frakkingsweet.com/razor-template-rendering/
- https://github.com/veccsolutions/RenderRazorConsole
- https://emilol.com/razor-mailer/
- https://codeopinion.com/using-razor-in-a-console-application/