namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Represents a health check which can be used to check the status
    /// of either a part or a dependency of the application.
    /// </summary>
    public interface IHealthCheck
    {
        /// <summary>
        /// Runs the health check.
        /// </summary>
        /// <returns>The result of the health check.</returns>
        HealthCheckResult Check();
    }
}