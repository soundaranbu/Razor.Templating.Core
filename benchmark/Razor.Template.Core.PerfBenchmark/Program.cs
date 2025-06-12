using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using ExampleRazorTemplatesLibrary.Models;
using Razor.Templating.Core;

var config = ManualConfig.CreateMinimumViable()
    .AddExporter(MarkdownExporter.GitHub)
    .AddAnalyser(EnvironmentAnalyser.Default)
    .WithArtifactsPath(".");

BenchmarkRunner.Run<RazorTemplatingCoreBenchMark>(config);

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class RazorTemplatingCoreBenchMark
{
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