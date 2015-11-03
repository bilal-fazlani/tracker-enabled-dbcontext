using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges]
    public class SoftDeletableModel :ISoftDeletable
    {
        public long Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Description { get; set; }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
