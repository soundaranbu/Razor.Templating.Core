#:project ../RazorTemplates/RazorTemplatesLibrary.csproj
#:project ../../src/Razor.Templating.Core/Razor.Templating.Core.csproj
#:property PublishAot=false

using ExampleRazorTemplatesLibrary.Models;
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
Console.Write(html);