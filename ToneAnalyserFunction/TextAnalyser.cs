using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Devices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace ToneAnalyserFunction
{
    public static class TextAnalyser
    {
        const string speechSubscriptionKey = "subkey";
        const string speechServiceRegion = "uksouth";

        const string IBMWatsonToneAnalyzerApiKey = "apikey";
        const string IBMWatsonToneAnalyzerApiEndpoint = "apiendpoint";

        const string MicrosoftSentimentAnalysisApiKey = "apikey";
        const string MicrosoftSentimentAnalysisApiEndpoint = "apiendpoint";

        const string deviceName = "devicename";

        [FunctionName("TextAnalyser")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Stream speechToAnalyse = req.Body;
            string serviceSelected = req.Query["service"];
            // Do speech-to-text conversion
            SpeechCognitiveService speechService = new SpeechCognitiveService(speechSubscriptionKey, speechServiceRegion);
            string textToAnalyse = await speechService.RecognizeSpeechAsync(speechToAnalyse, log);

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
            var serviceClient = ServiceClient.CreateFromConnectionString("iothubconnectionstring");
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
