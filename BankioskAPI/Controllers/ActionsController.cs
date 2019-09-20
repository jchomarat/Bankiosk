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
    public class ActionsController : ControllerBase
    {
        private string _storageConnectionString;

        public ActionsController(IOptions<AppSettings> appSettings)
        {
            _storageConnectionString = appSettings.Value.StorageConnectionString;
        }

        // GET: api/actions
        /// <summary>
        /// Get a list of actions that can be performed in the branch
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<Models.Action>> GetActionsAsync()
        {
            var db = new ActionDB(_storageConnectionString);
            return await db.GetActionsAsync();
        }

    }
}
