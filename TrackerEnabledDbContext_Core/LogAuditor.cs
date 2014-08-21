using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Models;

namespace TrackerEnabledDbContext
{
    public class LogAuditor : IDisposable
    {
        private readonly DbEntityEntry _dbEntry;

        public LogAuditor(DbEntityEntry dbEntry)
        {
            _dbEntry = dbEntry;
        }

        public AuditLog GetLogRecord(object userName, EventType eventType,DbContext context)
        {
            var entityType = Helper.GetEntityType(_dbEntry.Entity.GetType());
            DateTime changeTime = DateTime.UtcNow;

            TrackChanges trackChangesAttr = entityType.GetCustomAttributes(typeof(TrackChanges), false).SingleOrDefault() as TrackChanges;

            if (trackChangesAttr == null || !trackChangesAttr.Enabled) { return null; }

            //Get primary key value (If you have more than one key column, this will need to be adjusted)
            string keyName;
            try
            {
                keyName =
                    entityType.GetProperties()
                        .Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
                        .Name;
            }
            catch (Exception)
            {
                throw new KeyNotFoundException(string.Format(@"A single primary key attribute is reqiured per entity for tracker to work. 
The entity '{0}' does not contain a primary key attribute or contains more than one.", entityType.Name));
            }

            var newlog = new AuditLog
            {
                UserName = userName.ToString(),
                EventDateUTC = changeTime,
                EventType = eventType,
                TableName = GetTableName(entityType, context),
                //GoliathDeveloper 21/08/2014
                RecordId = _dbEntry.GetDatabaseValues().GetValue<object>(keyName).ToString()
            };
            
            using (var detailsAuditor = new LogDetailsAuditor(_dbEntry, newlog))
            {
                newlog.LogDetails = detailsAuditor.GetLogDetails().ToList();
            }

            return newlog;
        }

        private string GetTableName(Type entityType,DbContext context)
        {
            TableAttribute tableAttr = entityType.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            var dbsetPropertyName = context.GetType().GetProperties().Single(x => x.PropertyType.GenericTypeArguments.Any(y => y == entityType)).Name;

            // Get table name (if it has a Table attribute, use that, otherwise dbset property name)
            string tableName = tableAttr != null ? tableAttr.Name : dbsetPropertyName;

            return tableName;
        }

        public void Dispose()
        {
            
        }
    }
}
