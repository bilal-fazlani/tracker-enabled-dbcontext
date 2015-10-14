using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Tools
{
    public class MigrationJobStatus
    {
        public string EntityFullName { get; set; }
        public int Percent { get; set; }
    }
}
