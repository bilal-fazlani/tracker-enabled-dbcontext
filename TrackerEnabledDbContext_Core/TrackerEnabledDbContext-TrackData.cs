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
    public partial class TrackerContext
    {
        /// <summary>
        /// This method saves the model changes to the database.
        /// If the tracker for of table is active, it will also put the old values in tracking table.
        /// Always use this method instead of SaveChanges() whenever possible.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public int SaveChanges(object userName)
        {
            // Get all Deleted/Modified entities (not Unmodified or Detached or Added)
            foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {

                var record = GetLogRecord(ent, userName,
                    ent.State == EntityState.Modified ? EventType.Modified : EventType.Deleted);
                if (record != null)
                {
                    this.AuditLog.Add(record);
                }
            }

            var addedEntries = this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
            var result = base.SaveChanges();
            //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.

            // Get all Added entities
            foreach (var ent in addedEntries)
            {
                var record = GetLogRecord(ent, userName, EventType.Added);
                if (record != null)
                {
                    this.AuditLog.Add(record);    
                }
            }

            //save changed to audit of added entries
            base.SaveChanges();
            return result;
        }

        private AuditLog GetLogRecord(DbEntityEntry dbEntry, object userName, EventType eventType)
        {
            var entityType = EntityDesription.GetEntityType(dbEntry.Entity.GetType());
            DateTime changeTime = DateTime.UtcNow;
            TableAttribute tableAttr = entityType.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            TrackChanges trackChangesAttr = entityType.GetCustomAttributes(typeof(TrackChanges), false).SingleOrDefault() as TrackChanges;
            if (trackChangesAttr == null || !trackChangesAttr.Enabled) {return null;}

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : entityType.Name;

            //Get primary key value (If you have more than one key column, this will need to be adjusted)
            var keyName = entityType.GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any()).Name;

            var newlog = new AuditLog()
            {
                UserName = userName.ToString(),
                EventDateUTC = changeTime,
                EventType = eventType,
                TableName = tableName,
                RecordId = dbEntry.OriginalValues.GetValue<object>(keyName).ToString()
            };

            newlog.LogDetails = dbEntry.LogDetails(newlog).ToList();

            return newlog;
        }
    }
}
