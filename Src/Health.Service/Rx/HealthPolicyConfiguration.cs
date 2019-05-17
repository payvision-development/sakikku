namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// <see cref="IHealthPolicyConfiguration"/> implementation using Rx.
    /// </summary>
    internal sealed class HealthPolicyConfiguration : IHealthPolicyConfiguration
    {
        private readonly string policyName;

        private readonly IHealthPolicy healthPolicy;

        public HealthPolicyConfiguration(string policyName, IHealthPolicy healthPolicy)
        {
            this.policyName = policyName;
            this.healthPolicy = healthPolicy;
        }

        /// <summary>
        /// Builds the configured health policy as a reactive sequence.
        /// </summary>
        /// <param name="scheduler">The scheduler used by the sequence.</param>
        /// <returns>The observable health check entry sequence.</returns>
        internal IObservable<HealthCheckEntry> Build(IScheduler scheduler) =>
            Observable.FromAsync(this.healthPolicy.CheckAsync, scheduler)
            .Catch<HealthCheck, Exception>(x => CatchUnhandledException(x, scheduler))
                .TimeInterval(scheduler)
                .Where(x => x.Value != null)
                .Select(x => ToEntry(x.Value, x.Interval));

        private HealthCheckEntry ToEntry(HealthCheck healthCheck, TimeSpan interval) =>
            new HealthCheckEntry(this.policyName, healthCheck.Status, healthCheck.Message, interval);

        private static IObservable<HealthCheck> CatchUnhandledException(Exception exception, IScheduler scheduler) =>
            Observable.Return(new HealthCheck(HealthStatus.Unhealthy, exception.Message), scheduler);
    }
}
