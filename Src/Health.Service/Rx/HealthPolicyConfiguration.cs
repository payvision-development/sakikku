namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// <see cref="IHealthPolicyConfiguration"/> implementation using Rx.
    /// </summary>
    internal sealed class HealthPolicyConfiguration : IHealthPolicyConfiguration
    {
        private readonly string policyName;

        private readonly IHealthPolicy healthPolicy;

        private TimeSpan? pollingInterval;

        public HealthPolicyConfiguration(string policyName, IHealthPolicy healthPolicy)
        {
            this.policyName = policyName;
            this.healthPolicy = healthPolicy;
        }

        /// <inheritdoc />
        public IHealthPolicyConfiguration Buffer(TimeSpan interval)
        {
            this.pollingInterval = interval;
            return this;
        }

        /// <summary>
        /// Builds the configured health policy as a reactive sequence.
        /// </summary>
        /// <param name="scheduler">The scheduler used by the sequence.</param>
        /// <returns>The observable health check entry sequence.</returns>
        internal IObservable<HealthCheckEntry> Build(IScheduler scheduler, ICollection<IDisposable> disposables)
        {
            var source = this.BuildSource(scheduler);
            if (this.pollingInterval.HasValue)
            {
                var buffer = new PollingObservable<HealthCheckEntry>(source, this.pollingInterval.Value, scheduler);
                disposables.Add(buffer);
                source = buffer;
            }

            return source;
        }

        private IObservable<HealthCheckEntry> BuildSource(IScheduler scheduler) =>
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
