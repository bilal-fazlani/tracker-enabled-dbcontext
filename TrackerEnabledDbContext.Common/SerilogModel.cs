using System;
using System.Collections.Generic;
using System.Linq;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    public class LogInfo
    {
        public LogInfo(AuditLog log)
        {
            UserName = log.UserName;
            EventDateUTC = log.EventDateUTC;
            EventType = log.EventType;
            TypeFullName = log.TypeFullName;
            RecordId = log.RecordId;
            LogDetails = log.LogDetails.Select(lg => new LogDetailInfo(lg)).ToList();
        }

        public string UserName { get; set; }

        public DateTime EventDateUTC { get; set; }

        public EventType EventType { get; set; }

        public string TypeFullName { get; set; }

        public string RecordId { get; set; }

        public List<LogDetailInfo> LogDetails { get; set; } 
    }

    public class LogDetailInfo
    {
        public LogDetailInfo(AuditLogDetail logDetail)
        {
            PropertyName = logDetail.PropertyName;
            OriginalValue = logDetail.OriginalValue;
            NewValue = logDetail.NewValue;
        }

        public string PropertyName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }
    }
}