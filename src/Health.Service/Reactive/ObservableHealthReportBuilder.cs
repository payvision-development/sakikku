// -----------------------------------------------------------------------
// <copyright file="ObservableHealthReportBuilder.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// Builder of reactive sequences that returns health reports.
    /// </summary>
    /// <seealso cref="Payvision.Diagnostics.Health.IHealthCheckSet" />
    internal sealed class ObservableHealthReportBuilder : IHealthCheckSet, IObservableBuilder<HealthReport>
    {
        private readonly Dictionary<string, IObservableBuilder<HealthCheckEntry>> healthChecks =
            new Dictionary<string, IObservableBuilder<HealthCheckEntry>>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc />
        public IHealthCheckConfiguration Add(string name, IHealthCheck healthCheck)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (healthCheck == null)
            {
                throw new ArgumentNullException(nameof(healthCheck));
            }

            var configuration = new ObservableHealthCheckBuilder(healthCheck);
            this.healthChecks[name] = configuration;
            return configuration;
        }

        /// <inheritdoc />
        public IObservable<HealthReport> Build(IScheduler scheduler, ICompositeDisposable disposable) =>
            this.healthChecks
                .Select(x => x.Value.Build(scheduler, disposable).Select(entry => (x.Key, entry)))
                .Merge(scheduler)
                .ToDictionary(x => x.Key, x => x.entry)
                .TimeInterval(scheduler)
                .Select(x => new HealthReport(new ReadOnlyDictionary<string, HealthCheckEntry>(x.Value), x.Interval));
    }
}