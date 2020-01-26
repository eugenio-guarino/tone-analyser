using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using ToneAnalyserFunction.Models.SentimentAnalysis;

namespace ToneAnalyserFunction
{
    class SentimentAnalysisCognitiveService : IToneAnalyticsService
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

            string convertedResult;

            // This will be changed when the new Text Analytics Client Library v3 comes out from pre-release
            if (result.Score >= 0.60)
                convertedResult = "Positive :-)";
            else if (result.Score >= 0.45)
                convertedResult = "Neutral :-|";
            else
                convertedResult = "Negative :-(";

            return convertedResult;
        }
    }
}