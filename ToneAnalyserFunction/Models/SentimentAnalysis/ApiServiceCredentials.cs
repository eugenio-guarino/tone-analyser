using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToneAnalyserFunction.Models.SentimentAnalysis
{
    class ApiKeyServiceCredentials : ServiceClientCredentials
    {
        private readonly string apiKey;

        public ApiKeyServiceCredentials(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            request.Headers.Add("Ocp-Apim-Subscription-Key", this.apiKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}
