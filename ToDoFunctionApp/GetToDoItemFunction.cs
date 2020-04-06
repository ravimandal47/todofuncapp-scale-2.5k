using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;

namespace ToDoFunctionApp
{
    public static class GetToDoItemFunction
    {
        [FunctionName(nameof(GetToDoItemFunction))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{userId}")] HttpRequest req, string userId,
            [CosmosDB(databaseName: Constants.DBName, collectionName: Constants.CollectionName, ConnectionStringSetting = Constants.ConnectionStringSettingsName)] IDocumentClient documentClient,
            ILogger log)
        {
            log.LogInformation($"{nameof(GetToDoItemFunction)} function processed a request.");

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.DBName, Constants.CollectionName);

            IDocumentQuery<ToDoItem> query = documentClient.CreateDocumentQuery<ToDoItem>(collectionUri, new FeedOptions
            {
                PartitionKey = new PartitionKey(userId)

            }).AsDocumentQuery();

            var items = new List<ToDoItem>();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<ToDoItem>();
                items.AddRange(result);
            }

            return new OkObjectResult(items);
        }
    }
}
