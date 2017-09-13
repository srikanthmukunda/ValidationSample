using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationSample.Models
{
    public class PipelineStage
    {
        public int PipelineStageId { get; set; }
        public string PipelineStage1 { get; set; }
        public string PipelineStageDescription { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}