using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleLogMaker.ViewModels
{
    public class BaseHistoryVM
    {
        public int LogId { get; set; }
        
        /// <summary>
        /// This is a strign now as it can be a composite key. This may be changed to array/collection later.
        /// </summary>
        public string RecordId { get; set; }

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