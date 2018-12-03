// -----------------------------------------------------------------------
// <copyright file="HealthCheckSetExtensions.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// <see cref="IHealthCheckSet"/> extension methods.
    /// </summary>
    public static class HealthCheckSetExtensions
    {
        /// <summary>
        /// Adds a new <see cref="IHealthCheck"/> related to the specified name.
        /// </summary>
        /// <param name="set">The <see cref="IHealthCheckSet"/> instance.</param>
        /// <param name="name">The name of the health check which must be unique within the collection.</param>
        /// <typeparam name="T">The type of the health check to add.</typeparam>
        /// <returns>A <see cref="IHealthCheckConfiguration"/> to configure the health check.</returns>
        public static IHealthCheckConfiguration Add<T>(this IHealthCheckSet set, string name)
            where T : IHealthCheck, new() => set.Add(name, new T());
    }
}