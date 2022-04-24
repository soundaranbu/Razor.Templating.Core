using Razor.Templating.Core.Helpers;

namespace ExampleConsoleApp.Net6_0
{
    public class BlobRazorViewSource : RazorViewFileProvider
    {
        private readonly string _connectionString;

        public BlobRazorViewSource(string connectionString = "")
        {
            _connectionString = connectionString;
        }

        protected override (bool RazorViewExists, DateTimeOffset? RazorViewLastModified, Stream? RazorViewStream) GetRazorViewFileInfo(string subpath)
        {
            // implement logic to retrive the file
            var fileExists = subpath.ToUpper().Contains(@"Views\ExampleViewWithoutViewModel.cshtml".ToUpper());
            // NOTE: CHANGE THIS
            var fileInfo = new FileInfo(@"D:\Projects\MyProjects\RazorTemplating\examples\Templates\ExampleAppRazorTemplates\Views\ExampleViewWithoutViewModel.cshtml");

            return (fileExists, fileInfo.LastWriteTime, fileInfo.Create());
        }

        protected override bool HasChanged(string filter)
        {
            var fileInfo = new FileInfo(@"D:\Projects\MyProjects\RazorTemplating\examples\Templates\ExampleAppRazorTemplates\Views\ExampleViewWithoutViewModel.cshtml");

            if (fileInfo?.LastWriteTimeUtc > fileInfo?.LastAccessTimeUtc)
            {
                return true;
            }

            return false;
        }
    }
}