using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.Common;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using HangfireData.Models;

namespace HangfireData
{
    public class HangfireJobQuery : DapperTableBase
    {
        protected override string TableName => "Job";
        public HangfireJobQuery(IHangfireSack sack) : base(sack)
        {
        }

        public IEnumerable<JobSearchResult> SearchJobs(int from, int count, string searchTerm, string methodName, string state)
        {
            var queryParams = new[]
            {
                string.IsNullOrEmpty(searchTerm) ? string.Empty : " and j.Arguments LIKE '%' + @searchTerm + '%' ",
                string.IsNullOrEmpty(methodName) ? string.Empty : " and jm.Processor = @methodName "
            };

            var sql = $@"select * from(
                select j.*, s.Data as StateData, jm.Queue, jm.[Processor], jm.Display, row_number() over (order by j.Id desc) as RowNumber, COUNT(*) OVER() AS [Total]
                from [Hangfire].Job j with(nolock, forceseek)
                join [Hangfire].JobMeta jm with(nolock) on j.Id = jm.JobId
                join [Hangfire].[State] s with(nolock) on s.Id = j.StateId
                where s.Name = @state {queryParams[0]}	{queryParams[1]}
				) as j where j.RowNumber between @start and @end";
            var parms = new { searchTerm, methodName, state, start = from, end = from + count };
            var rawresult = LoadCollection<JobSearchResultsInternal>(sql, parms) ?? new List<JobSearchResultsInternal>();
            foreach (var i in rawresult)
            {
                yield return Transform(i);
            }
        }

        public JobSearchResult Transform(JobSearchResultsInternal rawResults)
        {
            var data = JobHelper.FromJson<InvocationData>(rawResults.InvocationData);
            data.Arguments = rawResults.Arguments;
            var deserializedData = JobHelper.FromJson<Dictionary<string, string>>(rawResults.StateData);
            var stateData = deserializedData != null ? new Dictionary<string, string>(deserializedData, StringComparer.OrdinalIgnoreCase) : null;
            
            return new JobSearchResult()
            {
                Method = data.Method,
                ParameterTypes = data.ParameterTypes,
                Result = stateData.ContainsKey("Result") ? stateData["Result"] : null,
                Duration = stateData.ContainsKey("PerformanceDuration") && stateData.ContainsKey("Latency")
                    ? long.Parse(stateData["PerformanceDuration"]) + (long?)long.Parse(stateData["Latency"]) : null,
                CompletedAt = ExtractCompletionDT(stateData),
                JobId = rawResults.Id,
                State = rawResults.StateName,
                Type = data.Type,
                Display = rawResults.Display,
                Total = rawResults.Total
            };
        }

        private DateTime? ExtractCompletionDT(Dictionary<string, string> stateData)
        {
            if (stateData.ContainsKey("SucceededAt"))
                return JobHelper.DeserializeNullableDateTime(stateData["SucceededAt"]);
            if (stateData.ContainsKey("FailedAt"))
                return JobHelper.DeserializeNullableDateTime(stateData["FailedAt"]);
            if (stateData.ContainsKey("DeletedAt"))
                return JobHelper.DeserializeNullableDateTime(stateData["DeletedAt"]);
            return null;
        }
    }
}
