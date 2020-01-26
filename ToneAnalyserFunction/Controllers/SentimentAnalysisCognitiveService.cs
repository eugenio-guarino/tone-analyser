using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using ToneAnalyserFunction.Models.SentimentAnalysis;

namespace ToneAnalyserFunction
{
    class SentimentAnalysisCognitiveService : ITextAnalyticsService
    {
        public string ApiKey { get ; set ; }
        public string ApiEndpoint { get ; set ; }

        public string AnalyseText(string text)
        {
            ApiKeyServiceCredentials credentials = new ApiKeyServiceCredentials(ApiKey);
            TextAnalyticsClient client = new TextAnalyticsClient(credentials)
            {
                Endpoint = ApiEndpoint
            };

            var result = client.Sentiment(text, "en");

            double? score = result.Score;

            string analysisResult = (score != null) ? score.ToString() : "error";

            return analysisResult;
        }
    }
}