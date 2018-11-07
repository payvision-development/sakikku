namespace Payvision.Diagnostics.Health
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the result of a health check.
    /// </summary>
    public readonly struct HealthCheckResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckResult"/> struct.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="message">The message.</param>
        /// <param name="data">The data.</param>
        public HealthCheckResult(HealthStatus status, string message, IReadOnlyDictionary<string, string> data)
        {
            this.Status = status;
            this.Message = message;
            this.Data = data;
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
        /// Gets any extra data related to the health check.
        /// </summary>
        public readonly IReadOnlyDictionary<string, string> Data;
    }
}
