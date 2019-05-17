namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Builder of <see cref="IHealthService"/> instances.
    /// </summary>
    public interface IHealthServiceBuilder
    {
        /// <summary>
        /// Builds a new health service instance as configured.
        /// </summary>
        /// <returns>The new <see cref="IHealthService"/> instance ready to run.</returns>
        IHealthService Build();
    }
}
