using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HangfireData
{
    public interface IHangfireSack : IDisposable
    {
        ILoggerFactory LogFactory { get; }
        IDatabaseService DatabaseService { get; }
    }
}
