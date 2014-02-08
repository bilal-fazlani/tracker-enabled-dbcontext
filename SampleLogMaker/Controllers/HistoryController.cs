using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleLogMaker.Models;
using TrackerEnabledDbContext;
using SampleLogMaker.ViewModels;

namespace SampleLogMaker.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        //
        // GET: /History/
        public ActionResult Index()
        {
            var db = new MyDbContext();
            var data = db.AuditLog
                .OrderByDescending(x=>x.EventDateUTC)
                .ToList();
            var vm = new List<BaseHistoryVM>();

            foreach (var log in data)
            {
                switch (log.EventType)
                {
                    case EventType.Added : // added
                    vm.Add(new AddedHistoryVM {
                        Date = log.EventDateUTC.ToLocalTime().DateTime,
                        LogId = log.AuditLogId,
                        RecordId = int.Parse(log.RecordId),
                        TableName = log.TableName,
                        UserName = log.UserName
                    });
                    break;

                    case EventType.Deleted: //deleted
                    vm.Add(new DeletedHistoryVM {
                        Date = log.EventDateUTC.ToLocalTime().DateTime,
                        LogId = log.AuditLogId,
                        RecordId = int.Parse(log.RecordId),
                        TableName = log.TableName,
                        UserName = log.UserName
                    });
                    break;

                    case EventType.Modified: //modified
                    vm.Add(new ChangedHistoryVM {
                        //ColumnName = log.ColumnName,
                        Date = log.EventDateUTC.ToLocalTime().DateTime,
                        LogId = log.AuditLogId,
                        RecordId = int.Parse(log.RecordId),
                        TableName = log.TableName,
                        UserName = log.UserName,
                        //OldValue = log.OriginalValue,
                        //NewValue = log.NewValue
                    });
                    break;
                }
                
            }

            return View(vm);
        }
	}
}