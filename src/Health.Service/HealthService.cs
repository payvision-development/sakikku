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
    /// Allows the definition of a new <see cref="IHealthService"/> instance.
    /// </summary>
    public static class HealthService
    {
        /// <summary>
        /// Creates a new health service using the given configuration expression in order to include
        /// all the health checks that will be executed within the new health service.
        /// </summary>
        /// <param name="configure">The configuration expression.</param>
        /// <returns>The <see cref="IHealthService"/> instance.</returns>
        /// <exception cref="ArgumentNullException">configure</exception>
        public static IHealthService Create(Action<IHealthCheckSet> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var reportBuilder = new ObservableHealthReportBuilder();
            configure(reportBuilder);
            return new ReactiveHealthService(reportBuilder);
        }
    }
}