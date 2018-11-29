// -----------------------------------------------------------------------
// <copyright file="IHealthCheckConfiguration.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    using System.Collections.Generic;

    /// <summary>
    /// Configures a <see cref="IHealthCheck"/> instance to be executed within a <see cref="IHealthService"/>.
    /// </summary>
    public interface IHealthCheckConfiguration
    {
        /// <summary>
        /// Sets the specified tags to any result produced by the health check.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns>The <see cref="IHealthCheckConfiguration"/> instance.</returns>
        IHealthCheckConfiguration Tags(IEnumerable<string> tags);
    }
}