namespace Payvision.Diagnostics.Health
{
    using System;

    using Payvision.Diagnostics.Health.Rx;

    /// <summary>
    /// Starting point from which create new health services.
    /// </summary>
    public static class HealthService
    {
        /// <summary>
        /// Creates a new health service builder that will use the health policies as specified
        /// in the given configuration callback.
        /// </summary>
        /// <param name="configure">The health policies configuration callback.</param>
        /// <returns>The <see cref="IHealthServiceBuilder"/> instance.</returns>
        public static IHealthServiceBuilder Create(Action<IHealthPolicyCollection> configure) => new ReactiveHealthServiceBuilder(configure);
    }
}
