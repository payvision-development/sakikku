namespace Payvision.Diagnostics.Health
{
    using System;

    /// <summary>
    /// Configures how a component is going to be executed within a task scheduler.
    /// </summary>
    /// <typeparam name="TThis">Type of the configuration inheritor.</typeparam>
    public interface IBehaviourConfiguration<out TThis>
    {
        /// <summary>
        /// Configures the execution to be scheduled in a polling in order to retrieve
        /// the last polled result.
        /// </summary>
        /// <remarks>Polling will avoid overlapped executions until previous one is finished.</remarks>
        /// <param name="interval">The time interval to refresh the buffered result.</param>
        /// <returns>The configuration instance.</returns>
        TThis Buffer(TimeSpan interval);

        /// <summary>
        /// Applies a timeout policy to the execution.
        /// If the result is not received within the specified timeout duration, a timeout result is going to be returned.
        /// </summary>
        /// <param name="dueTime">Maximum duration allowed for the execution.</param>
        /// <returns>The configuration instance.</returns>
        TThis Timeout(TimeSpan dueTime);
    }
}
