using BankioskIoT.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BankioskIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private string _apiUrl;

        public PhotoController(ApiSettings apiSettings)
        {
            _apiUrl = apiSettings.ApiUrl;
        }

        /// <summary>
        /// From the UI, the picture is posted to this service which will
        /// send it to the REST API and pass the result back
        /// </summary>        
        /// <returns>The person information after authentication</returns>
        [HttpPost]
        public async Task<ActionResult<JObject>> PostPictureAsync()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_apiUrl);
            var byteContent = new StreamContent(Request.Body);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Request.ContentType));
            HttpResponseMessage response = await client.PostAsync("Photo", byteContent);  
            dynamic resp = new JObject();
            if (!response.IsSuccessStatusCode)
            {
                resp.status = false;
                resp.value = "No user found in FaceAPI";
                Debug.WriteLine($"Error {response.StatusCode} ({response.ReasonPhrase})");
                return resp;
            }
            var id = await response.Content.ReadAsStringAsync();
            response = await client.GetAsync($"customers/getbyrowkey/{id}");
            if (!response.IsSuccessStatusCode)
            {
                resp.status = false;
                resp.value = "No user found in database";
                Debug.WriteLine($"Error {response.StatusCode} ({response.ReasonPhrase})");
                return resp;
            }
            var customer = await response.Content.ReadAsAsync<Customer>();
            resp.status = true;
            resp.value = JsonConvert.SerializeObject(customer);
            return resp;
        }

    }
}