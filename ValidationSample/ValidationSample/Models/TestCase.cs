using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Models
{
    public class TestCase
    {
        public int TestCaseId { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage ="Select Testsuite")]
        public int TestSuiteId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Select Pipeline stage")]
        public int PipelineStageId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Select Testcase type")]
        public int TestCaseTypeId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Testcase Name")]
        public string TestCaseName { get; set; }
        public string Description { get; set; }
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please Select Status")]
        public bool IsActive { get; set; }
        public bool IsObsolete { get; set; }
        public string ObsoleteReason { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}