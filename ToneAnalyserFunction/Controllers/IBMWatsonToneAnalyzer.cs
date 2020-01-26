using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.ToneAnalyzer.v3;
using IBM.Watson.ToneAnalyzer.v3.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ToneAnalyserFunction.Models.WatsonToneAnalyser;

namespace ToneAnalyserFunction
{
    public class IBMWatsonToneAnalyzer : IToneAnalyticsService
    {
        public string ApiKey { get ; set ; }
        public string ApiEndpoint { get ; set ; }

        public string AnalyseText(string text)
        {
            IamAuthenticator authenticator = new IamAuthenticator(apikey: ApiKey);

            ToneAnalyzerService toneAnalyzer = new ToneAnalyzerService("2017-09-21", authenticator);
            toneAnalyzer.SetServiceUrl(ApiEndpoint);

            ToneInput toneInput = new ToneInput()
            {
                Text = text
            };

            var result = toneAnalyzer.Tone(toneInput: toneInput);

            string deserializedResult = DeserializeResult(result.Response);

            return deserializedResult;
        }

        public string DeserializeResult(string result)
        {
            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(result);

            List<Tone> tones = rootObject.document_tone.tones;

            Tone highestScoringTone = tones.OrderByDescending(item => item.score).First();

            string deserializedResult = highestScoringTone.tone_name + " " + Math.Round(highestScoringTone.score * 100, 2) + "%";

            return deserializedResult;

        }
    }
}
