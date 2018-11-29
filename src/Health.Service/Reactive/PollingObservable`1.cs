// -----------------------------------------------------------------------
// <copyright file="PollingObservable`1.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Threading;

    /// <summary>
    /// Executes any source sequence within a polling which avoids ticks overlapping (if the source is still executing,
    /// it will to next tick to execute again) returning the last known result. It means that both false positive or
    /// false negative results can be retrieved.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <seealso cref="System.IObservable{TSource}" />
    /// <seealso cref="System.IDisposable" />
    internal sealed class PollingObservable<TSource> : IObservable<TSource>, IDisposable
    {
        private readonly IScheduler scheduler;

        private readonly AtomicBool isExecuting = false;

        private readonly TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();

        private IDisposable polling;

        private TSource currentValue;

        public PollingObservable(IObservable<TSource> source, TimeSpan pollingInterval, IScheduler scheduler)
        {
            this.scheduler = scheduler;
            this.polling = Observable.Timer(TimeSpan.Zero, pollingInterval, scheduler)
                .Where(_ => this.isExecuting.Exchange(true))
                .SelectMany(_ => source)
                .Subscribe(this.UpdateCurrentValue);
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<TSource> observer) => this.scheduler.ScheduleAsync(observer, this.WaitToSendResult);

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.polling == null)
            {
                return;
            }

            this.polling.Dispose();
            this.polling = null;
        }

        private void UpdateCurrentValue(TSource value)
        {
            this.currentValue = value;
            this.tsc.TrySetResult(true);
            this.isExecuting.Exchange(false);
        }

        private async Task<IDisposable> WaitToSendResult(IScheduler _, IObserver<TSource> observer, CancellationToken ct)
        {
            try
            {
                await this.tsc.WaitAsync(ct);
                if (!ct.IsCancellationRequested)
                {
                    observer.OnNext(this.currentValue);
                    observer.OnCompleted();
                }
            }
            catch (OperationCanceledException)
            {
                // If the scheduled task has been canceled just shouldn't send any message.
            }
            catch (Exception exception)
            {
                observer.OnError(exception);
            }

            return Disposable.Empty;
        }
    }
}