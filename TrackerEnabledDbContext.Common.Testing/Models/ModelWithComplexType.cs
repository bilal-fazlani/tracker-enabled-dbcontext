using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges]
    public class ModelWithComplexType
    {
        public long Id { get; set; }
        public ComplexType ComplexType { get; set; }
    }

    public class ComplexType
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as ComplexType;

            return Property1 == other.Property1;
        }

        public override int GetHashCode()
        {
            return Property1.GetHashCode();
        }
    }
}
