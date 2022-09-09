using System;
using Xunit;

namespace Razor.Templating.Core.Test
{
    public class RazorTemplateEngineImplTest
    {
        [Fact]
        public void Constructor_does_not_allow_null_serviceProvider_argument()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new RazorTemplateEngineImpl(null));
            Assert.Equal("serviceProvider", actual.ParamName);
        }
    }
}
