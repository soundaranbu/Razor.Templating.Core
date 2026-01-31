using EmbeddedResourceTemplates;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class EmbeddedResourceViewTest
    {
        [Fact]
        public async Task RenderView_FromEmbeddedResource_Works()
        {
            // Arrange
            var services = new ServiceCollection();

            // Ensure the assembly hosting embedded views is loaded
            _ = typeof(EmbeddedResourceProjectMarker).Assembly;

            services.AddRazorTemplating();

            // Act
            var html = await RazorTemplateEngine.RenderAsync("/Views/Embedded/HelloWorld.cshtml");

            // Assert
            Assert.Contains("Hello from embedded view", html, StringComparison.OrdinalIgnoreCase);
        }
    }
}
