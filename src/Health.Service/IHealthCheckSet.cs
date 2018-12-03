﻿// -----------------------------------------------------------------------
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
        /// <param name="healthCheck">The health check instance to be executed within the owning <see cref="IHealthService"/>.</param>
        /// <returns>A <see cref="IHealthCheckConfiguration"/> to configure the health check.</returns>
        IHealthCheckConfiguration Add(string name, IHealthCheck healthCheck);
    }
}