using Microsoft.AspNetCore.Mvc;

namespace BankioskIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankioskBase : ControllerBase
    {

        /// <summary>
        /// Check if we have a person in proximity
        /// This may only work if we have GPIO
        /// </summary>
        /// <returns>true if someone is at proximity</returns>
        [HttpGet]
        public ActionResult<bool> GetIsThereSomeone()
        {
            return false;
        }


  
        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }
}
