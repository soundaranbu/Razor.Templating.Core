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
        [TestMethod]
        public async Task RenderViewToStringAsync_CompiledRazorTemplateAndModel_Html()
        {
            // Act
            var model = new ExampleModel() { 
                PlainText = "Some text", 
                HtmlContent = "<em>Some emphasized text</em>" 
            };

            var viewData = new Dictionary<string, object>();
            viewData["Value1"] = "1";
            viewData["Value2"] = "2";

            var html = await RazorTemplateEngine.RenderAsync("/Views/ExampleView.cshtml", model, viewData);

            // Assert
            Assert.IsNotNull(html);
        }
    }
}
