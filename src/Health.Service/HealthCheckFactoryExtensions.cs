// -----------------------------------------------------------------------
// <copyright file="HealthCheckFactoryExtensions.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Health check extension methods.
    /// </summary>
    public static class HealthCheckFactoryExtensions
    {
        /// <summary>
        /// The configuring <see cref="IHealthService"/> will execute the specified <see cref="IHealthCheck"/>.
        /// </summary>
        /// <param name="factory">The <see cref="IHealthCheckFactory"/>.</param>
        /// <returns>The <see cref="IHealthCheckConfiguration"/> to configure the health check.</returns>
        public static IHealthCheckConfiguration For<T>(this IHealthCheckFactory factory)
            where T : IHealthCheck, new() => factory.For(new T());
    }
}