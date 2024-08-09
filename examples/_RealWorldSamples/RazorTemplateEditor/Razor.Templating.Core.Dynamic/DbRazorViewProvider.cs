using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core.Dynamic.Data;
using System.Text;

namespace Razor.Templating.Core.Dynamic
{
    public class DbRazorViewProvider : RazorViewFileProvider
    {
        private TestDatabaseContext _db;
        private readonly IServiceProvider sp;

        public DbRazorViewProvider(IServiceProvider sp)
        {
            this.sp = sp;
        }

        protected override (bool RazorViewExists, DateTimeOffset? RazorViewLastModified, Stream? RazorViewStream) GetRazorViewFileInfo(string subpath)
        {
            using var scope = sp.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<TestDatabaseContext>();
            var razorView = _db.Templates.FirstOrDefault(x => subpath.EndsWith(x.ViewName));

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
            using var scope = sp.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<TestDatabaseContext>();

            var razorView = _db.Templates.FirstOrDefault(x => x.ViewName == filter);

            return razorView != null ? razorView?.LastModified > DateTime.UtcNow : false;
        }
    }
}