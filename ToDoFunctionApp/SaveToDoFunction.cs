using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ToDoFunctionApp
{
    public static class SaveToDoFunction
    {
        [FunctionName(nameof(SaveToDoFunction))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [CosmosDB( databaseName: Constants.DBName, collectionName: Constants.CollectionName, ConnectionStringSetting = Constants.ConnectionStringSettingsName)] IAsyncCollector<ToDoItem> asyncCollector,
            ILogger log)
        {
            log.LogInformation($"{nameof(SaveToDoFunction)} function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ToDoItem toDo = JsonConvert.DeserializeObject<ToDoItem>(requestBody);

            await asyncCollector.AddAsync(toDo);

            return new OkObjectResult(new { message = "Added new ToDo", status = 200});
        }
    }
}
