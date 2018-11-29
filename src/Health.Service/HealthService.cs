// -----------------------------------------------------------------------
// <copyright file="HealthService.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    using System;

    using Reactive;

    /// <summary>
    /// Starting point to define a new <see cref="IHealthService"/> instance.
    /// </summary>
    public static class HealthService
    {
        /// <summary>
        /// Creates a new health service using the given configuration expression in order to include
        /// all the health checks that will be executed within the new health service.
        /// </summary>
        /// <param name="configure">The configuration callback.</param>
        /// <returns>The <see cref="IHealthServiceBuilder"/> instance to configure the service.</returns>
        /// <exception cref="ArgumentNullException">configure</exception>
        public static IHealthServiceBuilder Create(Action<IHealthCheckSet> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var builder = new ReactiveHealthServiceBuilder();
            builder.Apply(configure);
            return builder;
        }
    }
}