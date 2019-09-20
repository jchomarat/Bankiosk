using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskAPI.Models
{
    public class CustomerViewModel : Customer
    {
        List<Appointment> Appointments { get; set; }
        List<Document> Documents { get; set; }

    }


    public class Customer : TableEntity
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public long CardNumber { get; set; }
        public string Expiry { get; set; }
    }

    public class CustomerDB
    {
        public Storage DB { get; set; }

        public CustomerDB(string ConnectionString)
        {
            DB = new Storage(ConnectionString, "Customers");
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<Customer> query = new TableQuery<Customer>();

            // Print the fields for each customer.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<Customer> resultSegment = await DB.table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (Customer cust in resultSegment.Results)
                {
                    customers.Add(cust);
                }
            } while (token != null);

            return customers;
        }


        public async Task<CustomerAuth> GetCustomersByIDAsync(string CustomerID)
        {
            var customer = new Customer();
            TableContinuationToken token = null;

            var query = new TableQuery<Customer>().Where(TableQuery.CombineFilters(
TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Customer"),
   TableOperators.And,
TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, CustomerID)
           ));
            do
            {
                TableQuerySegment<Customer> resultSegment = await DB.table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (Customer cust in resultSegment.Results)
                {
                    customer = cust;
                }
            } while (token != null);

            return new CustomerAuth() { FirstName = customer.FirstName, Surname = customer.Surname, RowKey = customer.RowKey };
        }

        internal async Task<CustomerAuth> GetCustomerByCardNumberAsync(long cc_number)
        {
            // taking the top 1 in case you have > 1 records with the same cc number
            var query = new TableQuery<Customer>().Where(TableQuery.CombineFilters(
TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Customer"),
    TableOperators.And,
TableQuery.GenerateFilterConditionForLong("CardNumber", QueryComparisons.Equal, cc_number)
            )).Take(1);

            var customer = new Customer();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<Customer> resultSegment = await DB.table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (Customer cust in resultSegment.Results)
                {
                    customer = cust;

                    // also check for appointments

                    // also check for documents
                }
            } while (token != null);

            return new CustomerAuth() {FirstName = customer.FirstName, Surname = customer.Surname, RowKey = customer.RowKey};
        }


    }
}
