using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;
using EmbeddedResourceTemplates;
using ExampleRazorTemplatesLibrary;
using Razor.Templating.Core;

var config = ManualConfig.CreateMinimumViable()
    .AddExporter(MarkdownExporter.GitHub)
    .WithArtifactsPath(".");

BenchmarkRunner.Run<RazorTemplatingCoreBenchMark>(config);

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class RazorTemplatingCoreBenchMark
{
    [GlobalSetup]
    public async Task GlobalSetup()
    {
        // Ensure both assemblies are loaded before RazorTemplateEngine initializes
        _ = typeof(EmbeddedResourceProjectMarker).Assembly;
        _ = typeof(RazorTemplatesLibraryProjectMarker).Assembly;

        // Warm up the engine by rendering both views once
        // This ensures all file providers (including embedded) are registered
        var rclModel = BuildRazorTemplateModel();
        var embeddedModel = BuildEmbeddedResourceModel();

        var viewData = new Dictionary<string, object>
        {
            ["Value1"] = "1",
            ["Value2"] = "2"
        };

        await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", rclModel, viewData);
        await RazorTemplateEngine.RenderAsync("/Views/Embedded/EmbeddedResourceView.cshtml", embeddedModel, viewData);
    }

    [Benchmark]
    public async Task RenderViewWithModelAsync()
    {
        var model = BuildRazorTemplateModel();
        var viewData = new Dictionary<string, object>();
        viewData["Value1"] = "1";
        viewData["Value2"] = "2";

        var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);
    }

    [Benchmark]
    public async Task RenderEmbeddedRclViewWithModelAsync()
    {
        var model = BuildRazorTemplateModel();
        var viewData = new Dictionary<string, object>();
        viewData["Value1"] = "1";
        viewData["Value2"] = "2";

        var html = await RazorTemplateEngine.RenderAsync("/Views/Embedded/ExampleView2.cshtml", model, viewData);
    }


    [Benchmark]
    public async Task RenderEmbeddedResourceViewWithModelAsync()
    {
        var model = BuildEmbeddedResourceModel();
        var viewData = new Dictionary<string, object>();
        viewData["Value1"] = "1";
        viewData["Value2"] = "2";

        var html = await RazorTemplateEngine.RenderAsync("/Views/Embedded/EmbeddedResourceView.cshtml", model, viewData);
    }


    private ExampleRazorTemplatesLibrary.Models.ExampleModel BuildRazorTemplateModel()
    {
        return new ExampleRazorTemplatesLibrary.Models.ExampleModel()
        {
            PlainText = "Some text",
            HtmlContent = "<em>Some emphasized text</em>"
        };
    }


    private EmbeddedResourceTemplates.Models.ExampleModel BuildEmbeddedResourceModel()
    {
        return new EmbeddedResourceTemplates.Models.ExampleModel()
        {
            PlainText = "Some text",
            HtmlContent = "<em>Some emphasized text</em>"
        };
    }

}