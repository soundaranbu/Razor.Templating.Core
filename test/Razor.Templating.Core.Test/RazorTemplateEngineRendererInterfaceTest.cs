using ExampleRazorTemplatesLibrary.Models;
using ExampleRazorTemplatesLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class RazorTemplateEngineRendererInterfaceTest
    {
        [Fact]
        public async Task RenderView_WithModelAndViewData_WithPartialView()
        {
            //Optionally call this to create cache of the renderer
            //Otherwise, render time will be more than usual on first time only

            // Arrange
            var model = new ExampleModel()
            {
                PlainText = "Lorem Ipsium",
                HtmlContent = "<em>Lorem Ipsium</em>"
            };

            var viewData = new Dictionary<string, object>();
            viewData["Value1"] = "1";
            viewData["Value2"] = "2";

            // Act
            var engine = GetRazorTemplateEngine();
            var html = await engine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);

            // Assert
            Assert.Contains("<div>Plain text: Lorem Ipsium</div>", html);
            Assert.Contains("<div>ViewBag data: 1</div>", html);
            Assert.Contains("<div>ViewData data: 2</div>", html);
            Assert.Contains("<div>Html content: <em>Lorem Ipsium</em></div>", html);
        }

        [Fact]
        public async Task RenderView_WithLayout_WithViewData()
        {
            // Arrange
            var viewData = new Dictionary<string, object>();
            viewData["Title"] = "This is Title";

            // Act
            var engine = GetRazorTemplateEngine();
            var html = await engine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml", null, viewData);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("This is the view content", html);
            Assert.Contains("This is Title", html);
        }

        [Fact]
        public async Task RenderView_WithLayout_WithoutData()
        {
            // Act
            var engine = GetRazorTemplateEngine();
            var html = await engine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml");

            // Assert
            Assert.NotNull(html);
            Assert.Contains("This is the view content", html);
        }

        [Fact]
        public async Task RenderView_Without_ViewModel()
        {
            // Act
            var engine = GetRazorTemplateEngine();
            var html = await engine.RenderAsync("~/Views/Feature/ExampleViewWithoutViewModel.cshtml");

            // Assert
            Assert.NotNull(html);
            Assert.Contains("<div>Hi I'm example view without any viewmodel or view data</div>", html);
        }

        [Fact]
        public async Task RenderPartialView_WithModel()
        {
            // Arrange
            var model = new ExampleModel()
            {
                PlainText = "Lorem Ipsium",
                HtmlContent = "<em>Lorem Ipsium</em>"
            };

            // Act
            var engine = GetRazorTemplateEngine();
            var html = await engine.RenderPartialAsync("~/Views/_ExamplePartialView.cshtml", model);

            // Assert
            Assert.NotNull(html);

            var expected = "\r\n<div>Partial view</div>\r\n<div>Html content: <em>Lorem Ipsium</em></div>\r\n";
            Assert.Equal(expected, html);
        }

        [Fact]
        public async Task RenderView_WithServiceInjection()
        {
            // Arrange
            var model = new ExampleModel()
            {
                PlainText = "Lorem Ipsium",
                HtmlContent = "<em>Lorem Ipsium</em>"
            };

            // Add dependencies to the service collection and add razor templating to the collection
            var engine = GetRazorTemplateEngine(services =>
            {
                services.AddTransient<ExampleService>();
            });

            // Act
            var html = await engine.RenderAsync("~/Views/ExampleViewServiceInjection.cshtml", model);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("Injected Service Data: Some Random Value - ", html);
        }

        [Fact]
        public async Task RenderView_WithModel_WithViewImport()
        {
            // Arrange
            var model = new ExampleModel()
            {
                PlainText = "Lorem Ipsium",
                HtmlContent = "<em>Lorem Ipsium</em>"
            };

            // Act
            var engine = GetRazorTemplateEngine();
            var html = await engine.RenderAsync("~/Views/ExampleViewUsingViewImports.cshtml", model);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("Plain text: Lorem Ipsium", html);
        }

        [Fact]
        public async Task RenderView_WithTagHelpers()
        {
            // Act
            var engine = GetRazorTemplateEngine();
            var html = await engine.RenderAsync("~/Views/ExampleViewWithTagHelpers.cshtml");

            // Assert
            Assert.NotNull(html);
            Assert.Contains(@"<label class=""caption"" for=""FirstName"">First Name:</label>", html);
            Assert.Contains("<form method=\"post\" class=\"form-horizontal\" role=\"form\" action=\"/Account/Login\">", html);
            Assert.Contains("<a href=\"/Speaker/Index\">All Speakers</a>", html);
        }

        [Fact]
        public async Task RenderView_WithViewComponent()
        {
            // Act
            var engine = GetRazorTemplateEngine();
            var html = await engine.RenderAsync("~/Views/ExampleViewWithViewComponent.cshtml");

            // Assert
            Assert.NotNull(html);
            Assert.Contains(@"Example View Component!", html);
        }

        [Fact]
        public async Task RenderInvalidView_Should_ThrowError()
        {
            try
            {
                var engine = GetRazorTemplateEngine();
                var html = await engine.RenderAsync("/Views/SomeInvalidView.cshtml");
            }
            catch (System.Exception e)
            {
                Assert.True(e is InvalidOperationException);
                Assert.Contains("Unable to find view '/Views/SomeInvalidView.cshtml'.", e.Message);
            }
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Null()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => GetRazorTemplateEngine().RenderAsync(null!));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Empty()
        {
            var actual =
                await Assert.ThrowsAsync<ArgumentNullException>(() => GetRazorTemplateEngine().RenderAsync(string.Empty));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Whitespace()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => GetRazorTemplateEngine().RenderAsync(" "));
            Assert.Equal("viewName", actual.ParamName);
        }

        /// <summary>
        /// Gets an instance of <see cref="IRazorTemplateEngine"/>.
        /// </summary>
        /// <param name="configure">Optional parameter to configure more services before creating instance.</param>
        private static IRazorTemplateEngine GetRazorTemplateEngine(Action<IServiceCollection> configure = null)
        {
            var services = new ServiceCollection();
            services.AddRazorTemplating();
            configure?.Invoke(services);
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IRazorTemplateEngine>();
        }

        [Fact]
        public async Task TryRenderViewAsync_Should_Return_False_For_InvalidPaths()
        {
            var engine = GetRazorTemplateEngine();
            var result = await engine.TryRenderAsync("/Views/SomeInvalidView.cshtml");

            Assert.False(result.ViewExists);
            Assert.Empty(result.RenderedView);

            result = await engine.TryRenderPartialAsync("/Views/SomeInvalidView.cshtml");

            Assert.False(result.ViewExists);
            Assert.Empty(result.RenderedView);
        }
    }
}
