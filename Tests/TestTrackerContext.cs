using SampleLogMaker.Models;
using System.Data.Entity;
using TrackerEnabledDbContext;

namespace Tests
{
    public class TestTrackerContext : TrackerContext
    {
        public TestTrackerContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
