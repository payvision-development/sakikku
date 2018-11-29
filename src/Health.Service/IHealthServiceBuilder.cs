// -----------------------------------------------------------------------
// <copyright file="IHealthServiceBuilder.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Builder to configure a new <see cref="IHealthService"/> instance.
    /// </summary>
    public interface IHealthServiceBuilder : IBehaviorConfiguration<IHealthServiceBuilder>
    {
        /// <summary>
        /// Builds a new health service instance as configured.
        /// </summary>
        /// <returns>The new <see cref="IHealthService"/> instance ready to run.</returns>
        IHealthService Build();
    }
}