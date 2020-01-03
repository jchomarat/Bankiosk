using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BankioskIoT.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<JObject>> FunFact()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_apiUrl);
            var byteContent = new StreamContent(Request.Body);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Request.ContentType));
            HttpResponseMessage response = await client.PostAsync("Photo/FunFact", byteContent);
            dynamic resp = new JObject();
            if (!response.IsSuccessStatusCode)
            {
                resp.status = false;
                resp.value = "Could not get fun fact from custom vision";
                Debug.WriteLine($"Error {response.StatusCode} ({response.ReasonPhrase})");
                return resp;
            }
            else
            {
                var funFacts = await response.Content.ReadAsAsync<List<PhotoFunFact>>();
                // Get the highest probability for those over 75%
                var goodProbabilities = funFacts.Where(f => f.Probability > 75);

                if (goodProbabilities.Count() > 0)
                {
                    // Get the highest tag found on the image
                    resp.status = true;
                    resp.value = JsonConvert.SerializeObject(
                        goodProbabilities.OrderByDescending(f => f.Probability).First()
                    );
                    return resp;
                }
                else
                {
                    // No good tags found, send nothing
                    resp.status = false;
                    resp.value = "This picture does not have trained tags";
                    return resp;
                }
            }
        }
    }
}