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
        /// <param name="result">The result.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="tags">The tags.</param>
        internal HealthCheckEntry(HealthCheckResult result, TimeSpan duration, IEnumerable<string> tags)
        {
            this.Duration = duration;
            this.Tags = tags;
            this.Status = result.Status;
            this.Message = result.Message;
            this.Data = result.Data;
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
        public readonly IReadOnlyDictionary<string, object> Data;
    }
}