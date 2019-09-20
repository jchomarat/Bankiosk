using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskAPI.Models
{
    public class Appointment : TableEntity
    {
        public string Subject { get; set; }
        public string Advisor { get; set; }
        public DateTime AppointmentTime { get; set; }
    }

    public class AppointmentDB
    {
        public Storage DB { get; set; }

        public AppointmentDB(string ConnectionString)
        {
            DB = new Storage(ConnectionString, "Appointments");
        }

        public async Task<List<AppointmentInfo>> GetAppointmentAsync(string CustomerID)
        {
            var appointments = new List<AppointmentInfo>();
            TableContinuationToken token = null;

            var query = new TableQuery<Appointment>().Where(TableQuery.CombineFilters(
TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Appointment"),
    TableOperators.And,
TableQuery.GenerateFilterCondition("Customer", QueryComparisons.Equal, CustomerID)
            ));

            do
            {
                TableQuerySegment<Appointment> resultSegment = await DB.table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (Appointment app in resultSegment.Results)
                {
                    appointments.Add(new AppointmentInfo() { Subject = app.Subject, Advisor = app.Advisor, AppointmentTime = app.AppointmentTime });
                }
            } while (token != null);

            return appointments;

        }
    }
}
