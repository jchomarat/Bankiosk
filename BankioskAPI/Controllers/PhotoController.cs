using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BankioskAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Extensions.Options;

namespace BankioskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        //private const string CustomerGroupId = "customer";
        private readonly IFaceClient _faceClient;
        private readonly string _customerGroupId;

        public PhotoController(IOptions<AppSettings> appSettings)
        {

            _faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(appSettings.Value.FaceKey),
            new System.Net.Http.DelegatingHandler[] { });
            _faceClient.Endpoint = appSettings.Value.FaceEndPoint;
            _customerGroupId = appSettings.Value.FaceGroupID;
    }

        /// <summary>
        /// From the UI, the picture is posted to this service which will
        /// send it to the REST API and pass the result back
        /// </summary>        
        /// <returns>The person information after authentication</returns>
        [HttpPost]
        public async Task<ActionResult<string>> PostPictureAsync()
        {
            {
                var faces = await _faceClient.Face.DetectWithStreamAsync(Request.Body);
                var faceIds = faces.Select(face => face.FaceId).Select(g => g ?? Guid.Empty).ToList();
                var results = await _faceClient.Face.IdentifyAsync(faceIds, _customerGroupId);
                foreach (var identifyResult in results)
                {

                    if (identifyResult.Candidates.Count > 0)
                    {
                        // Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = await _faceClient.PersonGroupPerson.GetAsync(_customerGroupId, candidateId);
                        return person.UserData.ToString();
                    }
                }
            }
            return "";
        }
    }
}