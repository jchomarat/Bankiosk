using BankioskAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankioskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : Controller
    {

        private string _storageConnectionString;

        public AppointmentsController(IOptions<AppSettings> appSettings)
        {
            _storageConnectionString = appSettings.Value.StorageConnectionString;
        }

        // GET: api/appointments
        /// <summary>
        /// Get list of appointments for that customer
        /// </summary>
        /// <param name="CustomerID">The customer's RowKey</param>
        /// <returns></returns>
        [HttpGet("GetByRowKey/{CustomerID}")]
        public async Task<List<AppointmentInfo>> Get(string CustomerID)
        {
            var db = new AppointmentDB(_storageConnectionString);
            return await db.GetAppointmentAsync(CustomerID);
        }
    }
}