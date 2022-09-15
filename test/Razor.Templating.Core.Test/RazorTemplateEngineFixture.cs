using System;
using System.Threading;
using Xunit;

namespace Razor.Templating.Core.Test
{
    /// <summary>
    /// Provides a synchronized access to the using RazorTemplateEngine in unit tests in a isolated way.
    /// Any Unit Tests that use the static RazorTemplateEngine should use this fixture.
    /// </summary>
    public class RazorTemplateEngineFixture : IDisposable
    {
        public IDisposable BeginTest()
        {
            return new RazorTemplateEngineReset();
        }

        public void Dispose()
        {
        }

        private class RazorTemplateEngineReset : IDisposable
        {
            private static readonly object SyncLock = new();

            public RazorTemplateEngineReset()
            {
                Monitor.Enter(SyncLock);
            }

            public void Dispose()
            {
                try
                {
                    RazorTemplateEngine.Reset();
                }
                finally
                {
                    Monitor.Exit(SyncLock);
                }
            }
        }
    }

    [CollectionDefinition("Razor Template Engine collection")]
    public class RazorTemplateEngineCollectionFixture : ICollectionFixture<RazorTemplateEngineFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
