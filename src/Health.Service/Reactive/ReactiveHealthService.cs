// -----------------------------------------------------------------------
// <copyright file="ReactiveHealthService.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class ReactiveHealthService : IHealthService
    {
        private readonly IObservable<HealthReport> stream;

        public ReactiveHealthService(IObservable<HealthReport> stream)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <inheritdoc />
        public async Task<HealthReport> CheckAsync(CancellationToken cancellationToken) => await this.stream.RunAsync(cancellationToken);
    }
}