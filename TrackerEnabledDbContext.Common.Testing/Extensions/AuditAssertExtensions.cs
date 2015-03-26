using System.Collections.Generic;
using System.Linq;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Testing.Extensions
{
    public static class AuditAssertExtensions
    {
        public static T AssertAuditForAddition<T>(this T entity, ITrackerContext db, object entityId, string userName = null, params KeyValuePair<string,string>[] newValues)
        {
            var logs = db.GetLogs<T>(entityId)
                .AssertCountIsNotZero("log count is zero");

            var lastLog = logs.Last(x => x.EventType == EventType.Added && x.UserName == userName)
                .AssertIsNotNull("log not found");

            lastLog.LogDetails
                .AssertCountIsNotZero("no log details found")
                .AssertCount(newValues.Count());

            foreach (var keyValuePair in newValues)
            {
                lastLog.LogDetails.AssertAny(x => x.NewValue == keyValuePair.Value 
                    && x.ColumnName == keyValuePair.Key);
            }

            return entity;
        }

        public static T AssertAuditForDeletion<T>(this T entity, ITrackerContext db, object entityId, string userName = null, params KeyValuePair<string, string>[] oldValues)
        {
            var logs = db.GetLogs<T>(entityId)
                .AssertCountIsNotZero("log count is zero");

            var lastLog = logs.Last(x => x.EventType == EventType.Deleted && x.UserName == userName)
                .AssertIsNotNull("log not found");

            lastLog.LogDetails
                .AssertCountIsNotZero("no log details found")
                .AssertCount(oldValues.Count());

            foreach (var keyValuePair in oldValues)
            {
                lastLog.LogDetails.AssertAny(x => x.OriginalValue == keyValuePair.Value 
                    && x.ColumnName == keyValuePair.Key);
            }

            return entity;
        }

        public static T AssertAuditForModification<T>(this T entity, ITrackerContext db, object entityId, 
            object userName = null, params AuditLogDetail[] logdetails)
        {
            var logs = db.GetLogs<T>(entityId)
                .AssertCountIsNotZero("log count is zero");

            var lastLog = logs.Last(x => x.EventType == EventType.Modified && x.UserName == (userName != null ? userName.ToString(): null))
                .AssertIsNotNull("log not found");

            lastLog.LogDetails
                .AssertCountIsNotZero("no log details found")
                .AssertCount(logdetails.Count());

            foreach (var logdetail in logdetails)
            {
                lastLog.LogDetails.AssertAny(x => x.OriginalValue == logdetail.OriginalValue
                    && x.ColumnName == logdetail.ColumnName
                    && x.NewValue == logdetail.NewValue, "could not find an expected auditlog detail");
            }

            return entity;
        }
    }
}
