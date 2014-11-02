using SampleLogMaker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
