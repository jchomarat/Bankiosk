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
    public class AuditsController : ControllerBase
    {
        private string _storageConnectionString;

        public AuditsController(IOptions<AppSettings> appSettings)
        {
            _storageConnectionString = appSettings.Value.StorageConnectionString;
        }

        //GET api/audit/authenticates/actiontype (like faceAPI)
        [HttpGet("authenticates/{atype}")]
        public Task<List<Audit>> GetAuthtype(string atype)
        {
            var db = new AuditDB(_storageConnectionString);
            return db.GetType(atype, "Authenticate");
        }


        //GET api/audit/authenticates/actiontype (like faceAPI)
        [HttpGet("authenticates/add/{atype}")]
        public void Add_auth(string atype)
        {
            var db = new AuditDB(_storageConnectionString);
            db.CreateActionData(atype, "Authenticate");
        }

        //GET api/audit/action/actiontype (like complain)
        [HttpGet("action/{atype}")]
        public Task<List<Audit>> GetActiontype(string atype)
        {
            var db = new AuditDB(_storageConnectionString);
            return db.GetType(atype, "Action");
        }

        //GET api/audit/action/actiontype (like complain)
        [HttpGet("action/add/{atype}")]
        public void Add_act(string atype)
        {
            var db = new AuditDB(_storageConnectionString);
            db.CreateActionData(atype, "Action");
        }
    }
}
