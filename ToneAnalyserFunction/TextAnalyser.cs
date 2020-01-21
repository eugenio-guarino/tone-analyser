using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ToneAnalyserFunction
{
    public static class TextAnalyser
    {
        [FunctionName("TextAnalyser")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            const string speechSubscriptionKey = "";
            const string speechServiceRegion = "";

            const string IBMWatsonToneAnalyzerApiKey = "";
            const string IBMWatsonToneAnalyzerApiEndpoint = "";

            const string MicrosoftSentimentAnalysisApiKey = "";
            const string MicrosoftSentimentAnalysisApiEndpoint = "";

            string serviceSelected = req.Query["service"];

            SpeechCognitiveService speechService = new SpeechCognitiveService(speechSubscriptionKey, speechServiceRegion);
            string textToAnalyse = await speechService.RecognizeSpeechAsync(req.Body, log);

            TextAnalyticsService textAnalyticsService = null;

            if (serviceSelected == "TextAnalytics")
            {
                textAnalyticsService = new SentimentAnalysisCognitiveService(MicrosoftSentimentAnalysisApiKey, MicrosoftSentimentAnalysisApiEndpoint);
            }
            else
            {
                textAnalyticsService = new IBMWatsonToneAnalyzer(IBMWatsonToneAnalyzerApiKey, IBMWatsonToneAnalyzerApiEndpoint);
            }

            string requestBody = textAnalyticsService.AnalyseText(textToAnalyse);
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            return data != null
                ? (ActionResult)new OkObjectResult($"{data}")
                : new BadRequestObjectResult("Choose either 'WatsonAnalyser' or 'TextAnalytics' for the service query string value.");
        }
    }
}
