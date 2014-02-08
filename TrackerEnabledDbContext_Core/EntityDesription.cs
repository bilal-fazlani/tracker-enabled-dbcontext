using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
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
        public static IEnumerable<AuditLogDetail> LogDetails(this DbEntityEntry dbEntry, AuditLog log)
        {
            foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
            {
                if (TrackingEnabled(dbEntry, propertyName) && ValueChanged(dbEntry, propertyName))
                {
                    yield return new AuditLogDetail
                    {
                        ColumnName = propertyName,
                        OrginalValue = OriginalValue(dbEntry, propertyName),
                        NewValue = CurrentValue(dbEntry, propertyName),
                        Log = log
                    };
                }
            }
        }

        private static bool TrackingEnabled(DbEntityEntry dbEntry, string propertyName)
        {
            var entityType = GetEntityType(dbEntry.Entity.GetType());
            var skipTracking = entityType.GetProperty(propertyName).GetCustomAttributes(false).OfType<SkipTracking>().SingleOrDefault();
            return skipTracking == null || !skipTracking.Enabled;
        }

        private static bool ValueChanged(DbEntityEntry dbEntry, string propertyName)
        {
            return !Equals(OriginalValue(dbEntry, propertyName), CurrentValue(dbEntry, propertyName));
        }

        private static string OriginalValue(DbEntityEntry dbEntry, string propertyName)
        {
            string originalValue;

            if (dbEntry.State == EntityState.Unchanged)
            {
                originalValue = null;
            }
            else
            {
                originalValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
            }

            return originalValue;
        }

        private static string CurrentValue(DbEntityEntry dbEntry, string propertyName)
        {
            string newValue;

            try
            {
                newValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null
                    ? null
                    : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
            }
            catch (System.InvalidOperationException) // It will be invalid operation when its in deleted state. in that case, new value should be null
            {
                newValue = null;
            }

            return newValue;
        }
        public static Type GetEntityType(Type entityType)
        {
            if (entityType.Namespace == ProxyNamespace)
            {
                return GetEntityType(entityType.BaseType);
            }
            else
            {
                return entityType;
            }
        }

        private const string ProxyNamespace = @"System.Data.Entity.DynamicProxies";

        ///// <summary>
        ///// Describes the DbEntityEntry in json format using its Original values
        ///// </summary>
        ///// <param name="dbEntry">DbEntityEntry</param>
        ///// <returns>String representation of DbEntityEntry</returns>
        //public static string DescribeOriginal(this DbEntityEntry dbEntry)
        //{
        //    var result = "{ ";

        //    foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
        //    {
        //        var value = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
        //        result += string.Format("{0}:'{1}',", propertyName,value);
        //    }

        //    return result + " }";
        //}
        //public static IEnumerable<AuditLogDetail> OriginalAuditLogDetails(this DbEntityEntry dbEntry,AuditLog log)
        //{
        //    foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
        //    {
        //        if (TrackingEnabled(dbEntry, propertyName) /*&& ValueChanged(dbEntry, propertyName)*/)
        //        {
        //            var value = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
        //            yield return new AuditLogDetail
        //            {
        //                ColumnName = propertyName,
        //                OrginalValue = value,
        //                NewValue = 
        //                Log = log
        //            };
        //        }
        //    }
        //}

        //public static IEnumerable<AuditLogDetail> CurrentAuditLogDetails(this DbEntityEntry dbEntry, AuditLog log)
        //{
        //    foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
        //    {
        //        if (TrackingEnabled(dbEntry, propertyName) /*&& ValueChanged(dbEntry, propertyName)*/)
        //        {
        //            var value = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
        //            yield return new AuditLogDetail
        //            {
        //                ColumnName = propertyName,
        //                OrginalValue = value,
        //                Log = log
        //            };
        //        }
        //    }
        //}

        ///// <summary>
        ///// Describes the DbEntityEntry in json format using its Current values
        ///// </summary>
        ///// <param name="dbEntry">DbEntityEntry</param>
        ///// <returns>String representation of DbEntityEntry</returns>
        //public static string DescribeCurrent(this DbEntityEntry dbEntry)
        //{
        //    var result = "{ ";

        //    foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
        //    {
        //        var value = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString();
        //        result += string.Format("{0}:'{1}',", propertyName, value);
        //    }

        //    return result + " }";
        //}
    }
}
