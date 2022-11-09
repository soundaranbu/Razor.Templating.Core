using ExampleRazorTemplatesLibrary.Models;
using ExampleRazorTemplatesLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class RazorPartialViewToStringRendererTest
    {
        [Fact]
        public async Task RenderPartialView_WithOutLayout_WithModel()
        {
            // Arrange
            var viewData = new Dictionary<string, object>();
            var model = "This is a string from a model";

            // Act
            var html = await RazorTemplateEngine.RenderPartialAsync("~/Views/_examplePartialView.cshtml", model, null);

            // Expected
            var htmlContent = @"<h1>This is a partial view</h1>
<p>This is from a model:
    <strong>This is a string from a model</strong>
</p>";

            // Assert
            Assert.NotNull(html);
            Assert.Equal(htmlContent, html);
        }

        [Fact]
        public async Task RenderPartialView_Without_ViewModel()
        {
            // Act
            var html = await RazorTemplateEngine.RenderPartialAsync("~/Views/_examplePartialView.cshtml");


            // Expected
            var htmlContent = @"<h1>This is a partial view</h1>
<p>This is from a model:
    <strong></strong>
</p>";

            // Assert
            Assert.NotNull(html);
            Assert.Contains(htmlContent, html);
        }
    }
}