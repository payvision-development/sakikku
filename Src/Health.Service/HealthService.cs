namespace Payvision.Diagnostics.Health
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    using Payvision.Diagnostics.Health.Rx;

    /// <summary>
    /// Starting point from which create new health services.
    /// </summary>
    public sealed class HealthService : IHealthServiceBuilder
    {
        private readonly HealthPolicyCollection policies = new HealthPolicyCollection();

        private IScheduler? scheduler;

        private HealthService()
        {
        }

        /// <summary>
        /// Creates a new health service builder that will use the health policies as specified
        /// in the given configuration callback.
        /// </summary>
        /// <param name="configure">The health policies configuration callback.</param>
        /// <returns>The <see cref="IHealthServiceBuilder"/> instance.</returns>
        public static IHealthServiceBuilder Create(Action<IHealthPolicyCollection> configure)
        {
            var builder = new HealthService();
            configure(builder.policies);
            return builder;
        }

        /// <inheritdoc />
        public IHealthService Build()
        {
            var scheduler = this.scheduler ?? Scheduler.Default;
            IObservable<HealthReport> source = this.policies.Build(scheduler)
                .ToList()
                .TimeInterval(scheduler)
                .Select(x => new HealthReport(x.Value, x.Interval));
            return new ReactiveHealthService(source);
        }
    }
}
