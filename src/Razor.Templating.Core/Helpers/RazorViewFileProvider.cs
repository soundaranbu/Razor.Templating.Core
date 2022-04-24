using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Razor.Templating.Core.Infrastructure;
using System;
using System.IO;

namespace Razor.Templating.Core.Helpers
{
    public abstract class RazorViewFileProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return NotFoundDirectoryContents.Singleton;
        }

        public virtual IFileInfo GetFileInfo(string subpath)
        {
            var (razorViewExists, razorViewLastModified, razorViewStream) = GetRazorViewFileInfo(subpath);

            return razorViewExists ? new RazorViewFileInfo(subpath, razorViewLastModified ?? DateTimeOffset.MinValue, razorViewStream ?? Stream.Null) : new NotFoundFileInfo(subpath);
        }

        public virtual IChangeToken Watch(string filter)
        {
            if (filter is null)
            {
                return NullChangeToken.Singleton;
            }

            return new RazorFileChangeToken(HasChanged(filter));
        }

        protected abstract bool HasChanged(string filter);
        protected abstract (bool RazorViewExists, DateTimeOffset? RazorViewLastModified, Stream? RazorViewStream) GetRazorViewFileInfo(string subpath);
    }
}
