using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleLogMaker.ViewModels
{
    public class LogDetail
    {
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}