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
                    case "A": // added
                    vm.Add(new AddedHistoryVM {
                        Date = log.EventDateUTC.ToLocalTime().DateTime,
                        LogId = log.AuditLogID,
                        RecordId = int.Parse(log.RecordID),
                        TableName = log.TableName,
                        UserName = log.UserId
                    });
                    break;

                    case "D": //deleted
                    vm.Add(new DeletedHistoryVM {
                        Date = log.EventDateUTC.ToLocalTime().DateTime,
                        LogId = log.AuditLogID,
                        RecordId = int.Parse(log.RecordID),
                        TableName = log.TableName,
                        UserName = log.UserId
                    });
                    break;

                    case "M" : //modified
                    vm.Add(new ChangedHostoryVM {
                        ColumnName = log.ColumnName,
                        Date = log.EventDateUTC.ToLocalTime().DateTime,
                        LogId = log.AuditLogID,
                        RecordId = int.Parse(log.RecordID),
                        TableName = log.TableName,
                        UserName = log.UserId,
                        OldValue = log.OriginalValue,
                        NewValue = log.NewValue
                    });
                    break;
                }
                
            }

            return View(vm);
        }
	}
}