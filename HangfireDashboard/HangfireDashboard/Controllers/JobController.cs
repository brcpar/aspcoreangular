using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDashboard.Controllers
{
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        [HttpGet]
        public IActionResult GetJobs([FromQuery]string query, [FromQuery]int pageNumber)
        {
            var dataSet = new List<Job>()
            {
                new Job(){JobId = 1, Name = "Job1", Status = "Success", Symbol = "blah"},
                new Job(){JobId = 2, Name = "Job2", Status = "Failed", Symbol = "blah"},
                new Job(){JobId = 3, Name = "Job3", Status = "Success", Symbol = "blah"},
                new Job(){JobId = 4, Name = "Job4", Status = "Success", Symbol = "blah"},
                new Job(){JobId = 5, Name = "Job5", Status = "Processing", Symbol = "blah"},
                new Job(){JobId = 6, Name = "Job6", Status = "Success", Symbol = "blah"},
                new Job(){JobId = 7, Name = "Job7", Status = "Success", Symbol = "blah"},
                new Job(){JobId = 8, Name = "Job8", Status = "Enqueued", Symbol = "blah"},
                new Job(){JobId = 9, Name = "Job9", Status = "Success", Symbol = "blah"},
                new Job(){JobId = 10, Name = "Job10", Status = "Success", Symbol = "blah"},
                new Job(){JobId = 11, Name = "Job11", Status = "Success", Symbol = "blah"},
            };
            if (!string.IsNullOrEmpty(query))
                dataSet = dataSet.Where(d => d.Name.Contains(query)).ToList();

            var retObj = new {items = dataSet.Skip(pageNumber * 5).Take(5), total_count = dataSet.Count};
            return Ok(retObj);
        }
    }

    public class Job
    {
        public string Name { get; set; }
        public int JobId { get; set; }
        public string Status { get; set; }
        public string Symbol { get; set; }
    }
}
