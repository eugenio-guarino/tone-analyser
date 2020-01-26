using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using ToneAnalyserFunction.Models.SentimentAnalysis;

namespace ToneAnalyserFunction
{
    class SentimentAnalysisCognitiveService : TextAnalyticsService
    {
        readonly string subscriptionKey;
        readonly string endpoint;

        public SentimentAnalysisCognitiveService(string speechSubscriptionKey, string endpoint)
        {
            this.subscriptionKey = speechSubscriptionKey;
            this.endpoint = endpoint;
        }

        public override string AnalyseText(string text)
        {
            ApiKeyServiceCredentials credentials = new ApiKeyServiceCredentials(subscriptionKey);
            TextAnalyticsClient client = new TextAnalyticsClient(credentials)
            {
                Endpoint = endpoint
            };

            var result = client.Sentiment(text, "en");

            double? score = result.Score;

            string analysisResult = (score != null) ? score.ToString() : "error";

            return analysisResult;
        }
    }
}