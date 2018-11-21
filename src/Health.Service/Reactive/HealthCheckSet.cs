namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    internal sealed class HealthCheckSet : IHealthCheckSet
    {
        private readonly Dictionary<string, ReactiveHealthCheck> checks =
            new Dictionary<string, ReactiveHealthCheck>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc />
        public IHealthCheckFactory Add(string name)
        {
            var factory = new ReactiveHealthCheck();
            this.checks.Add(name, factory);
            return factory;
        }

        internal IObservable<HealthReport> Build(IScheduler scheduler) =>
            this.checks
                .Select(x => x.Value.Build(scheduler).Select(entry => (x.Key, entry)))
                .Merge()
                .ToDictionary(x => x.Key, x => x.entry)
                .TimeInterval(scheduler)
                .Select(x => new HealthReport(new ReadOnlyDictionary<string, HealthCheckEntry>(x.Value), x.Interval));
    }
}
