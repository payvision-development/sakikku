namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Contains the result of a health policy check.
    /// </summary>
    public sealed class HealthCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckResult"/> struct.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="message">The message.</param>
        public HealthCheck(HealthStatus status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        /// <summary>
        /// Gets the status of the checked component.
        /// </summary>
        public HealthStatus Status { get; }

        /// <summary>
        /// Gets a readable message that describes the health check result.
        /// </summary>
        public string Message { get; }
    }
}
