namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Payvision.Diagnostics.Health.Threading;

    /// <summary>
    /// Executes any source sequence within a polling which avoids ticks overlapping (if the source is still executing,
    /// it will wait to next tick to execute again) returning the last known result. It means that both false positive or
    /// false negative results can be retrieved.
    /// </summary>
    /// <remarks>If the inner source is not taking almost time to be executed, this buffer might fall into a back-pressure
    /// problem, so a throttling strategy could be useful at that point.</remarks>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <seealso cref="System.IObservable{TSource}" />
    /// <seealso cref="System.IDisposable" />
    internal sealed class PollingObservable<TSource> : IObservable<TSource>, IDisposable
    {
        private const int BufferLength = 1;

        private readonly IObservable<TSource> source;

        private readonly AtomicBool isExecuting = false;

        private readonly IDisposable pollingHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingObservable{TSource}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pollingInterval">The polling interval.</param>
        /// <param name="scheduler">The scheduler.</param>
        public PollingObservable(IObservable<TSource> source, TimeSpan pollingInterval, IScheduler scheduler)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            IConnectableObservable<TSource> publisher = Observable.Timer(TimeSpan.Zero, pollingInterval, scheduler)
                .Where(_ => this.isExecuting.Exchange(true))
                .SelectMany(_ => source)
                .Multicast(new ReplaySubject<TSource>(BufferLength, scheduler));

            this.source = publisher;
            this.pollingHandler = new CompositeDisposable(publisher.Subscribe(this.OnTick), publisher.Connect());
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<TSource> observer) => this.source.Take(BufferLength).Subscribe(observer);

        /// <inheritdoc />
        public void Dispose() => this.pollingHandler.Dispose();

        private void OnTick(TSource _) => this.isExecuting.Exchange(false);
    }
}
