using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using BankioskIoT.Models;

namespace BankioskIoT.Controllers
{
    [Route("api/[controller]")]
    public class AuditController : Controller
    {
        private string _apiUrl;

        public AuditController(ApiSettings apiSettings)
        {
            _apiUrl = apiSettings.ApiUrl;
        }

        /// <summary>
        /// Add an authenticate row in the audit
        /// </summary>
        /// <param name="mode">Can be either faceAPI or cc_number</param>
        [Route("[action]/{mode}")]
        [HttpPost]
        public async void auditAuthentication(string mode)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_apiUrl);
            await client.GetAsync($"Audits/authenticates/add/{mode}");
        }

        /// <summary>
        /// Add an action selected row in the audit
        /// </summary>
        /// <param actionName="action">Any kind of actions supported</param>
        [Route("[action]/{actionName}")]
        [HttpPost]
        public async void auditAction(string actionName)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_apiUrl);
            await client.GetAsync($"Audits/action/add/{actionName}");
        }
    }
}
