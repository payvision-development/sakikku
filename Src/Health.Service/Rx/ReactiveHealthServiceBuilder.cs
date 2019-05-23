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

        private TimeSpan? timeout;

        public ReactiveHealthServiceBuilder(Action<IHealthPolicyCollection> configurePolicies) => configurePolicies(this.policies);

        /// <inheritdoc />
        public IHealthServiceBuilder Buffer(TimeSpan interval)
        {
            this.bufferInterval = interval;
            return this;
        }

        /// <inheritdoc />
        public IHealthServiceBuilder Timeout(TimeSpan dueTime)
        {
            this.timeout = dueTime;
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
            var source = this.BuildSource(disposables);
            source = this.BuildTimeout(source);
            source = this.BuildBuffer(source, disposables);

            return new ReactiveHealthService(source, disposables);
        }

        private IObservable<HealthReport> BuildBuffer(IObservable<HealthReport> source, ICollection<IDisposable> disposables)
        {
            if (!this.bufferInterval.HasValue)
            {
                return source;
            }

            var buffer = new PollingObservable<HealthReport>(source, this.bufferInterval.Value, this.scheduler);
            disposables.Add(buffer);
            return buffer;
        }

        private IObservable<HealthReport> BuildTimeout(IObservable<HealthReport> source)
        {
            if (!this.timeout.HasValue)
            {
                return source;
            }

            return source.Timeout(this.timeout.Value, this.scheduler)
                .Catch<HealthReport, TimeoutException>(_ => Observable.Return(HealthReport.Timeout(this.timeout.Value), this.scheduler));
        }

        private IObservable<HealthReport> BuildSource(ICollection<IDisposable> disposables) =>
            this.policies.Build(this.scheduler, disposables)
                .ToList()
                .TimeInterval(this.scheduler)
                .Select(x => new HealthReport(x.Value, x.Interval));
    }
}
