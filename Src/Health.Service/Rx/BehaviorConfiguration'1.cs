namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// Base class to configure any sequence execution behavior.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TThis"></typeparam>
    internal abstract class BehaviorConfiguration<TSource, TThis> : ISequenceBuilder<TSource>, IBehaviorConfiguration<TThis>
        where TThis : IBehaviorConfiguration<TThis>
    {
        private TimeSpan? pollingInterval;

        private TimeSpan? timeout;

        /// <inheritdoc />
        public TThis Buffer(TimeSpan interval)
        {
            this.pollingInterval = interval;
            return (TThis)(IBehaviorConfiguration<TThis>)this;
        }

        /// <inheritdoc />
        public TThis Timeout(TimeSpan dueTime)
        {
            this.timeout = dueTime;
            return (TThis)(IBehaviorConfiguration<TThis>)this;
        }

        /// <inheritdoc />
        public IObservable<TSource> Build(IScheduler scheduler, ICollection<IDisposable> compositeDisposable)
        {
            IObservable<TSource> source = this.BuildSource(scheduler, compositeDisposable);
            source = this.BuildTimeout(source, scheduler);
            return this.BuildBuffer(source, scheduler, compositeDisposable);
        }

        /// <summary>
        /// Builds the observable source sequence to modify its execution behavior as configured.
        /// </summary>
        /// <param name="scheduler">The scheduler where the sequence is going to be executed.</param>
        /// <param name="compositeDisposable">If a disposable handler is needed it will be added to this composite.</param>
        /// <returns>The source sequence.</returns>
        protected abstract IObservable<TSource> BuildSource(IScheduler scheduler, ICollection<IDisposable> compositeDisposable);

        /// <summary>
        /// Creates a proper source response to handle any <see cref="TimeoutException"/> if produced.
        /// </summary>
        /// <param name="timeout">The configured timeout waited before the exception is thrown.</param>
        /// <param name="exception">The timeout exception.</param>
        /// <returns>The response describing the timeout.</returns>
        protected abstract TSource TimeoutResponse(TimeSpan timeout, TimeoutException exception);

        private IObservable<TSource> BuildBuffer(
            IObservable<TSource> source,
            IScheduler scheduler,
            ICollection<IDisposable> compositeDisposable)
        {
            if (!this.pollingInterval.HasValue)
            {
                return source;
            }

            var buffer = new PollingObservable<TSource>(source, this.pollingInterval.Value, scheduler);
            compositeDisposable.Add(buffer);
            return buffer;
        }

        private IObservable<TSource> BuildTimeout(IObservable<TSource> source, IScheduler scheduler)
        {
            if (!this.timeout.HasValue)
            {
                return source;
            }

            return source.Timeout(this.timeout.Value, scheduler)
                .Catch<TSource, TimeoutException>(x => Observable.Return(this.TimeoutResponse(this.timeout.Value, x), scheduler));
        }
    }
}