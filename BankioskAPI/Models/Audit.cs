using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskAPI.Models
{
    public class Audit : TableEntity
    {
        public string ActionType { get; set; }

    }

    public class AuditDB
    {

        public Storage DB { get; set; }

        public AuditDB(string ConnectionString)
        {
            DB = new Storage(ConnectionString, "Audits");
        }

        public async void CreateActionData(string atype, string partkey)
        {

            var info = new Audit();
            info.PartitionKey = partkey;
            info.ActionType = atype;
            info.RowKey = Guid.NewGuid().ToString().ToUpper();


            TableOperation insertormerge = TableOperation.InsertOrMerge(info);
            await DB.table.ExecuteAsync(insertormerge);

        }

        public async Task<List<Audit>> GetType(string atype, string partkey)
        {
            var auditdata = new List<Audit>();
            TableContinuationToken token = null;

            var query = new TableQuery<Audit>().Where(TableQuery.CombineFilters(
TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partkey),
   TableOperators.And,
TableQuery.GenerateFilterCondition("ActionType", QueryComparisons.Equal, atype)));

            do
            {
                TableQuerySegment<Audit> resultSegment = await DB.table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (Audit act in resultSegment.Results)
                {
                    auditdata.Add(act);
                }
            } while (token != null);

            return auditdata;
        }

    }
}
