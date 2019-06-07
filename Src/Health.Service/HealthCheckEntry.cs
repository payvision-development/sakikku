namespace Payvision.Diagnostics.Health
{
    using System;

    /// <summary>
    /// Represents an entry in a <see cref="HealthReport"/> containing the result of a single health check.
    /// </summary>
    public sealed class HealthCheckEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckEntry"/> struct.
        /// </summary>
        /// <param name="policy">The policy name.</param>
        /// <param name="status">The status.</param>
        /// <param name="message">The message.</param>
        /// <param name="duration">The duration.</param>
        public HealthCheckEntry(
            string policy,
            HealthStatus status,
            string message,
            TimeSpan duration)
        {
            this.Policy = policy;
            this.Status = status;
            this.Message = message;
            this.Duration = duration;
        }

        /// <summary>
        /// Gets the name of the policy that performed the health check.
        /// </summary>
        public string Policy { get; internal set; }

        /// <summary>
        /// Gets the status of the checked component.
        /// </summary>
        public HealthStatus Status { get; internal set; }

        /// <summary>
        /// Gets a readable message that describes the health check result.
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// Gets the health check execution duration.
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// Creates a <see cref="HealthCheckEntry"/> that indicates a timeout result.
        /// </summary>
        /// <param name="policy">The policy name.</param>
        /// <param name="timeout">The timeout value.</param>
        /// <returns>The timeout entry.</returns>
        internal static HealthCheckEntry Timeout(string policy, TimeSpan timeout)
        {
            const string TimeoutMessage = "The policy has taken longer time to be executed than the maximum allowed.";

            return new HealthCheckEntry(policy, HealthStatus.Unhealthy, TimeoutMessage, timeout);
        }
    }
}
