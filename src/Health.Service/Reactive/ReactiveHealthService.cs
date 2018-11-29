// -----------------------------------------------------------------------
// <copyright file="ReactiveHealthService.cs" company="Payvision">
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

    /// <summary>
    /// <see cref="IHealthService"/> implementation through reactive sequences per health check.
    /// </summary>
    /// <seealso cref="Payvision.Diagnostics.Health.IHealthService" />
    internal sealed class ReactiveHealthService : IHealthService, ICompositeDisposable
    {
        private readonly CancellationDisposable cancellationDisposable = new CancellationDisposable();

        private readonly IObservable<HealthReport> stream;

        public ReactiveHealthService(IObservableBuilder<HealthReport> observableBuilder)
        {
            this.stream = observableBuilder.Build(Scheduler.Default, this);
        }

        /// <inheritdoc />
        public async Task<HealthReport> CheckAsync(CancellationToken cancellationToken) => await this.stream.RunAsync(cancellationToken);

        /// <inheritdoc />
        public void Attach(IDisposable disposable) => this.cancellationDisposable.Token.Register(disposable.Dispose);

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.cancellationDisposable.IsDisposed)
            {
                return;
            }

            this.cancellationDisposable.Dispose();
        }
    }
}