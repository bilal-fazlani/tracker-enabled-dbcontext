using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SampleLogMaker.Models
{
    [TrackChanges]
    public class Blog
    {
        [Key]
        [SkipTracking]
        public int Id { get; set; }

        public string Title { get; set; }
    }


}