using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.Common;

namespace HangfireData.Models
{
    public class JobSearchResultsInternal : BaseEntity
    {
        public string InvocationData { get; set; }
        public string Arguments { get; set; }
        public string Queue { get; set; }
        public string StateData { get; set; }
        public string Processor { get; set; }
        public string Display { get; set; }
        public int RowNumber { get; set; }
        public string StateName { get; set; }
        public int Total { get; set; }
    }

    public class JobSearchResult
    {
        public object Result { get; set; }
        public long? Duration { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Method { get; set; }
        public string ParameterTypes { get; set; }
        public string Type { get; set; }
        public int JobId { get; set; }
        public string State { get; set; }
        public string Display { get; set; }
        public int Total { get; set; }
    }
}
