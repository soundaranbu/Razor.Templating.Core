using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ExampleRazorTemplatesLibrary.Models;
using Razor.Templating.Core;

BenchmarkRunner.Run<RazorTemplateBenchMark>();

[MemoryDiagnoser]
public class RazorTemplateBenchMark
{

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public async Task RenderViewWithModelAsync()
    {
        var model = new ExampleModel()
        {
            PlainText = "Some text",
            HtmlContent = "<em>Some emphasized text</em>"
        };
        var viewData = new Dictionary<string, object>();
        viewData["Value1"] = "1";
        viewData["Value2"] = "2";

        var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
    }
}