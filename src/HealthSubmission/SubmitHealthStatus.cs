using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using System.Collections.Generic;

namespace app_plat_workshop
{
    public static class SubmitHealthStatus
    {
        [FunctionName(nameof(SubmitHealthStatus))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "healthhistory")] HealthSubmissionDto submission,
                        [CosmosDB(
                databaseName: "ContosoHealthcheck",
                collectionName: "Submissions",
                ConnectionStringSetting = "DefaultConnection")]
                IAsyncCollector<HealthSubmission> healthHistory,
            ILogger log)
        {

            await healthHistory.AddAsync(new HealthSubmission()
            {
                Status = submission.Status,
                SubmittedOn = DateTimeOffset.UtcNow,
                UserId = submission.UserId
            });

            return new OkResult();
        }
    }

    public static class GetMyHealthHistory
    {
        [FunctionName(nameof(GetMyHealthHistory))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "healthhistory/{userId}")] HttpRequest req,
            string userId,
                        [CosmosDB(
                databaseName: "ContosoHealthcheck",
                collectionName: "Submissions",
                ConnectionStringSetting = "DefaultConnection")] DocumentClient healthHistory,
            ILogger log)
        {

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new NotFoundResult();
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("ContosoHealthcheck", "Submissions");

            IDocumentQuery<HealthSubmission> query = healthHistory.CreateDocumentQuery<HealthSubmission>(collectionUri)
                    .Where(p => p.UserId == userId)
                    .AsDocumentQuery();

            var myHealthHistory = new List<HealthSubmission>();

            while (query.HasMoreResults)
            {
                foreach (HealthSubmission result in await query.ExecuteNextAsync())
                {
                    myHealthHistory.Add(result);
                }
            }

            return new OkObjectResult(myHealthHistory);
        }
    }

    

    public static class GetMyHealthSubmissionDetails
    {
        [FunctionName(nameof(GetMyHealthSubmissionDetails))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "healthhistory/{userId}/{submissionId}")] HttpRequest req,
                        [CosmosDB(
                databaseName: "ContosoHealthcheck",
                collectionName: "Submissions",
                ConnectionStringSetting = "DefaultConnection",
                Id = "{submissionId}",
                PartitionKey = "{userId}")] HealthSubmission submission)
        {
            if (submission == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(submission);
        }
    }
}
