using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleLogMaker.Models
{
    /// <summary>
    /// Added support for composite key entities. Also added support for entities who do not have specified key, but have primary key based on EF conventions.
    /// </summary>
    [TrackChanges]
    public class Car
    {
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ModelName { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Make { get; set; }

        [StringLength(300)]
        public string Description { get; set; }
    }
}
