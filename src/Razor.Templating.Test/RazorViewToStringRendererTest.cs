using ExampleRazorTemplatesLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Razor.Templating.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorRendererTest
{
    [TestClass]
    public class RazorViewToStringRendererTest
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            //Optionally call this to create cache of the renderer
            //Otherwise, render time will be more than usual on first time only
            RazorTemplateEngine.Initialize();
        }

        [TestMethod]
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
            Assert.IsNotNull(html);
            Assert.IsTrue(html.Contains("Lorem Ipsium"));
            Assert.IsTrue(html.Contains("<em>Lorem Ipsium</em>"));
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
    }
}
