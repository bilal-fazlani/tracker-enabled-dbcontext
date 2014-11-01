using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleLogMaker.ViewModels
{
    public class BaseHistoryVM
    {
        public int LogId { get; set; }
        
        public int RecordId { get; set; }

        public DateTime Date { get; set; }

        public string UserName { get; set; }

        public string TableName { get; set; }

        public IEnumerable<LogDetail> Details = new List<LogDetail>();
    }

    public class ChangedHistoryVM : BaseHistoryVM
    {
        
    }

    public class DeletedHistoryVM : BaseHistoryVM
    {
        
    }

    public class AddedHistoryVM : BaseHistoryVM
    {
        
    }
}