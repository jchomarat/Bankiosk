using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskIoT.Models
{
    /// <summary>
    /// Model for Action that the customer can performs on the kiosk. This list is stored in Azure
    /// </summary>
    public class Action
    {
        // The actual action name from the table storage on Azure
        public string RowKey { get; set; }
    }
}
