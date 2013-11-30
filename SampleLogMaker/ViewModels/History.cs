using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleLogMaker.ViewModels
{
    public class BaseHistoryVM
    {
        public string DateFormat = "dd-MMM-yyyy  hh:mm tt";

        public Guid LogId { get; set; }
        
        public int RecordId { get; set; }

        public DateTime Date { get; set; }

        public string UserName { get; set; }

        public string TableName { get; set; }

        public virtual string Message { get; set; }
    }

    public class ChangedHostoryVM : BaseHistoryVM
    {
        public string ColumnName { get; set; }

        public string NewValue { get; set; }

        public string OldValue { get; set; }

        public new string Message
        {
            get
            {
                return string.Format("{0} in {1} was changed by {2} from {3} to {4} at {5}.",
                    ColumnName,
                    TableName,
                    UserName,
                    OldValue,
                    NewValue,
                    Date.ToString(DateFormat)
                    );
            }
        }
    }

    public class DeletedHistoryVM : BaseHistoryVM
    {
        public new string Message
        {
            get
            {
                return string.Format("record #: {0} in {1} was deleted by {2} at {3}.",
                    RecordId,
                    TableName,
                    UserName,
                    Date.ToString(DateFormat)
                    );
            }
        }
    }

    public class AddedHistoryVM : BaseHistoryVM
    {
        public new string Message
        {
            get
            {
                return string.Format("record #: {0} in {1} was added by {2} at {3}",
                    RecordId,
                    TableName,
                    UserName,
                    Date.ToString(DateFormat)
                    );
            }
        }
    }
}