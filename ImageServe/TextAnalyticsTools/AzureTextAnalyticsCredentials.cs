namespace TextAnalyticsTools
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Rest; 
    public class AzureTextAnalyticsCredentials : ServiceClientCredentials
    {
        private string subscriptionKey;

        public AzureTextAnalyticsCredentials(string subscriptionKey)
        {
            this.subscriptionKey = subscriptionKey;
        }
        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}