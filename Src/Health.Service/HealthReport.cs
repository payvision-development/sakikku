namespace Payvision.Diagnostics.Health
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains the result of executing a group of health check instances.
    /// </summary>
    public sealed class HealthReport
    {
        private readonly HealthCheckEntry[] entries;

        public HealthReport(IEnumerable<HealthCheckEntry> entries, TimeSpan duration)
        {
            this.entries = entries.ToArray();
            this.Duration = duration;
            this.Status = GetLowerStatus(this.entries.Select(x => x.Status));
        }

        /// <summary>
        /// Gets the <see cref="HealthStatus"/> representing the aggregate status of <see cref="Entries"/>.
        /// The value will be the most severe value.
        /// </summary>
        public HealthStatus Status { get; }

        /// <summary>
        /// Gets the time taken by the health check service to execute.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Gets the results from each health check policies executed by the health service.
        /// </summary>
        public IEnumerable<HealthCheckEntry> Entries => this.entries;

        private static HealthStatus GetLowerStatus(IEnumerable<HealthStatus> statuses)
        {
            HealthStatus result = HealthStatus.Healthy;
            foreach (HealthStatus status in statuses)
            {
                if (status == HealthStatus.Unhealthy)
                {
                    return status;
                }

                if (status > result)
                {
                    result = status;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a default health report containing a timeout message.
        /// </summary>
        /// <param name="timeout">The timeout value.</param>
        /// <returns>The timeout health report.</returns>
        internal static HealthReport Timeout(TimeSpan timeout) =>
            new HealthReport(Enumerable.Empty<HealthCheckEntry>(), timeout);
    }
}
