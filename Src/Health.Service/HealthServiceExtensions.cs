namespace Payvision.Diagnostics.Health
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// <see cref="IHealthService"/> extension methods.
    /// </summary>
    public static class HealthServiceExtensions
    {
        /// <summary>
        /// Checks the health of the application executing asynchronously the configured health checks.
        /// </summary>
        /// <returns>The <see cref="HealthReport"/> of the application at the calling time.</returns>
        public static Task<HealthReport> CheckAsync(this IHealthService healthService) => healthService.CheckAsync(CancellationToken.None);
    }
}
