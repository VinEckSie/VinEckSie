using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;


namespace PocketInvest.PBAPI
{
    public static class PIPBAPI
    {
         private static HttpClient client = new HttpClient();
        private static string jwtToken;

    [FunctionName("PIPBAPI")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "options", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        // Authenticate with the API
        // var username = Environment.GetEnvironmentVariable("vesiv@proton.me");
        // var password = Environment.GetEnvironmentVariable("UUeK&3iaFaAr$T-J");
        var username = "vesiv@proton.me";
        var passwordVar = "UUeK&3iaFaAr$T-J";
        await Authenticate(username, passwordVar);

        // Get the list of loans
        var loans = await GetLoans();

        return new OkObjectResult(loans);
    }

    private static async Task Authenticate(string username, string _password)
    {
        var loginUrl = "https://api.peerberry.com/v1/investor/login";
        var payload = new { email = username, password = _password };
        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        // Print the request payload
        Console.WriteLine($"Request payload: {JsonConvert.SerializeObject(payload)}");

        var response = await client.PostAsync(loginUrl, content);   

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(responseBody);
            jwtToken = data?.childrenTokens[0];
        }
        else
        {
            // Print the response status and content
            Console.WriteLine($"Response status: {response.StatusCode}");
            Console.WriteLine($"Response content: {await response.Content.ReadAsStringAsync()}");

            var contentString = await content.ReadAsStringAsync();
            throw new Exception("Authentication failed. Content: " + contentString);
        }
    }

    private static async Task<dynamic> GetLoans()
    {
        Console.WriteLine("Get loans start process");
        // var loansUrl = "https://api.peerberry.com/v1/loans?sort=-loanId&offset=0&pageSize=40";
        var loansUrl = "https://api.peerberry.com/v1/loans";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Options, loansUrl));

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Get loans process success");
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
            dynamic loans = JsonConvert.DeserializeObject(responseBody);
            Console.WriteLine(loans.toString());
            return loans;
        }
        else
        {
            Console.WriteLine("Get loans process failed");
            throw new Exception("Failed to get loans");
        }
    }
        // [FunctionName("PIPBAPI")]
        // public static async Task<IActionResult> Run(
        //     [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        //     ILogger log)
        // {
        //     log.LogInformation("C# HTTP trigger function processed a request.");

        //     string name = req.Query["name"];

        //     string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //     dynamic data = JsonConvert.DeserializeObject(requestBody);
        //     name = name ?? data?.name;

        //     string responseMessage = string.IsNullOrEmpty(name)
        //         ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //         : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //     return new OkObjectResult(responseMessage);
        // }
    }
}
