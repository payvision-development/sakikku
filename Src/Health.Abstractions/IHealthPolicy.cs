namespace Payvision.Diagnostics.Health
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a health check policy that can be used to check the status of
    /// either a part or a dependency of the checking application.
    /// </summary>
    public interface IHealthPolicy
    {
        /// <summary>
        /// Runs the health check.
        /// </summary>
        /// <remarks>The health check service will catch any thrown exception projecting to an unhealthy result.</remarks>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> of the asynchronous operation.</param>
        /// <returns>The result of the health check.</returns>
        Task<HealthCheck> CheckAsync(CancellationToken cancellationToken);
    }
}
