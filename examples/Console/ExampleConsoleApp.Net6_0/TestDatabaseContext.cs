using Microsoft.EntityFrameworkCore;

namespace ExampleConsoleApp.Net6_0
{
    public class TestDatabaseContext : DbContext
    {
        public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Template> Templates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
//            modelBuilder.Entity<Template>().HasData();
        }
    }


    public class Template
    {
        public int Id { get; set; }
        public string ViewName { get; set; }
        public string ViewTemplate { get; set; }
        public DateTime LastRead { get; set; }
        public DateTime LastModified { get; set; }
    }
}
