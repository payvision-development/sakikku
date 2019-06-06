using System.Reactive.Concurrency;

namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Builder of <see cref="IHealthService"/> instances.
    /// </summary>
    public interface IHealthServiceBuilder : IBehaviorConfiguration<IHealthServiceBuilder>
    {
        /// <summary>
        /// The building health service will execute its policy tasks within the specified scheduler.
        /// </summary>
        /// <param name="scheduler">The scheduler used to schedule all the health check tasks.</param>
        /// <returns>The <see cref="IHealthServiceBuilder"/> instance.</returns>
        IHealthServiceBuilder ScheduleOn(IScheduler scheduler);

        /// <summary>
        /// Builds a new health service instance as configured.
        /// </summary>
        /// <returns>The new <see cref="IHealthService"/> instance ready to run.</returns>
        IHealthService Build();
    }
}