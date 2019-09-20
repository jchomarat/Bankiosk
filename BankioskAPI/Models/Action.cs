using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankioskAPI.Models
{
    public class Action : TableEntity
    {
    }
    public class ActionDB
    {
        public Storage DB { get; set; }

        public ActionDB(string ConnectionString)
        {
            DB = new Storage(ConnectionString, "Actions");
        }

        public async Task<List<Action>> GetActionsAsync()
        {
            var actions = new List<Action>();

            TableQuery<Action> query = new TableQuery<Action>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<Action> resultSegment = await DB.table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (Action act in resultSegment.Results)
                {
                    actions.Add(act);
                }
            } while (token != null);

            return actions;
        }


    }
    

}
