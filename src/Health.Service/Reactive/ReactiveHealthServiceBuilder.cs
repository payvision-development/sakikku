// -----------------------------------------------------------------------
// <copyright file="ReactiveHealthServiceBuilder.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Reactive.Concurrency;

    internal sealed class ReactiveHealthServiceBuilder :
        ObservableBuilder<HealthReport, ReactiveHealthServiceBuilder, IHealthServiceBuilder>,
        IHealthServiceBuilder
    {
        private readonly ObservableHealthReportBuilder sourceBuilder = new ObservableHealthReportBuilder();

        /// <inheritdoc />
        public IHealthService Build() => new ReactiveHealthService(this);

        /// <summary>
        /// Applies the specified health check configuration on the inner <see cref="IHealthCheckSet"/>.
        /// </summary>
        /// <param name="configure">The configuration callback.</param>
        internal void Apply(Action<IHealthCheckSet> configure) => configure(this.sourceBuilder);

        /// <inheritdoc />
        protected override IObservable<HealthReport> BuildSourceObservable(IScheduler scheduler, ICompositeDisposable disposable) =>
            this.sourceBuilder.Build(scheduler, disposable);
    }
}