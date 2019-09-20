using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskIoT.Models
{
    public class CreditCardForAuthentication
    {
        public string CardNumber { get; set; }
        public DateTimeOffset Expiration { get; set; }
    }
}
