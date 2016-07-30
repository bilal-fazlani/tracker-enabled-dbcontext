using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    public class TimeMachine
    {
        private readonly ITrackerContext _db;

        public TimeMachine(ITrackerContext db)
        {
            _db = db;
        }

        public Snapshot<T> TimeTravel<T>(DateTime utcDate, object primaryKey) where T : class, new()
        {
            var logs = _db.GetLogs<T>(primaryKey)
                .Where(x => x.EventDateUTC <= utcDate)
                .ToList();

            Dictionary<string, string> propertyValues = new Dictionary<string, string>();

            Snapshot<T> snapshot = new Snapshot<T>();

            foreach (var auditLog in logs)
            {
                foreach (var detail in auditLog.LogDetails)
                {
                    if (auditLog.EventType != EventType.Deleted)
                        propertyValues[detail.PropertyName] = detail.NewValue;
                    else
                        propertyValues[detail.PropertyName] = detail.OriginalValue;
                }
            }

            snapshot.LastAuditLog = logs.LastOrDefault();
            if (snapshot.LastAuditLog != null)
                snapshot.Entity = ConstructEntity<T>(propertyValues);

            return snapshot;
        }

        private T ConstructEntity<T>(Dictionary<string, string> propertyValues) where T : class, new()
        {
            T entity = new T();
            Type entityType = typeof(T);

            foreach (var property in propertyValues)
            {
                PropertyInfo propertyInfo = entityType.GetProperty(property.Key);
                propertyInfo.SetValueFromString(entity, property.Value);
            }

            return entity;
        }
    }

    public class Snapshot<T> where T : class, new()
    {
        public AuditLog LastAuditLog { get; set; }

        public T Entity { get; set; }
    }
}