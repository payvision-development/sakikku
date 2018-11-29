// -----------------------------------------------------------------------
// <copyright file="ObservableBuilder`3.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Base implementation of an observable builder which implements the scheduling strategy configuration.
    /// </summary>
    /// <typeparam name="TSource">The type of the observable source.</typeparam>
    /// <typeparam name="TThis">The type of the implementor.</typeparam>
    /// <typeparam name="TBuilder">The type of the builder to cast the implementor.</typeparam>
    internal abstract class ObservableBuilder<TSource, TThis, TBuilder> : IBehaviorConfiguration<TBuilder>,
        IObservableBuilder<TSource>
        where TThis : ObservableBuilder<TSource, TThis, TBuilder>, TBuilder
    {
        private TimeSpan? polling;

        /// <inheritdoc />
        public TBuilder Polling(TimeSpan pollingInterval)
        {
            this.polling = pollingInterval;
            return (TThis)this;
        }

        /// <inheritdoc />
        public IObservable<TSource> Build(IScheduler scheduler, ICompositeDisposable disposable)
        {
            IObservable<TSource> stream = this.BuildSourceObservable(scheduler, disposable);
            if (this.polling.HasValue)
            {
                var polledObservable = new PollingObservable<TSource>(stream, this.polling.Value, scheduler);
                disposable.Attach(polledObservable);
                stream = polledObservable;
            }

            return stream;
        }

        /// <summary>
        /// Builds the observable sequence which is going to be decorated with further sequence controlling the execution behavior.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="disposable">The disposable.</param>
        /// <returns>The observable instance.</returns>
        protected abstract IObservable<TSource> BuildSourceObservable(IScheduler scheduler, ICompositeDisposable disposable);
    }
}