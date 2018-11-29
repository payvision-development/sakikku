// -----------------------------------------------------------------------
// <copyright file="ISchedulingConfiguration`1.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    using System;

    /// <summary>
    /// Configures how a component is going to be executed within a task scheduler.
    /// </summary>
    /// <typeparam name="TThis">Type of the configuration interface.</typeparam>
    public interface ISchedulingConfiguration<out TThis>
    {
        /// <summary>
        /// Configures the execution to be scheduled in a polling in order to retrieve the
        /// last polled result.
        /// </summary>
        /// <remarks>Polling will avoid overlapped executions until previous one is finished.</remarks>
        /// <param name="pollingInterval">The polling time interval.</param>
        /// <returns>The configuration instance.</returns>
        TThis Polling(TimeSpan pollingInterval);
    }
}