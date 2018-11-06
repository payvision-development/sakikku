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
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthReport"/> class.
        /// </summary>
        /// <param name="entries">The entries.</param>
        /// <param name="totalDuration">The total duration.</param>
        internal HealthReport(IReadOnlyDictionary<string, HealthCheckEntry> entries, TimeSpan totalDuration)
        {
            this.Entries = entries ?? throw new ArgumentNullException(nameof(entries));
            this.TotalDuration = totalDuration;
            this.Status = GetLowerStatus(entries.Values.Select(x => x.Status));
        }

        /// <summary>
        /// Gets the <see cref="HealthStatus"/> representing the aggregate status of <see cref="Entries"/>.
        /// The value will be the most severe value.
        /// </summary>
        public HealthStatus Status { get; }

        /// <summary>
        /// Gets the results from each health check executed by the health service.
        /// </summary>
        /// <remarks>
        /// The keys maps the name of each health check to its returned <see cref="HealthCheckEntry"/>.
        /// </remarks>
        public IReadOnlyDictionary<string, HealthCheckEntry> Entries { get; }

        /// <summary>
        /// Gets the time taken by the health check service to execute.
        /// </summary>
        public TimeSpan TotalDuration { get; }

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
    }
}