using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationSample.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }

        public string Recurrence { get; set; }
        public string RecurrencePattern { get; set; }

        public string Location { get; set; }
        public string RecurrenceCount { get; set; }
        public string RecurrenceInterval { get; set; }
    }
}