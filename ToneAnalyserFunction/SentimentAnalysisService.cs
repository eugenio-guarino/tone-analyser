
namespace ToneAnalyserFunction
{
    abstract class SentimentAnalysisService
    {
        public abstract string ApiKey { get; set; }

        public abstract string ApiEndpoint { get; set; }

        public abstract string AnalyseText(string text);

    }
}
