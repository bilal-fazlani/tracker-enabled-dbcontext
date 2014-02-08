using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext
{
    public partial class TrackerContext : DbContext
    {
        public TrackerContext()
            : base()
        {
            
        }

        public TrackerContext(string connectinString)
            : base(connectinString)
        {
            
        }
        
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<AuditLogDetail> LogDetails { get; set; }
    }

    

    
    
}
