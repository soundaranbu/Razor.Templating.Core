using ExampleRazorTemplatesLibrary.Models;
using ExampleRazorTemplatesLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class RazorViewToStringRendererTest
    {
        [Fact]
        public async Task RenderView_WithModelAndViewData_WithPartialView()
        {
            //Optionally call this to create cache of the renderer
            //Otherwise, render time will be more than usual on first time only
            RazorTemplateEngine.Initialize();
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
            var html = await RazorTemplateEngine.RenderAsync<object>("~/Views/ExampleViewWithLayout.cshtml", null, viewData);

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
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExamplePartialView.cshtml", model);

            // Assert
            Assert.NotNull(html);
            Assert.Contains("Partial view", html);
            Assert.Contains("Html content: <em>Lorem Ipsium</em>", html);
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
            try
            {
                var html = await RazorTemplateEngine.RenderAsync("/Views/SomeInvalidView.cshtml");
            }
            catch (System.Exception e)
            {
                Assert.True(e is InvalidOperationException);
                Assert.Contains("Unable to find view '/Views/SomeInvalidView.cshtml'.", e.Message);
            }
        }
    }
}