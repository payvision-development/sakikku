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
    internal sealed class ObservableHealthCheckBuilder :
        ObservableBuilder<HealthCheckEntry, ObservableHealthCheckBuilder, IHealthCheckConfiguration>,
        IHealthCheckConfiguration
    {
        private readonly List<string> currentTags = new List<string>();

        private readonly IHealthCheck healthCheck;

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
        protected override IObservable<HealthCheckEntry> BuildSourceObservable(IScheduler scheduler, ICompositeDisposable disposable) =>
            this.healthCheck.ToObservable(scheduler)
                .ToHealthCheckEntries(this.currentTags, scheduler);
    }
}