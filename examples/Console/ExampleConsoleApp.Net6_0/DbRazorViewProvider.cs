using Razor.Templating.Core.Helpers;
using System.Text;

namespace ExampleConsoleApp.Net6_0
{
    public class DbRazorViewProvider : RazorViewFileProvider
    {
        private readonly TestDatabaseContext _db;

        public DbRazorViewProvider(TestDatabaseContext db)
        {
            _db = db;

            string template = @"
            <h1>hello</h1>
            ";

            var model = new Template
            {
                Id = 1,
                ViewName = "/mysampleview.cshtml",
                ViewTemplate = template,
                LastModified = DateTime.UtcNow,
                LastRead = DateTime.MinValue
            };

            _db.Templates.Add(model);

            _db.SaveChanges();
        }

        protected override (bool RazorViewExists, DateTimeOffset? RazorViewLastModified, Stream? RazorViewStream) GetRazorViewFileInfo(string subpath)
        {
            var razorView = _db.Templates.FirstOrDefault(x => x.ViewName == subpath);

            if (razorView == null)
            {
                return (false, null, null);
            }

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(razorView.ViewTemplate));

            // update last modified
            razorView.LastModified = DateTime.UtcNow;
            _db.SaveChanges();

            return (true, razorView.LastModified, stream);
        }

        protected override bool HasChanged(string filter)
        {
            var razorView = _db.Templates.FirstOrDefault(x => x.ViewName == filter);

            return razorView != null ? razorView?.LastModified > DateTime.UtcNow : false;
        }
    }
}