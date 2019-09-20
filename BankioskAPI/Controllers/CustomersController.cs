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
    public class CustomersController : ControllerBase
    {
        private string _storageConnectionString;

        /// <summary>
        /// initializer
        /// </summary>
        /// <param name="appSettings"></param>
        public CustomersController(IOptions<AppSettings> appSettings)
        {
            _storageConnectionString = appSettings.Value.StorageConnectionString;
        }

        // GET: api/customers
        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<Customer>> Get()
        {
            var db = new CustomerDB(_storageConnectionString);
            return await db.GetCustomersAsync();
        }

        // GET: api/customers/5
        /// <summary>
        /// Retrieve a specific customer
        /// </summary>
        /// <param name="RowKey">The customer's RowKey</param>
        /// <returns></returns>
        [HttpGet("GetByRowKey/{RowKey}")]
        public async Task<CustomerAuth> Get(string RowKey)
        {
            var db = new CustomerDB(_storageConnectionString);
            return await db.GetCustomersByIDAsync(RowKey);
        }

        // GET: api/Customers/5217646340285558
        /// <summary>
        /// Get the customer name and surname for a given credit card number
        /// </summary>
        /// <param name="ccnumber">Credit card number</param>
        /// <returns></returns>
        [HttpGet("GetByCard/{ccnumber}")]
        public async Task<CustomerAuth> GetByCard(long ccnumber)
        {
            var db = new CustomerDB(_storageConnectionString);
            return await db.GetCustomerByCardNumberAsync(ccnumber);
        }

    }
}
