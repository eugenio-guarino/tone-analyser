using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.ToneAnalyzer.v3;
using IBM.Watson.ToneAnalyzer.v3.Model;


namespace ToneAnalyserFunction
{
    public class IBMWatsonToneAnalyzer : TextAnalyticsService
    {

        readonly string key;
        readonly string endpoint;

        public IBMWatsonToneAnalyzer(string ApiKey, string ApiEndpoint)
        {
            this.key = ApiKey;
            this.endpoint = ApiEndpoint;

        }

        public override string AnalyseText(string text)
        {
            IamAuthenticator authenticator = new IamAuthenticator(apikey: key);

            ToneAnalyzerService toneAnalyzer = new ToneAnalyzerService("2017-09-21", authenticator);
            toneAnalyzer.SetServiceUrl(endpoint);

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
