namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Builds a new observable sequence.
    /// </summary>
    /// <typeparam name="TSource">Type of the item streamed.</typeparam>
    internal interface ISequenceBuilder<TSource>
    {
        /// <summary>
        /// Builds a reactive sequence.
        /// </summary>
        /// <param name="scheduler">The scheduler where the sequence is going to be scheduled.</param>
        /// <param name="compositeDisposable">If a disposable handler is needed it will be added to this composite.</param>
        /// <returns>The observable sequence.</returns>
        IObservable<TSource> Build(IScheduler scheduler, ICollection<IDisposable> compositeDisposable);
    }
}
