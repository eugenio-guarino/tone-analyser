using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Devices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

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

            const string deviceName = "MyNodeDevice";

            string serviceSelected = req.Query["service"];

            // Do speech-to-text conversion
            SpeechCognitiveService speechService = new SpeechCognitiveService(speechSubscriptionKey, speechServiceRegion);
            string textToAnalyse = await speechService.RecognizeSpeechAsync(req.Body, log);

            log.LogInformation("Speech-To-Text result: " + textToAnalyse);

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

            string result = textAnalyticsService.AnalyseText(textToAnalyse);

            log.LogInformation("Analysis result: " + result);

            // Send translation result as C2D message
            string iothubConnectionString = System.Environment.GetEnvironmentVariable("iotHubConnectionString");
            var serviceClient = ServiceClient.CreateFromConnectionString(iothubConnectionString);
            var commandMessage = new Message(Encoding.ASCII.GetBytes(result));
            await serviceClient.SendAsync(deviceName, commandMessage);


            if (result != null)
            {
                return new OkObjectResult("Analysis result: " + result);
            }
            else
            {
                return new BadRequestObjectResult("Failed to analyze speech.");
            }
        }
    }
}
