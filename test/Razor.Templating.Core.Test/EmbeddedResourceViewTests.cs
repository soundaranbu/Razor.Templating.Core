using EmbeddedRazorTemplates;
using EmbeddedResourceTemplates;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class EmbeddedResourceViewTests
    {
        [Fact]
        public async Task RenderView_FromEmbeddedResourceRazorApp_Works()
        {
            // Arrange
            var services = new ServiceCollection();

            // Ensure the assembly hosting embedded views is loaded
            _ = typeof(EmbeddedResourceProjectMarker).Assembly;

            services.AddRazorTemplating();

            var model = new EmbeddedResourceTemplates.Models.ExampleModel()
            {
                PlainText = "Hello from embedded resource in a razor web app!",
                HtmlContent = "<strong>This is bold text from the model.</strong>"
            };

            var currentTime = DateTime.Now.ToString("u");
            var viewData = new Dictionary<string, object>();
            viewData["Value1"] = $"Loading at {currentTime}";
            viewData["Value2"] = "from caller ViewData";

            // Act
            var html = await RazorTemplateEngine.RenderAsync("/Views/Embedded/EmbeddedResourceView.cshtml", model, viewData);

            // Assert
            Assert.Contains("<div>Plain text: Hello from embedded resource in a razor web app!</div>", html);
            Assert.Contains($"<div>ViewBag data: Loading at {currentTime}</div>", html);
            Assert.Contains("<div>ViewData data: from caller ViewData</div>", html);
            Assert.Contains("<div>Partial: &lt;strong&gt;This is bold text from the model.&lt;/strong&gt;</div>", html);

        }

        [Fact]
        public async Task RenderView_FromEmbeddedResourceClassLibrary_Works()
        {
            // Arrange
            var services = new ServiceCollection();

            // Ensure the assembly hosting embedded views is loaded
            _ = typeof(EmbeddedRazorTemplatesProjectMarker).Assembly;

            services.AddRazorTemplating();

            var model = new EmbeddedRazorTemplates.Models.ExampleModel()
            {
                PlainText = "Hello from embedded resource in a class library!",
                HtmlContent = "<strong>This is bold text from the model.</strong>"
            };

            var currentTime = DateTime.Now.ToString("u");
            var viewData = new Dictionary<string, object>();
            viewData["Value1"] = $"Loading at {currentTime}";
            viewData["Value2"] = "from caller ViewData";

            // Act
            var html = await RazorTemplateEngine.RenderAsync("/RazorViews/EmbeddedResourceView.cshtml", model, viewData);

            // Assert
            Assert.Contains("<div>Plain text: Hello from embedded resource in a class library!</div>", html);
            Assert.Contains($"<div>ViewBag data: Loading at {currentTime}</div>", html);
            Assert.Contains("<div>ViewData data: from caller ViewData</div>", html);
            Assert.Contains("<div>Partial: &lt;strong&gt;This is bold text from the model.&lt;/strong&gt;</div>", html);

        }
    }
}
