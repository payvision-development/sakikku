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
        private readonly Dictionary<string, ObservableHealthCheckBuilder> healthChecks =
            new Dictionary<string, ObservableHealthCheckBuilder>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc />
        public IHealthCheckFactory Add(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (this.healthChecks.ContainsKey(name))
            {
                throw new ArgumentException(name);
            }

            return new HealthCheckFactory(name, this);
        }

        public IObservable<HealthReport> Build(IScheduler scheduler, ICompositeDisposable disposable)
        {
            return this.healthChecks
                .Select(x => x.Value.Build(scheduler, disposable).Select(entry => (x.Key, entry)))
                .Merge()
                .ToDictionary(x => x.Key, x => x.entry)
                .TimeInterval(scheduler)
                .Select(x => new HealthReport(new ReadOnlyDictionary<string, HealthCheckEntry>(x.Value), x.Interval));
        }

        #region HealthCheckFactory

        private sealed class HealthCheckFactory : IHealthCheckFactory
        {
            private readonly ObservableHealthReportBuilder set;

            private readonly string name;

            public HealthCheckFactory(string name, ObservableHealthReportBuilder set)
            {
                this.set = set;
                this.name = name;
            }

            /// <inheritdoc />
            public IHealthCheckConfiguration For(IHealthCheck healthCheck)
            {
                if (healthCheck == null)
                {
                    throw new ArgumentNullException(nameof(healthCheck));
                }

                var configuration = new ObservableHealthCheckBuilder(healthCheck);
                this.set.healthChecks[this.name] = configuration;
                return configuration;
            }
        }

        #endregion
    }
}