using System;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class RazorTemplateEngineImplTest
    {
        [Fact]
        public void Constructor_Does_Not_Allow_Null_ServiceProvider_Argument()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new RazorTemplateEngineRenderer(null));
            Assert.Equal("serviceProvider", actual.ParamName);
        }
    }
}
