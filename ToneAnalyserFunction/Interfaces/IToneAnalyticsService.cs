namespace ToneAnalyserFunction
{
    interface IToneAnalyticsService
    {
        public string ApiKey { get; set; }
        public string ApiEndpoint { get; set; }
        public string AnalyseText(string text);
    }
}
