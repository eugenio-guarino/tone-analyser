using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.ToneAnalyzer.v3;
using IBM.Watson.ToneAnalyzer.v3.Model;


namespace ToneAnalyserFunction
{
    class IBMWatsonToneAnalyzer : SentimentAnalysisService
    {
        public override string ApiKey { get; set; }
        public override string ApiEndpoint { get; set; }

        public override string AnalyseText(string text)
        {
            IamAuthenticator authenticator = new IamAuthenticator(apikey: ApiKey);

            ToneAnalyzerService toneAnalyzer = new ToneAnalyzerService("2017-09-21", authenticator);
            toneAnalyzer.SetServiceUrl(ApiEndpoint);

            ToneInput toneInput = new ToneInput()
            {
                Text = text
            };

            var result = toneAnalyzer.Tone(
                toneInput: toneInput
                );

            return result.Response;
        }
    }
}
