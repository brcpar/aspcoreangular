using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HangfireData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace HangfireDashboard
{
    public class WebHangfireSack : IHangfireSack
    {
        private readonly IHostingEnvironment _env;

        public WebHangfireSack(IDatabaseService dbService, ILoggerFactory loggerFact, IHostingEnvironment env)
        {
            DatabaseService = dbService;
            LogFactory = loggerFact;
            _env = env;
        }

        public bool InDev => _env.EnvironmentName.ToLower() == "development";

        public string EnvironmentName => _env.EnvironmentName;

        public ILoggerFactory LogFactory { get; }
        public IDatabaseService DatabaseService { get; }

        public void Dispose()
        {
            DatabaseService?.Dispose();
        }
    }
}
