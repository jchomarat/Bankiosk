using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskAPI.Models
{
    public class Advisor : TableEntity
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime TimeStamp { get; set; }
    }
    
    public class AdvisorDB
    {

        public Storage DB { get; set; }

        public AdvisorDB(string ConnectionString)
        {
            DB = new Storage(ConnectionString, "Advisors");
        }

        public async Task<AdvisorInfo> GetAdvisorsAsync(string AdvisorID)
        {
            var advisor = new Advisor();
            TableContinuationToken token = null;

            var query = new TableQuery<Advisor>().Where(TableQuery.CombineFilters(
TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Advisor"),
    TableOperators.And,
TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, AdvisorID)
            ));
            do
            {
                TableQuerySegment<Advisor> resultSegment = await DB.table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (Advisor adv in resultSegment.Results)
                {
                    advisor = adv;
                }
            } while (token != null);

            return new AdvisorInfo() { FirstName = advisor.FirstName, Surname = advisor.Surname, Email = advisor.Email };
        }
    }
}
