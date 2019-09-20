using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BM = BankioskIoT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCSC.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankioskIoT.Controllers
{
    [Route("api/[controller]")]
    public class BankTaskController : Controller
    {
        private string _apiUrl;

        public BankTaskController(BM.ApiSettings apiSettings)
        {
            _apiUrl = apiSettings.ApiUrl;
        }

        [Route("[action]/{CustomerId}")]
        [HttpGet]
        public async Task<JObject> Appointments(string CustomerId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_apiUrl);

            dynamic resp = new JObject();
            var apiResponse = await client.GetAsync($"Appointments/GetByRowKey/{CustomerId}");

            if (apiResponse.IsSuccessStatusCode)
            {
                var apiAppointments = await apiResponse.Content.ReadAsAsync<List<BM.Appointment>>();

                // Load advisor info into the object
                foreach (var appointment in apiAppointments)
                {
                    var advResponse = await client.GetAsync($"Advisors/GetByRowKey/{appointment.Advisor}");
                    appointment.AdvisorDetails = await advResponse.Content.ReadAsAsync<BM.Advisor>();
                }

                resp.status = true;
                resp.value = new JArray(
                    apiAppointments.Select(a => JsonConvert.SerializeObject(a))
                );

            }
            else
            {
                resp.status = false;
                resp.value = "Could not retrieve appointments list";
            }
            return resp;
        }

        [Route("[action]/{CustomerId}")]
        [HttpGet]
        public async Task<JObject> Documents(string CustomerId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_apiUrl);

            dynamic resp = new JObject();
            var apiResponse = await client.GetAsync($"Documents/GetByRowKey/{CustomerId}");

            if (apiResponse.IsSuccessStatusCode)
            {
                var apiAppointments = await apiResponse.Content.ReadAsAsync<List<BM.Document>>();

                resp.status = true;
                resp.value = new JArray(
                    apiAppointments.Select(a => a.Description)
                );

            }
            else
            {
                resp.status = false;
                resp.value = "Could not retrieve appointments list";
            }
            return resp;
        }
    }
}
