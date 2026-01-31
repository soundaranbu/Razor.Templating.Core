using EmbeddedRazorTemplates;
using EmbeddedResourceTemplates;
using ExampleRazorTemplatesLibrary;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Razor.Templating.Core.Test;

public class EmbeddedResourceViewTests
{
    [Fact]
    public async Task RenderEmbeddedResourceView_FromRazorApp_Works()
    {
        // Arrange
        var services = new ServiceCollection();

        // Ensure the assembly hosting embedded views is loaded
        _ = typeof(EmbeddedResourceProjectMarker).Assembly;

        services.AddRazorTemplating();

        var plainText = "Hello from embedded resource in a razor web app!";
        var htmlContent = "<strong>This is bold text from the Razor web app model.</strong>";

        var model = new EmbeddedResourceTemplates.Models.ExampleModel()
        {
            PlainText = plainText,
            HtmlContent = htmlContent
        };

        var currentTime = DateTime.Now.ToString("u");
        var viewData = new Dictionary<string, object>();
        viewData["Value1"] = $"Loading at {currentTime}";
        viewData["Value2"] = "from caller ViewData";

        // Act
        var html = await RazorTemplateEngine.RenderAsync("/Views/Embedded/EmbeddedResourceView.cshtml", model, viewData);

        // Assert
        Assert.Contains($"<div>Plain text: {plainText}</div>", html);
        Assert.Contains($"<div>ViewBag data: Loading at {currentTime}</div>", html);
        Assert.Contains("<div>ViewData data: from caller ViewData</div>", html);
        Assert.Contains($"<div>Partial: {htmlContent}</div>", html);
    }

    [Fact]
    public async Task RenderEmbeddedResourceView_FromStandardClassLibrary_Works()
    {
        // Arrange
        var services = new ServiceCollection();

        // Ensure the assembly hosting embedded views is loaded
        _ = typeof(EmbeddedRazorTemplatesProjectMarker).Assembly;

        services.AddRazorTemplating();


        var plainText = "Hello from embedded resource in a class library!";
        var htmlContent = "<strong>This is bold text from the class library model.</strong>";

        var model = new EmbeddedRazorTemplates.Models.ExampleModel()
        {
            PlainText = plainText,
            HtmlContent = htmlContent
        };

        var currentTime = DateTime.Now.ToString("u");
        var viewData = new Dictionary<string, object>();
        viewData["Value1"] = $"Loading at {currentTime}";
        viewData["Value2"] = "from caller ViewData";

        // Act
        var html = await RazorTemplateEngine.RenderAsync("/RazorViews/EmbeddedResourceView.cshtml", model, viewData);

        // Assert
        Assert.Contains($"<div>Plain text: {plainText}</div>", html);
        Assert.Contains($"<div>ViewBag data: Loading at {currentTime}</div>", html);
        Assert.Contains("<div>ViewData data: from caller ViewData</div>", html);
        Assert.Contains($"<div>Partial: {htmlContent}</div>", html);

    }

    [Fact]
    public async Task RenderEmbeddedResourceView_FromRazorClassLibrary_Works()
    {
        // Arrange
        var services = new ServiceCollection();

        // Ensure the assembly hosting embedded views is loaded
        _ = typeof(RazorTemplatesLibraryProjectMarker).Assembly;

        services.AddRazorTemplating();

        var plainText = "Hello from embedded resource in an RCL!";
        var htmlContent = "<strong>This is bold text from the RCL model.</strong>";

        var model = new ExampleRazorTemplatesLibrary.Models.ExampleModel()
        {
            PlainText = plainText,
            HtmlContent = htmlContent
        };

        var currentTime = DateTime.Now.ToString("u");
        var viewData = new Dictionary<string, object>();
        viewData["Value1"] = $"Loading at {currentTime}";
        viewData["Value2"] = "from caller ViewData";

        // Act
        var html = await RazorTemplateEngine.RenderAsync("/Views/Embedded/ExampleView2.cshtml", model, viewData);

        // Assert
        Assert.Contains($"<div>Plain text: {plainText}</div>", html);
        Assert.Contains($"<div>ViewBag data: Loading at {currentTime}</div>", html);
        Assert.Contains("<div>ViewData data: from caller ViewData</div>", html);
        Assert.Contains($"<div>Html content: {htmlContent}</div>", html);

    }
}
