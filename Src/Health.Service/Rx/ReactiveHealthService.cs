namespace Payvision.Diagnostics.Health.Rx
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Health service implementation that uses Rx to handle the policy execution.
    /// </summary>
    public sealed class ReactiveHealthService : IHealthService
    {
        private readonly IObservable<HealthReport> stream;

        private readonly IDisposable disposable;

        internal ReactiveHealthService(IObservable<HealthReport> stream, IDisposable disposable)
        {
            this.stream = stream;
            this.disposable = disposable;
        }

        /// <inheritdoc />
        public async Task<HealthReport> CheckAsync(CancellationToken cancellationToken) =>
            await this.stream.RunAsync(cancellationToken);

        public void Dispose() => this.disposable.Dispose();
    }
}
