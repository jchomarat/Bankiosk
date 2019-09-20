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
    public class DocumentsController : Controller
    {

        private string _storageConnectionString;

        public DocumentsController(IOptions<AppSettings> appSettings)
        {
            _storageConnectionString = appSettings.Value.StorageConnectionString;
        }

        // GET: api/documents
        /// <summary>
        /// Get documents for that customer
        /// </summary>
        /// <param name="CustomerID">The customer RowKey</param>
        /// <returns></returns>
        [HttpGet("{CustomerID}")]
        [HttpGet("GetByRowKey/{CustomerID}")]
        public async Task<List<Document>> Get(string CustomerID)
        {
            var db = new DocumentDB(_storageConnectionString);
            return await db.GetDocumentAsync(CustomerID);
        }
    }
}
