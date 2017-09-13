using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ValidationSample.Models
{
    public class TestCaseLog
    {
        
        public int TestCaseLogId { get; set; }
        public int TestInstanceId { get; set; }
        public int TestCaseId { get; set; }
        public int? ResultType { get; set; }
        public DateTime? EventStartTime { get; set; }
        public DateTime? EventEndTime { get; set; }
        public string Exception { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}