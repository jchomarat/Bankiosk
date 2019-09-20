using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskIoT.Models
{
    public class CreditCardAuthenticated
    {
        public bool IsAuthenticated { get; set; } = false;
        public int NumberOperation { get; set; } = -1;
        public byte PinRetryLeft { get; set; } = byte.MaxValue;
        public DateTimeOffset ExpirationDate { get; set; } = DateTimeOffset.MinValue;
    }
}
