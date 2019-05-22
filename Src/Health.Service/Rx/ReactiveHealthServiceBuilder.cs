namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    /// <summary>
    /// Builder factory of <see cref="ReactiveHealthService"/> instances.
    /// </summary>
    internal sealed class ReactiveHealthServiceBuilder : IHealthServiceBuilder
    {
        private readonly HealthPolicyCollection policies = new HealthPolicyCollection();

        private IScheduler scheduler = Scheduler.Default;

        private TimeSpan? bufferInterval;

        public ReactiveHealthServiceBuilder(Action<IHealthPolicyCollection> configurePolicies) => configurePolicies(this.policies);

        /// <inheritdoc />
        public IHealthServiceBuilder Buffer(TimeSpan interval)
        {
            this.bufferInterval = interval;
            return this;
        }

        /// <inheritdoc />
        public IHealthServiceBuilder ScheduleOn(IScheduler scheduler)
        {
            this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            return this;
        }

        /// <inheritdoc />
        public IHealthService Build()
        {
            var disposables = new CompositeDisposable();
            return new ReactiveHealthService(this.BuildSource(disposables), disposables);
        }

        private IObservable<HealthReport> BuildSource(ICollection<IDisposable> disposables)
        {
            var source = this.policies.Build(this.scheduler, disposables)
                .ToList()
                .TimeInterval(this.scheduler)
                .Select(x => new HealthReport(x.Value, x.Interval));
            if (this.bufferInterval.HasValue)
            {
                var buffer = new PollingObservable<HealthReport>(source, this.bufferInterval.Value, this.scheduler);
                disposables.Add(buffer);
                source = buffer;
            }

            return source;
        }
    }
}
