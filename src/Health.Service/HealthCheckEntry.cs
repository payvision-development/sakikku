namespace Payvision.Diagnostics.Health
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an entry in a <see cref="HealthReport"/> containing the result of a single health check.
    /// </summary>
    public readonly struct HealthCheckEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckEntry"/> struct.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="message">The message.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="data">The data.</param>
        /// <param name="tags">The tags.</param>
        internal HealthCheckEntry(
            HealthStatus status,
            string message,
            TimeSpan duration,
            IReadOnlyDictionary<string, string> data,
            IEnumerable<string> tags)
        {
            this.Status = status;
            this.Message = message;
            this.Data = data;
            this.Duration = duration;
            this.Tags = tags;
        }

        /// <summary>
        /// Gets the status of the checked component.
        /// </summary>
        public readonly HealthStatus Status;

        /// <summary>
        /// Gets a readable message that describes the health check result.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Gets the health check execution duration.
        /// </summary>
        public readonly TimeSpan Duration;

        /// <summary>
        /// Gets the tags related to this health check.
        /// </summary>
        public readonly IEnumerable<string> Tags;

        /// <summary>
        /// Gets any extra data related to the health check.
        /// </summary>
        public readonly IReadOnlyDictionary<string, string> Data;
    }
}