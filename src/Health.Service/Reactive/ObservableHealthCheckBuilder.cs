// -----------------------------------------------------------------------
// <copyright file="ObservableHealthCheckBuilder.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Configuration of a health check to execute in a reactive sequence.
    /// </summary>
    /// <seealso cref="Payvision.Diagnostics.Health.IHealthCheckConfiguration" />
    internal sealed class ObservableHealthCheckBuilder : IHealthCheckConfiguration,
        IObservableBuilder<HealthCheckEntry>
    {
        private readonly List<string> currentTags = new List<string>();

        private readonly IHealthCheck healthCheck;

        private TimeSpan? polling;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHealthCheckBuilder"/> class.
        /// </summary>
        /// <param name="healthCheck">The health check.</param>
        public ObservableHealthCheckBuilder(IHealthCheck healthCheck)
        {
            this.healthCheck = healthCheck;
        }

        /// <inheritdoc />
        public IHealthCheckConfiguration Tags(IEnumerable<string> tags)
        {
            this.currentTags.AddRange(tags);
            return this;
        }

        /// <inheritdoc />
        public IHealthCheckConfiguration Polling(TimeSpan pollingInterval)
        {
            this.polling = pollingInterval;
            return this;
        }

        public IObservable<HealthCheckEntry> Build(IScheduler scheduler, ICompositeDisposable disposable)
        {
            IObservable<HealthCheckResult> stream = this.healthCheck.ToObservable(scheduler);
            if (this.polling.HasValue)
            {
                var polledObservable = new PollingObservable<HealthCheckResult>(stream, this.polling.Value, scheduler);
                disposable.Attach(polledObservable);
                stream = polledObservable;
            }

            return stream.ToHealthCheckEntries(this.currentTags, scheduler);
        }
    }
}