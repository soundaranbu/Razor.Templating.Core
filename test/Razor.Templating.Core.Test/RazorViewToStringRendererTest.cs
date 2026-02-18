using ExampleRazorTemplatesLibrary.Models;
using ExampleRazorTemplatesLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class RazorViewToStringRendererTest
    {
        [Fact]
        public async Task RenderView_WithModelAndViewData_WithPartialView()
        {
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
            var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);

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
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml", null, viewData);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("This is the view content", html);
            Assert.Contains("This is Title", html);
        }

        [Fact]
        public async Task RenderView_WithLayout_WithoutData()
        {
            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml");

            // Assert
            Assert.NotNull(html);
            Assert.Contains("This is the view content", html);
        }

        [Fact]
        public async Task RenderView_Without_ViewModel()
        {
            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/Feature/ExampleViewWithoutViewModel.cshtml");

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
            var html = await RazorTemplateEngine.RenderPartialAsync("~/Views/_ExamplePartialView.cshtml", model);

            // Assert
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
            var services = new ServiceCollection();
            services.AddTransient<ExampleService>();
            // Add after registering all dependencies
            // this is important for the razor template engine to find the injected services
            services.AddRazorTemplating();
            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewServiceInjection.cshtml", model);

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
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewUsingViewImports.cshtml", model);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("Plain text: Lorem Ipsium", html);
        }

        [Fact]
        public async Task RenderView_WithTagHelpers()
        {
            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithTagHelpers.cshtml");

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
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithViewComponent.cshtml");

            // Assert
            Assert.NotNull(html);
            Assert.Contains(@"Example View Component!", html);
        }

        [Fact]
        public async Task RenderInvalidView_Should_ThrowError()
        {
            var actual = await Assert.ThrowsAnyAsync<InvalidOperationException>(() => RazorTemplateEngine.RenderAsync("/Views/SomeInvalidView.cshtml"));
            Assert.Contains("Unable to find view '/Views/SomeInvalidView.cshtml'.", actual.Message);

            actual = await Assert.ThrowsAsync<ViewNotFoundException>(() => RazorTemplateEngine.RenderAsync("/Views/SomeInvalidView.cshtml"));
            Assert.Contains("Unable to find view '/Views/SomeInvalidView.cshtml'.", actual.Message);
        }

        [Fact]
        public async Task RenderViewByNameOutsideMvcApplication_Should_ThrowError()
        {
            var actual = await Assert.ThrowsAnyAsync<InvalidOperationException>(() => RazorTemplateEngine.RenderAsync("Index"));
            Assert.Contains("Unable to find view 'Index'.", actual.Message);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Null()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => RazorTemplateEngine.RenderAsync(null!));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Empty()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => RazorTemplateEngine.RenderAsync(string.Empty));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Throws_ArgumentNullException_If_RenderAsync_When_ViewName_Is_Whitespace()
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => RazorTemplateEngine.RenderAsync(" "));
            Assert.Equal("viewName", actual.ParamName);
        }

        [Fact]
        public async Task Renders_CorrectLangStringFromResx_When_CultureInfoIsSet()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("es-ES");

            // Act
            var spanishHtml = await RazorTemplateEngine.RenderPartialAsync("~/Views/ExampleViewWithLocalization.cshtml");

            // Assert
            Assert.Contains("<h2>&#xA1;Hola, esto es una frase localizada!</h2>\r\n<p>Este texto proviene del archivo de recursos.</p>", spanishHtml);

            // Reset culture to English
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            // Act
            var englisHtml = await RazorTemplateEngine.RenderPartialAsync("~/Views/ExampleViewWithLocalization.cshtml");

            // Assert
            Assert.Contains("<h2>Hello, this is a localized phrase!</h2>\r\n<p>This text comes from the resource file</p>", englisHtml);
        }

        [Fact]
        public async Task RenderView_WhenHttpContextIsNull_UsesDefaultActionContext()
        {
            // Arrange
            var model = new ExampleModel()
            {
                PlainText = "Non-HTTP Context Test",
                HtmlContent = "<em>Test from background task</em>"
            };

            var services = new ServiceCollection();
            services.AddRazorTemplating();

            services.AddTransient<IHttpContextAccessor>(_ => new MockHttpContextAccessor(null));

            var serviceProvider = services.BuildServiceProvider();

            var engine = serviceProvider.GetRequiredService<IRazorTemplateEngine>();
            
            // Act
            var html = await engine.RenderAsync("~/Views/ExampleView.cshtml", model);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("<div>Plain text: Non-HTTP Context Test</div>", html);
            Assert.Contains("<div>Html content: <em>Test from background task</em></div>", html);
        }

        [Fact]
        public async Task RenderView_WhenEndpointIsNull_UsesDefaultActionContext()
        {
            // Arrange
            var model = new ExampleModel()
            {
                PlainText = "Null Endpoint Test",
                HtmlContent = "<em>Test with null endpoint</em>"
            };

            var services = new ServiceCollection();
            services.AddRazorTemplating();

            services.AddTransient<IHttpContextAccessor>(_ => new MockHttpContextAccessor(new DefaultHttpContext()));

            var serviceProvider = services.BuildServiceProvider();

            var engine = serviceProvider.GetRequiredService<IRazorTemplateEngine>();

            // Act
            var html = await engine.RenderAsync("~/Views/ExampleView.cshtml", model);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("<div>Plain text: Null Endpoint Test</div>", html);
            Assert.Contains("<div>Html content: <em>Test with null endpoint</em></div>", html);
        }
    }

    internal class MockHttpContextAccessor : IHttpContextAccessor
    {
        public MockHttpContextAccessor(HttpContext? httpContext)
        {
            HttpContext = httpContext;
        }

        public HttpContext? HttpContext { get; set; }
    }
}