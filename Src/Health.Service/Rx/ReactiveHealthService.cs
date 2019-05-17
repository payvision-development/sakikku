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

        internal ReactiveHealthService(IObservable<HealthReport> stream) => this.stream = stream;

        /// <inheritdoc />
        public async Task<HealthReport> CheckAsync(CancellationToken cancellationToken) =>
            await this.stream.RunAsync(cancellationToken);
    }
}
