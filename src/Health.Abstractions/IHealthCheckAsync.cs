// -----------------------------------------------------------------------
// <copyright file="IHealthCheckAsync.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a health check which can be used to check asynchronously
    /// the status of either a part or a dependency of the application.
    /// </summary>
    public interface IHealthCheckAsync
    {
        /// <summary>
        /// Gets the name associated to this health check.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the tags related to this health check.
        /// </summary>
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// Runs the health check asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> of the asynchronous check.</param>
        /// <returns>The result of the health check.</returns>
        Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken);
    }
}