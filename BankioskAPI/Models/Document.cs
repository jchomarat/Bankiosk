using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskAPI.Models
{
    public class Document : TableEntity

    {

        public string Customer { get; set; }
        public string Description { get; set; }
        public bool Collected { get; set; }



    }

    public class DocumentDB
    {

        public Storage DB { get; set; }

        public DocumentDB(string ConnectionString)
        {
            DB = new Storage(ConnectionString, "Documents");
        }

        public async Task<List<Document>> GetDocumentAsync(string CustumerID)
        {
            var documents = new List<Document>();
            TableContinuationToken token = null;

            var query = new TableQuery<Document>().Where(TableQuery.CombineFilters(
TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Document"),
    TableOperators.And,
TableQuery.GenerateFilterCondition("Customer", QueryComparisons.Equal, CustumerID)
            ));

            do
            {
                TableQuerySegment<Document> resultSegment = await DB.table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (Document doc in resultSegment.Results)
                {
                    documents.Add(doc);
                }
            } while (token != null);

            return documents;

        }
    }





}
