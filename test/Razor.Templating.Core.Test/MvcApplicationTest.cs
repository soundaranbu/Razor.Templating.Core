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
}
