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
    internal sealed class ReactiveHealthServiceBuilder : BehaviorConfiguration<HealthReport, IHealthServiceBuilder>, IHealthServiceBuilder
    {
        private readonly HealthPolicyCollection policies = new HealthPolicyCollection();

        private IScheduler scheduler = Scheduler.Default;

        public ReactiveHealthServiceBuilder(Action<IHealthPolicyCollection> configurePolicies) => configurePolicies(this.policies);

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
            IObservable<HealthReport> source = this.Build(this.scheduler, disposables);
            return new ReactiveHealthService(source, disposables);
        }

        /// <inheritdoc />
        protected override IObservable<HealthReport> BuildSource(IScheduler scheduler, ICollection<IDisposable> compositeDisposable) =>
            this.policies.Build(scheduler, compositeDisposable)
                .ToList()
                .TimeInterval(this.scheduler)
                .Select(x => new HealthReport(x.Value, x.Interval));

        /// <inheritdoc />
        protected override HealthReport TimeoutResponse(TimeSpan timeout, TimeoutException exception) =>
            HealthReport.Timeout(timeout);

        private IObservable<HealthReport> BuildSource(ICollection<IDisposable> disposables) =>
            this.policies.Build(this.scheduler, disposables)
                .ToList()
                .TimeInterval(this.scheduler)
                .Select(x => new HealthReport(x.Value, x.Interval));
    }
}
