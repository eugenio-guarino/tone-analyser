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
using ToneAnalyserFunction.Models.WatsonToneAnalyser;
using System.Collections.Generic;
using MoreLinq;
using System;

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
            string result;
            
            if (serviceSelected == "SentimentAnalysis")
            {
                textAnalyticsService = new SentimentAnalysisCognitiveService(MicrosoftSentimentAnalysisApiKey, MicrosoftSentimentAnalysisApiEndpoint);
                result = textAnalyticsService.AnalyseText(textToAnalyse);
            }
            else if (serviceSelected == "WatsonToneAnalyzer")
            {
                textAnalyticsService = new IBMWatsonToneAnalyzer(IBMWatsonToneAnalyzerApiKey, IBMWatsonToneAnalyzerApiEndpoint);
                result = await DeserializeWatsonToneAnalyzerJSON(textAnalyticsService.AnalyseText(textToAnalyse));
            }
            else
            {
                return new BadRequestObjectResult("You need to pick service");
            }

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

        public static async Task<string> DeserializeWatsonToneAnalyzerJSON(string result)
        {
            DocumentTone documentTone = JsonConvert.DeserializeObject<DocumentTone>(result);

            List<Tone> tones = documentTone.Tones;

            Tone tone = (Tone)tones.MaxBy(x => x.Score);

            result = tone.ToneName + "" + Math.Round(tone.Score * 100, 2);

            return result;
            
        }
    }
}
