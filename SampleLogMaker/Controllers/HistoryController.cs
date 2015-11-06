using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SampleLogMaker.Models;
using SampleLogMaker.ViewModels;
using TrackerEnabledDbContext.Common.Models;

namespace SampleLogMaker.Controllers
{
    [Authorize]
	public class HistoryController : Controller
	{
		//
		// GET: /History/
		public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            var data = db.AuditLog
                .OrderByDescending(x => x.EventDateUTC)
                .ToList();
            List<BaseHistoryVM> vm = ConvertToHistoryViewModel(data);

            return View(vm);
        }

        private static List<BaseHistoryVM> ConvertToHistoryViewModel(IEnumerable<AuditLog> data)
        {
            var vm = new List<BaseHistoryVM>();

            foreach (var log in data)
            {
                switch (log.EventType)
                {
                    case EventType.Added: // added
                        vm.Add(new AddedHistoryVM
                        {
                            Date = log.EventDateUTC.ToLocalTime().DateTime,
                            LogId = log.AuditLogId,
                            RecordId = log.RecordId,
                            TypeFullName = log.TypeFullName,
                            UserName = log.UserName,
                            Details = log.LogDetails.Select(x => new LogDetail { PropertyName = x.PropertyName, NewValue = x.NewValue })
                        });
                        break;

                    case EventType.Deleted: //deleted
                        vm.Add(new DeletedHistoryVM
                        {
                            Date = log.EventDateUTC.ToLocalTime().DateTime,
                            LogId = log.AuditLogId,
                            RecordId = log.RecordId,
                            TypeFullName = log.TypeFullName,
                            UserName = log.UserName,
                            Details = log.LogDetails.Select(x => new LogDetail { PropertyName = x.PropertyName, OldValue = x.OriginalValue })
                        });
                        break;

                    case EventType.Modified: //modified
                        vm.Add(new ChangedHistoryVM
                        {
                            Details = log.LogDetails.Select(x => new LogDetail { PropertyName = x.PropertyName, NewValue = x.NewValue, OldValue = x.OriginalValue }),
                            Date = log.EventDateUTC.ToLocalTime().DateTime,
                            LogId = log.AuditLogId,
                            RecordId = log.RecordId,
                            TypeFullName = log.TypeFullName,
                            UserName = log.UserName,
                        });
                        break;
                }

            }

            return vm;
        }

        public PartialViewResult EntityHistory(string typeFullName, object entityId)
        {
            var db = new ApplicationDbContext();
            var auditLogs = db.GetLogs(typeFullName, entityId)
                .OrderByDescending(x=>x.EventDateUTC);
            var viewModels = ConvertToHistoryViewModel(auditLogs);

            ViewBag.EntityType = typeFullName;
            ViewBag.EntityId = entityId;

            return PartialView("_EntityHistory", viewModels);
        }
	}
}