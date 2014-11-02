using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleLogMaker.Models;
using TrackerEnabledDbContext;
using SampleLogMaker.ViewModels;
using TrackerEnabledDbContext.Models;

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
						UserName = log.UserName,
                        Details = log.LogDetails.Select(x=> new LogDetail {PropertyName = x.ColumnName, NewValue = x.NewValue })
					});
					break;

					case EventType.Deleted: //deleted
					vm.Add(new DeletedHistoryVM {
						Date = log.EventDateUTC.ToLocalTime().DateTime,
						LogId = log.AuditLogId,
						RecordId = int.Parse(log.RecordId),
						TableName = log.TableName,
						UserName = log.UserName,
                        Details = log.LogDetails.Select(x=> new LogDetail { PropertyName = x.ColumnName, OldValue = x.OrginalValue })
					});
					break;

					case EventType.Modified: //modified
					vm.Add(new ChangedHistoryVM {
                        Details = log.LogDetails.Select(x => new LogDetail { PropertyName = x.ColumnName, NewValue = x.NewValue, OldValue = x.OrginalValue }),
                        Date = log.EventDateUTC.ToLocalTime().DateTime,
						LogId = log.AuditLogId,
						RecordId = int.Parse(log.RecordId),
						TableName = log.TableName,
						UserName = log.UserName,
					});
					break;
				}
				
			}

			return View(vm);
		}
	}
}