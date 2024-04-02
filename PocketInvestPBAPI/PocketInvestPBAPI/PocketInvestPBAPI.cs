using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PocketInvestPBAPI
{
    public static class PocketInvestPBAPI
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("PocketInvestPBAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // log.LogInformation("C# HTTP trigger function processed a request.");

            // string name = req.Query["name"];  

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            // string responseMessage = string.IsNullOrEmpty(name)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"Hello, {name}. This HTTP triggered function executed successfully.";

            // Get all endpoints for the URL "https://api.peerberry.com"
            string url = "https://api.peerberry.com/v2";
            HttpResponseMessage httpResponse = await httpClient.GetAsync(url);
            string responseMessage = ""; // Declare the responseMessage variable
            if (httpResponse.IsSuccessStatusCode)
            {
                string endpoints = await httpResponse.Content.ReadAsStringAsync();
                responseMessage += $"\n\nEndpoints for {url}:\n{endpoints}";
            }
            else
            {
                responseMessage += $"\n\nFailed to retrieve endpoints for {url}.";
            }

            return new OkObjectResult(responseMessage);
        }
    }
}

