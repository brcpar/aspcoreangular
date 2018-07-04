using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HangfireData;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDashboard.Controllers
{
    public abstract class BaseAPIController : Controller
    {
        protected readonly IHangfireSack _sack;
        protected BaseAPIController(IHangfireSack sack)
        {
            _sack = sack;
        }
    }
}
