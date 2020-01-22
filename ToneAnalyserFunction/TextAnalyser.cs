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

            // Do speech-to-text conversion
            SpeechCognitiveService speechService = new SpeechCognitiveService(speechSubscriptionKey, speechServiceRegion);
            string textToAnalyse = await speechService.RecognizeSpeechAsync(req.Body, log);

            // Choose which text analytics service to use
            TextAnalyticsService textAnalyticsService;
            if (serviceSelected == "SentimentAnalysis")
            {
                textAnalyticsService = new SentimentAnalysisCognitiveService(MicrosoftSentimentAnalysisApiKey, MicrosoftSentimentAnalysisApiEndpoint);
            }
            else if (serviceSelected == "WatsonToneAnalyzer")
            {
                textAnalyticsService = new IBMWatsonToneAnalyzer(IBMWatsonToneAnalyzerApiKey, IBMWatsonToneAnalyzerApiEndpoint);
            }
            else
            {
                return new BadRequestObjectResult("You need to pick service");
            }

            string requestBody = textAnalyticsService.AnalyseText(textToAnalyse);
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            return data != null
                ? (ActionResult)new OkObjectResult($"{data}")
                : new BadRequestObjectResult("Whoops, something went wrong!");
        }
    }
}
