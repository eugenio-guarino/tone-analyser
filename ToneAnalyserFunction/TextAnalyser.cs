using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Devices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace ToneAnalyserFunction
{
    public static class TextAnalyser
    {
        // Microsoft Speech Cognitive Service 
        const string speechSubscriptionKey = "";
        const string speechServiceRegion = "uksouth";

        // IBM Watson Tone Analyzer Service
        const string IBMWatsonToneAnalyzerApiKey = "";
        const string IBMWatsonToneAnalyzerApiEndpoint = "";

        // Microsoft Text Analytics Cognitive Service
        const string MicrosoftSentimentAnalysisApiKey = "";
        const string MicrosoftSentimentAnalysisApiEndpoint = "";

        // Microsoft Azure IoT Hub
        const string deviceName = "MyNodeDevice";
        const string connectionString = "HostName=;SharedAccessKeyName=;SharedAccessKey=";

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

            IToneAnalyticsService textAnalyticsService;
            
            if (serviceSelected == "Sentiment")
                textAnalyticsService = new SentimentAnalysisCognitiveService { ApiKey = MicrosoftSentimentAnalysisApiKey, ApiEndpoint = MicrosoftSentimentAnalysisApiEndpoint};
            else if (serviceSelected == "Emotion")
                textAnalyticsService = new IBMWatsonToneAnalyzer { ApiKey = IBMWatsonToneAnalyzerApiKey, ApiEndpoint = IBMWatsonToneAnalyzerApiEndpoint};               
            else
                return new BadRequestObjectResult("You need to pick service");
            
            // Retrieve result from the selected service's analysis
            string result = textAnalyticsService.AnalyseText(textToAnalyse);
            log.LogInformation(result);

            // Send translation result as C2D message to IoT Hub
            var serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
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
