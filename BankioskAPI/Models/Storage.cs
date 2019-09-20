using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskAPI.Models
{
    public class Storage
    {


        // get connection string from WebConfig or KeyVault
        public CloudStorageAccount storageAccount { get; set; }
        public CloudTableClient tableClient { get; set; }
        public CloudTable table { get; set; }

        public Storage(string connectionString, string tableName)
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(tableName);

            // make sure the table is there
            CreateTableAsync();

        }

        public async Task InsertAsync<T>(T t) where T : TableEntity
        {
            // insert a row into the table
            TableOperation insert = TableOperation.Insert(t);
            await table.ExecuteAsync(insert);
        }
       
        public void RemoveById(string id)
        {
            //CloudTable table = tableClient.GetTableReference(tableName);
            //table.CreateIfNotExists();

            //throw new NotImplementedException();
        }

        async void CreateTableAsync()
        {
            // Create the CloudTable if it does not exist
            await table.CreateIfNotExistsAsync();
        }

    }
}
