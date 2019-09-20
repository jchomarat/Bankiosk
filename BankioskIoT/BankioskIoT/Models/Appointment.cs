using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskIoT.Models
{
    public class Appointment
    {
        public string Subject { get; set; }

        public string Advisor { get; set; }

        public Advisor AdvisorDetails { get; set; }

        public DateTime AppointmentTime{ get; set; }
    }
}
