using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskAPI.Models
{
    public class AppointmentInfo
    {
        public string Subject { get; set; }
        public string Advisor { get; set; }
        public DateTime AppointmentTime { get; set; }
    }
}
