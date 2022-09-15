using AutoFixture;
using ExampleRazorTemplatesLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class RazorTemplateEngineStaticClassTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Null()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => RazorTemplateEngine.RenderAsync(null!));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Empty()
        {
            var actual =
                await Assert.ThrowsAsync<ArgumentNullException>(() => RazorTemplateEngine.RenderAsync(string.Empty));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Whitespace()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => RazorTemplateEngine.RenderAsync(" "));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Can_Render_Example_View_With_No_Model()
        {
            // Arrange

            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleView.cshtml");

            // Assert

            // if no model / view data / view bag passed, then gets rendered as empty string
            Assert.Contains("<div>Plain text: </div>", html);
            Assert.Contains("<div>Html content: </div>", html);
            Assert.Contains("<div>ViewBag data: </div>", html);
            Assert.Contains("<div>ViewData data: </div>", html);
        }

        [Fact]
        public async Task Can_Render_Example_View_With_Model_Only()
        {
            // Arrange
            var model = _fixture.Create<ExampleModel>();

            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleView.cshtml", model);

            // Assert
            Assert.Contains($"<div>Plain text: {model.PlainText}</div>", html);
            Assert.Contains($"<div>Html content: {model.HtmlContent}</div>", html);
            // if no view data / view bag passed, then gets rendered as empty string
            Assert.Contains("<div>ViewBag data: </div>", html);
            Assert.Contains("<div>ViewData data: </div>", html);
        }

        [Fact]
        public async Task Can_Render_Example_View_With_Model_And_View_Data()
        {
            // Arrange
            var model = _fixture.Create<ExampleModel>();

            var viewData = new Dictionary<string, object>();
            viewData["Value1"] = _fixture.Create<string>();
            viewData["Value2"] = _fixture.Create<string>();

            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleView.cshtml", model, viewData);

            // Assert
            Assert.Contains($"<div>Plain text: {model.PlainText}</div>", html);
            Assert.Contains($"<div>Html content: {model.HtmlContent}</div>", html);
            Assert.Contains($"<div>ViewBag data: {viewData["Value1"]}</div>", html);
            Assert.Contains($"<div>ViewData data: {viewData["Value2"]}</div>", html);
        }
    }
}