using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationSample.Models
{
    public class TestCaseType
    {
        public int TestCaseTypeId { get; set; }
        public string TestCaseType1 { get; set; }
        public string TestCaseTypeDescription { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}