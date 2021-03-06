using System;
using System.Collections.Generic;
using Jaeger.Core.Util;

namespace Jaeger.Core.Samplers
{
    /// <summary>
    /// Only used for unit testing
    /// </summary>
    internal interface IRateLimitingSampler : ISampler
    {
        double MaxTracesPerSecond { get; }
    }

    // RateLimitingSampler creates a sampler that samples at most maxTracesPerSecond. The distribution of sampled
    // traces follows burstiness of the service, i.e. a service with uniformly distributed requests will have those
    // requests sampled uniformly as well, but if requests are bursty, especially sub-second, then a number of
    // sequential requests can be sampled each second.
    public class RateLimitingSampler : IRateLimitingSampler
    {
        internal readonly IRateLimiter _rateLimiter;
        private readonly Dictionary<string, object> _tags;

        public double MaxTracesPerSecond { get; }

        public RateLimitingSampler(double maxTracesPerSecond)
            : this(maxTracesPerSecond, new RateLimiter(maxTracesPerSecond, Math.Max(maxTracesPerSecond, 1.0)))
        {}

        public RateLimitingSampler(double maxTracesPerSecond, IRateLimiter rateLimiter)
        {
            MaxTracesPerSecond = maxTracesPerSecond;

            _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
            _tags = new Dictionary<string, object> {
                { SamplerConstants.SamplerTypeTagKey, SamplerConstants.SamplerTypeRateLimiting },
                { SamplerConstants.SamplerParamTagKey, maxTracesPerSecond }
            };
        }

        public void Dispose()
        {
            // nothing to do
        }

        public (bool Sampled, Dictionary<string, object> Tags) IsSampled(TraceId id, string operation)
        {
            return (_rateLimiter.CheckCredit(1.0), _tags);
        }
    }
}
