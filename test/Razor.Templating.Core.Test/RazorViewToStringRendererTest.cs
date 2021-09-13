using ExampleRazorTemplatesLibrary.Models;
using ExampleRazorTemplatesLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Razor.Templating.Core;

namespace Razor.Templating.Test
{
    [TestClass]
    public class RazorViewToStringRendererTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {

        }

        [TestMethod]
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
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("<div>Hello, I'm example view without any model and view data</div>"));
        }

        [TestMethod]
        public async Task RenderView_WithLayout_WithViewData()
        {
            // Arrange
            var viewData = new Dictionary<string, object>();
            viewData["Title"] = "This is Title";

            // Act
            var html = await RazorTemplateEngine.RenderAsync<object>("~/Views/ExampleViewWithLayout.cshtml", null, viewData);

            // Assert
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("This is the view content"));
            Assert.IsTrue(html.Contains("This is Title"));
        }

        [TestMethod]
        public async Task RenderView_WithLayout_WithoutData()
        {
            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithLayout.cshtml");

            // Assert
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("This is the view content"));
        }

        [TestMethod]
        public async Task RenderView_Without_ViewModel()
        {
            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/Feature/ExampleViewWithoutViewModel.cshtml");

            // Assert
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("<div>Hi I'm example view without any viewmodel or view data</div>"));
        }

        [TestMethod]
        public async Task RenderParitialView_WithModel()
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
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("Partial view"));
            Assert.IsTrue(html.Contains("Html content: <em>Lorem Ipsium</em>"));
        }

        [TestMethod]
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
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewServiceInjection.cshtml");

            // Assert
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("Injected Service Data: Some Random Value - "));
        }


        [TestMethod]
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
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("Plain text: Lorem Ipsium"));
        }

        [TestMethod]
        public async Task RenderView_WithTagHelpers()
        {
            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithTagHelpers.cshtml");

            // Assert
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains(@"<label class=""caption"" for=""FirstName"">First Name:</label>"));
            Assert.IsTrue(html.Contains(@"<a href="""">All Speakers</a>"));
        }

        [TestMethod]
        public async Task RenderView_WithViewComponent()
        {
            // Act
            var html = await RazorTemplateEngine.RenderAsync("~/Views/ExampleViewWithViewComponent.cshtml");

            // Assert
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains(@"Example View Component!"));
        }

        [TestMethod]
        public async Task RenderInvalidView_Should_ThrowError()
        {
            try
            {
                var html = await RazorTemplateEngine.RenderAsync("/Views/SomeInvalidView.cshtml");
            }
            catch (System.Exception e)
            {
                Assert.IsTrue(e is InvalidOperationException);
                Assert.IsTrue(e.Message.Contains("Unable to find view '/Views/SomeInvalidView.cshtml'."));
            }
        }
    }
}
