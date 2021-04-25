using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace HmServiceCache.Client.RetryPolicies
{
    public class ForeverRetryPolicy : IRetryPolicy
    {
        private readonly TimeSpan timeSpan;

        public ForeverRetryPolicy(int miliseconds)
        {
            this.timeSpan = TimeSpan.FromMilliseconds(miliseconds);
        }

        public ForeverRetryPolicy(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
        }

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return timeSpan;
        }
    }
}
