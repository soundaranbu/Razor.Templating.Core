using Microsoft.EntityFrameworkCore;

namespace RazorTemplateEditor.Data
{
    public class TestDatabaseContext : DbContext
    {
        public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Template> Templates { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var template = @"
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
            //modelBuilder.Entity<Template>().HasData(model);
        }
    }

    public class Template
    {
        public int Id { get; set; }
        public string ViewName { get; set; } = null!;
        public string ViewTemplate { get; set; } = null!;
        public DateTime LastRead { get; set; }
        public DateTime LastModified { get; set; }
    }
}
