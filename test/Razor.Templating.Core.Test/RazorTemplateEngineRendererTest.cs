using AutoFixture;
using ExampleRazorTemplatesLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class RazorTemplateEngineRendererTest
    {
        /// <summary>
        /// Subject Under Test
        /// </summary>
        private readonly RazorTemplateEngineRenderer _sut;
        private readonly Fixture _fixture = new Fixture();

        public RazorTemplateEngineRendererTest()
        {
            var services = new ServiceCollection();
            services.AddRazorTemplating(opts => opts.UseStaticRazorTemplateEngine = false);
            var serviceProvider = services.BuildServiceProvider();

            _sut = new RazorTemplateEngineRenderer(serviceProvider);
        }

        [Fact]
        public void Throws_ArgumentNullException_If_ServiceProvider_Is_Null()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new RazorTemplateEngineRenderer(null!));
            Assert.Equal("serviceProvider", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Null()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RenderAsync(null!));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Empty()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RenderAsync(string.Empty));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Whitespace()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RenderAsync(" "));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Can_Render_Example_View_With_No_Model()
        {
            // Arrange

            // Act
            var html = await _sut.RenderAsync("~/Views/ExampleView.cshtml");

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
            var html = await _sut.RenderAsync("~/Views/ExampleView.cshtml", model);

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
            var html = await _sut.RenderAsync("~/Views/ExampleView.cshtml", model, viewData);

            // Assert
            Assert.Contains($"<div>Plain text: {model.PlainText}</div>", html);
            Assert.Contains($"<div>Html content: {model.HtmlContent}</div>", html);
            Assert.Contains($"<div>ViewBag data: {viewData["Value1"]}</div>", html);
            Assert.Contains($"<div>ViewData data: {viewData["Value2"]}</div>", html);
        }
    }
}
