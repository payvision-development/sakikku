// -----------------------------------------------------------------------
// <copyright file="IHealthCheckSet.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Contains a collection of <see cref="IHealthCheck"/> identified by a name.
    /// </summary>
    public interface IHealthCheckSet
    {
        /// <summary>
        /// Adds a new <see cref="IHealthCheck"/> related to the specified name.
        /// </summary>
        /// <param name="name">The name of the health check which must be unique within the collection.</param>
        /// <returns>A <see cref="IHealthCheckFactory"/> to configure the <see cref="IHealthCheck"/> creation.</returns>
        IHealthCheckFactory Add(string name);
    }
}