using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ToneAnalyserFunction
{
    public static class TextAnalyser
    {
        [FunctionName("TextAnalyser")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string service = req.Query["service"];
            string text = req.Query["text"];

            SentimentAnalysisService textAnalyticsService = null;

            if (service == "TextAnalytics")
            {

            }
            else if (service == "WatsonAnalyser")
            {
                textAnalyticsService = new IBMWatsonToneAnalyzer
                {
                    ApiKey = "",
                    ApiEndpoint = ""
                };
            }

            string requestBody = textAnalyticsService.AnalyseText(text);
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            return data != null
                ? (ActionResult)new OkObjectResult($"{data}")
                : new BadRequestObjectResult("Choose either 'WatsonAnalyser' or 'TextAnalytics' for the service query string value.");
        }
    }
}
