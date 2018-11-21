// -----------------------------------------------------------------------
// <copyright file="IHealthCheckFactory.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Creates a factory of one <see cref="IHealthCheck"/>.
    /// </summary>
    public interface IHealthCheckFactory
    {
        /// <summary>
        /// The configuring <see cref="IHealthService"/> will execute the specified <see cref="IHealthCheck"/>.
        /// </summary>
        /// <param name="healthCheck">The health check instance to be executed within the owning <see cref="IHealthService"/>.</param>
        /// <returns>The <see cref="IHealthCheckConfiguration"/> to configure the health check.</returns>
        IHealthCheckConfiguration For(IHealthCheck healthCheck);
    }
}