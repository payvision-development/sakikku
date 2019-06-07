namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// Reactive health policy collection implementation.
    /// </summary>
    internal sealed class HealthPolicyCollection : IHealthPolicyCollection, ISequenceBuilder<HealthCheckEntry>
    {
        private readonly Dictionary<string, HealthPolicyConfiguration> policies = 
            new Dictionary<string, HealthPolicyConfiguration>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc />
        public IHealthPolicyConfiguration Add(string policyName, IHealthPolicy healthPolicy)
        {
            if (string.IsNullOrWhiteSpace(policyName) || this.policies.ContainsKey(policyName))
            {
                throw new ArgumentException(nameof(policyName));
            }

            var policy = new HealthPolicyConfiguration(policyName, healthPolicy);
            this.policies.Add(policyName, policy);
            return policy;
        }

        /// <inheritdoc />
        public IObservable<HealthCheckEntry> Build(IScheduler scheduler, ICollection<IDisposable> compositeDisposable) =>
            this.policies.Values.Select(x => x.Build(scheduler, compositeDisposable)).Merge(scheduler);
    }
}