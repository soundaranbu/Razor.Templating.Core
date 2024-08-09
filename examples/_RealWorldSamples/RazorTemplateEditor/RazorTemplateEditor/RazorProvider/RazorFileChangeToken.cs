using Microsoft.Extensions.Primitives;
using System;

namespace Razor.Templating.Core.Infrastructure
{
    internal class RazorFileChangeToken : IChangeToken
    {
        public RazorFileChangeToken(bool hasChanged)
        {
            HasChanged = hasChanged;
        }

        /// <summary>
        /// Always false.
        /// </summary>
        public bool ActiveChangeCallbacks => false;

        /// <summary>
        /// Tells whether the file has changed or not
        /// </summary>
        public bool HasChanged { get; private set; }

        /// <summary>
        /// Always returns an empty disposable object. Callbacks will never be called.
        /// </summary>
        /// <param name="callback">This parameter is ignored</param>
        /// <param name="state">This parameter is ignored</param>
        /// <returns>A disposable object that noops on dispose.</returns>
        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
        {
            return EmptyDisposable.Instance;
        }
    }
}
