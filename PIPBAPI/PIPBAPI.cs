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
using PocketInvest.LoanResponseModels;


namespace PocketInvest.PIPBAPI
{
    public static class PIPBAPI
    {
         private static HttpClient client = new HttpClient();
        private static string jwtToken;

        [FunctionName("PIPBAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Authenticate with the API
            var username = Environment.GetEnvironmentVariable("PBUSN");
            var password = Environment.GetEnvironmentVariable("PBPWD");

            await Authenticate(username, password);

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
                jwtToken = data.access_token;
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
            var loansUrl = "https://api.peerberry.com/v1/loans";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, loansUrl));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Get loans process success");
                var responseBody = await response.Content.ReadAsStringAsync();
                LoanResponse loans = JsonConvert.DeserializeObject<LoanResponse>(responseBody);

                foreach (var loan in loans.Data)
                {
                    Console.WriteLine($@"LoanId: {loan.LoanId}
                    Country: {loan.Country}
                    InterestRate: {loan.InterestRate}
                    LoanOriginator: {loan.loanOriginator}
                    OriginatorId: {loan.originatorId}
                    IssuedDate: {loan.issuedDate}
                    FinalPaymentDate: {loan.finalPaymentDate}
                    TermType: {loan.termType}
                    Status: {loan.status}
                    RemainingTerm: {loan.remainingTerm}
                    InitialTerm: {loan.initialTerm}
                    LoanAmount: {loan.loanAmount}
                    AvailableToInvest: {loan.availableToInvest}
                    MinimumInvestmentAmount: {loan.minimumInvestmentAmount}
                    InvestedAmount: {loan.investedAmount}
                    Currency: {loan.currency}
                    Buyback: {loan.buyback}");
                }
                return loans;
            }
            else
            {
                Console.WriteLine("Get loans process failed");
                throw new Exception("Failed to get loans");
            }
        }
    }
}
