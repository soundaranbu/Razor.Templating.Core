using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Razor.Templating.Core.Test;
public class MvcApplicationTest
{
    private readonly WebApplicationFactory<Program> _factory;

    public MvcApplicationTest()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    [Fact]
    public async Task HomePage_Should_RenderViewByName()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Home/Index");

        // Assert
        response.EnsureSuccessStatusCode();

        var pageHtml = await response.Content.ReadAsStringAsync();

        Assert.Contains(@"<li><a href=""/home/Index"">Render content by using only the view name instead of path</a></li>", pageHtml);
        Assert.Contains(@"<h1>This is a partial page</h1>", pageHtml);
    }

    [Theory]
    [InlineData("fr", "<div>Content from Index.fr.cshtml</div>")]
    [InlineData("en-US", "<div>Content from Index.cshtml</div>")]
    public async Task RequestWithCulture_Should_RenderLocalizedView(string culture, string expectedContent)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/Home/Index?culture={culture}");

        // Assert
        response.EnsureSuccessStatusCode();

        var pageHtml = await response.Content.ReadAsStringAsync();

        Assert.Contains(expectedContent, pageHtml);
    }
}
