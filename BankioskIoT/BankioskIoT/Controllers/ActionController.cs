using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BM = BankioskIoT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankioskIoT.Controllers
{
    [Route("api/[controller]")]
    public class ActionController : Controller
    {
        private string _apiUrl;

        public ActionController(BM.ApiSettings apiSettings)
        {
            _apiUrl = apiSettings.ApiUrl;
        }
        
        [HttpGet]
        public async Task<JObject> Get()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_apiUrl);

            dynamic resp = new JObject();
            var apiResponse = await client.GetAsync("Actions");

            if (apiResponse.IsSuccessStatusCode)
            {
                var apiActions = await apiResponse.Content.ReadAsAsync<List<BM.Action>>();
                resp.status = true;
                resp.value = new JArray(
                    apiActions.Select(a => a.RowKey)
                );

            }
            else
            {
                resp.status = false;
                resp.value = "Could not retrieve actions list to peform.";
            }
            return resp;
        }
    }
}
