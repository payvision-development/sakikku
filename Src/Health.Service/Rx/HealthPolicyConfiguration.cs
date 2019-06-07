namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// <see cref="IHealthPolicyConfiguration"/> implementation using Rx.
    /// </summary>
    internal sealed class HealthPolicyConfiguration : BehaviorConfiguration<HealthCheckEntry, IHealthPolicyConfiguration>, IHealthPolicyConfiguration
    {
        private readonly string policyName;

        private readonly IHealthPolicy healthPolicy;

        public HealthPolicyConfiguration(string policyName, IHealthPolicy healthPolicy)
        {
            this.policyName = policyName;
            this.healthPolicy = healthPolicy;
        }

        private HealthCheckEntry ToEntry(HealthCheck healthCheck, TimeSpan interval) =>
            new HealthCheckEntry(this.policyName, healthCheck.Status, healthCheck.Message, interval);

        private static IObservable<HealthCheck> CatchUnhandledException(Exception exception, IScheduler scheduler) =>
            Observable.Return(new HealthCheck(HealthStatus.Unhealthy, exception.Message), scheduler);

        /// <inheritdoc />
        protected override IObservable<HealthCheckEntry> BuildSource(IScheduler scheduler, ICollection<IDisposable> compositeDisposable) =>
            Observable.FromAsync(this.healthPolicy.CheckAsync, scheduler)
            .Catch<HealthCheck, Exception>(x => CatchUnhandledException(x, scheduler))
                .TimeInterval(scheduler)
                .Where(x => x.Value != null)
                .Select(x => ToEntry(x.Value, x.Interval));

        /// <inheritdoc />
        protected override HealthCheckEntry TimeoutResponse(TimeSpan timeout, TimeoutException exception) =>
            HealthCheckEntry.Timeout(this.policyName, timeout);
    }
}