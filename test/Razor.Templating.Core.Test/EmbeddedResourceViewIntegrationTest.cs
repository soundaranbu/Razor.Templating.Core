using EmbeddedResourceTemplates;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class EmbeddedResourceViewIntegrationTest
    {
        [Fact]
        public async Task RenderAsync_Should_Render_Embedded_Resource_View()
        {
            // Arrange - ensure the embedded assembly is loaded
            _ = typeof(EmbeddedResourceProjectMarker).Assembly;

            // Act
            var html = await RazorTemplateEngine.RenderAsync("/Views/Embedded/HelloWorld.cshtml");

            // Assert
            Assert.Contains("Hello from embedded view", html);
        }
    }
}
