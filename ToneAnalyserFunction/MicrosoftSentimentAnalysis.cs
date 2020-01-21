using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json;

namespace ToneAnalyserFunction
{
    class MicrosoftSentimentAnalysis
    {
        readonly string subscriptionKey;
        readonly string endpoint;

        public MicrosoftSentimentAnalysis(string speechSubscriptionKey, string endpoint)
        {
            this.subscriptionKey = speechSubscriptionKey;
            this.endpoint = endpoint;
        }

        public string AnalyseText(string text)
        {
            HttpClient client = new HttpClient();

            var requestData = new StringContent(JsonConvert.SerializeObject( new { language = "en", id = 1, text = text }), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = client.PostAsync(endpoint, requestData);
            if (response.Result.Content != null)
            {
                var responseContent = response.Result.Content.ReadAsStringAsync();

                return responseContent.ToString();
            }
            return "";
        }
    }
}