using Microsoft.Extensions.FileProviders;

namespace Razor.Templating.Core.Dynamic
{
    public class RazorViewFileInfo : IFileInfo
    {
        public bool Exists => true;
        public bool IsDirectory => false;


        public DateTimeOffset _lastModified;
        public DateTimeOffset LastModified => _lastModified;

        public long _length;
        public long Length => _length;


        public string _name;
        public string Name => _name;


        public string _physicalPath;
        public string PhysicalPath => _physicalPath;

        private readonly Stream _stream;

        public RazorViewFileInfo(string fileName, DateTimeOffset lastModified, Stream fileStream)
        {
            _name = _physicalPath = fileName;
            _lastModified = lastModified;
            _length = fileStream.Length;
            _stream = fileStream;
        }

        public Stream CreateReadStream()
        {
            return _stream;
        }
    }
}
