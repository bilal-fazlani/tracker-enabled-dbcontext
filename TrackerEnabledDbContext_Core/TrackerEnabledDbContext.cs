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
    public class TrackerContext : DbContext
    {
        public bool TrackNewValues { get; set; }

        public TrackerContext(bool trackNewValues = false)
            : base()
        {
            TrackNewValues = trackNewValues;
        }

        public TrackerContext(string connectinString, bool trackNewValues = false)
            : base(connectinString)
        {
            TrackNewValues = trackNewValues;
        }

        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<AuditLogChild> LogChildren { get; set; }

        /// <summary>
        /// This method saves the model changes to the database.
        /// If the tracker for of table is active, it will also put the old values in tracking table.
        /// Always use this method instead of SaveChanges() whenever possible.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public int SaveChanges(object userId)
        {
            // Get all Deleted/Modified entities (not Unmodified or Detached or Added)
            foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                // For each changed record, get the audit record entries and add them
                foreach (AuditLog x in GetAuditRecordsForChange(ent, userId))
                {
                    this.AuditLog.Add(x);
                }
            }

            var AddedEntries = this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
            var result = base.SaveChanges();
            // By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.

            // Get all Added entities
            foreach (var ent in AddedEntries)
            {
                // For each changed record, get the audit record entries and add them
                foreach (AuditLog x in GetAuditRecordsForAddition(ent, userId))
                {
                    this.AuditLog.Add(x);
                }
            }

            //save changed to audit of added entries
            base.SaveChanges();
            return result;
        }

        /// <summary>
        /// Return the Log entries for added entities
        /// </summary>
        /// <param name="dbEntry">Entity to process</param>
        /// <param name="userName">Logged in Username</param>
        /// <returns></returns>
        private IEnumerable<AuditLog> GetAuditRecordsForAddition(DbEntityEntry dbEntry, object userId)
        {
            var entityType = GetEntityType(dbEntry.Entity.GetType());

            List<AuditLog> result = new List<AuditLog>();

            DateTime changeTime = DateTime.UtcNow;

            TableAttribute tableAttr = entityType.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            TrackChanges trackChangesAttr = entityType.GetCustomAttributes(typeof(TrackChanges), false).SingleOrDefault() as TrackChanges;

            if (trackChangesAttr == null || !trackChangesAttr.Enabled)
            {
                return result;
            }

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : entityType.Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            var keyName = entityType.GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

            // For Inserts, just add the whole record
            // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()
            result.Add(new AuditLog()
            {
                AuditLogID = Guid.NewGuid(),
                UserId = userId.ToString(),
                EventDateUTC = changeTime,
                EventType = "A", // Added
                TableName = tableName,
                RecordID = dbEntry.CurrentValues.GetValue<object>(keyName).ToString(),  // Again, adjust this if you have a multi-column key
                ColumnName = "*ALL",    // ,Or make it nullable, whatever you want
                NewValue = (TrackNewValues?dbEntry.DescribeCurrent():null)
            }
            );

            return result;
        }

        /// <summary>
        /// Return the Log entries for changed or deleted entities
        /// </summary>
        /// <param name="dbEntry">Entity to process</param>
        /// <param name="userName">Logged in Username</param>
        /// <returns></returns>
        private IEnumerable<AuditLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, object userId)
        {

            var entityType = GetEntityType(dbEntry.Entity.GetType());

            List<AuditLog> result = new List<AuditLog>();

            DateTime changeTime = DateTime.UtcNow;

            // Get the Table() attribute, if one exists
            TableAttribute tableAttr = entityType.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            TrackChanges trackChangesAttr = entityType.GetCustomAttributes(typeof(TrackChanges), false).SingleOrDefault() as TrackChanges;

            if (trackChangesAttr == null || !trackChangesAttr.Enabled)
            {
                return result;
            }

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : entityType.Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            var keyName = entityType.GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

            if (dbEntry.State == EntityState.Deleted)
            {
                // Same with deletes, do the whole record, and use either the description from Describe() or ToString()
                result.Add(new AuditLog()
                {
                    AuditLogID = Guid.NewGuid(),
                    UserId = userId.ToString(),
                    EventDateUTC = changeTime,
                    EventType = "D", // Deleted
                    TableName = tableName,
                    RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                    ColumnName = "*ALL",//,
                    OriginalValue = dbEntry.DescribeOriginal(),
                    NewValue = null
                    //(dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
                }
                );
            }
            else if (dbEntry.State == EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    var SkipTracking = entityType.GetProperty(propertyName).GetCustomAttributes(false).OfType<SkipTracking>().SingleOrDefault() as SkipTracking;
                    // For those only where skip tracking is not mentioned OR skiptracking is disabled
                    if (SkipTracking == null || !SkipTracking.Enabled)
                    {
                        // For updates, we only want to capture the columns that actually changed
                        if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                        {
                            result.Add(
                                new AuditLog()
                                {
                                    AuditLogID = Guid.NewGuid(),
                                    UserId = userId.ToString(),
                                    EventDateUTC = changeTime,
                                    EventType = "M",    // Modified
                                    TableName = tableName,
                                    RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                                    ColumnName = propertyName,
                                    OriginalValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                                    NewValue = (this.TrackNewValues ? dbEntry.CurrentValues.GetValue<object>(propertyName).ToString() : null)
                                }
                            );
                        }
                    }
                }
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities
            return result;
        }

        private Type GetEntityType(Type entityType){
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
    }

    

    
    
}
