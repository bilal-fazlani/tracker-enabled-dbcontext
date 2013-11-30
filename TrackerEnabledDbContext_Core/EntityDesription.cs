using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext
{
    /// <summary>
    /// This is an extension to help describe model entities in string format.
    /// It can help describing an entity to its Original Values or Curent Values.
    /// </summary>
    public static class EntityDesription
    {
        /// <summary>
        /// Describes the DbEntityEntry in json format using its Original values
        /// </summary>
        /// <param name="dbEntry">DbEntityEntry</param>
        /// <returns>String representation of DbEntityEntry</returns>
        public static string DescribeOriginal(this DbEntityEntry dbEntry)
        {
            var result = "{ ";

            foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
            {
                var value = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
                result += string.Format("{0}:'{1}',", propertyName,value);
            }

            return result + " }";
        }

        /// <summary>
        /// Describes the DbEntityEntry in json format using its Current values
        /// </summary>
        /// <param name="dbEntry">DbEntityEntry</param>
        /// <returns>String representation of DbEntityEntry</returns>
        public static string DescribeCurrent(this DbEntityEntry dbEntry)
        {
            var result = "{ ";

            foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
            {
                var value = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
                result += string.Format("{0}:'{1}',", propertyName, value);
            }

            return result + " }";
        }
    }
}
