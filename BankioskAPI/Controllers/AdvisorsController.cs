using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankioskAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BankioskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvisorsController : ControllerBase
    {
        private string _storageConnectionString;

        public AdvisorsController(IOptions<AppSettings> appSettings)
        {
            _storageConnectionString = appSettings.Value.StorageConnectionString;
        }

        // GET: api/advisors
        /// <summary>
        /// Gets details for the specified Advisor ID
        /// </summary>
        /// <param name="AdvisorID">The RowKey of the advisor</param>
        /// <returns></returns>
        [HttpGet("GetByRowKey/{AdvisorID}")]
        public async Task<AdvisorInfo> Get(string AdvisorID)
        {
            var db = new AdvisorDB(_storageConnectionString);
            return await db.GetAdvisorsAsync(AdvisorID);
        }

    }
}
