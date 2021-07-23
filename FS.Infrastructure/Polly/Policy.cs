using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;

namespace FS.Infrastructure.Polly
{
    internal static class Policy
    {
        internal static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.Redirect)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan
                .FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}