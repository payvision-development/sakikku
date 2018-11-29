// -----------------------------------------------------------------------
// <copyright file="HealthCheckExtensions.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// <see cref="IHealthCheck"/> extension methods.
    /// </summary>
    internal static class HealthCheckExtensions
    {
        /// <summary>
        /// Converts the specified health check to an observable sequence of <see cref="HealthCheckResult"/>.
        /// </summary>
        /// <param name="healthCheck">The health check.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>The <see cref="HealthCheckResult"/> observable sequence.</returns>
        /// <exception cref="ArgumentNullException">
        /// healthCheck
        /// or
        /// scheduler
        /// </exception>
        public static IObservable<HealthCheckResult> ToObservable(this IHealthCheck healthCheck, IScheduler scheduler)
        {
            if (healthCheck == null)
            {
                throw new ArgumentNullException(nameof(healthCheck));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return Observable.FromAsync(healthCheck.CheckAsync, scheduler)
                .Catch<HealthCheckResult, Exception>(exception => CatchExceptionHandler(exception, scheduler));
        }

        /// <summary>
        /// Maps a sequence of <see cref="HealthCheckResult"/> into a sequence of <see cref="HealthCheckEntry"/>.
        /// </summary>
        /// <param name="source">The <see cref="HealthCheckResult"/> source.</param>
        /// <param name="tags">The tags related to the health check.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// source
        /// or
        /// tags
        /// or
        /// scheduler
        /// </exception>
        public static IObservable<HealthCheckEntry> ToHealthCheckEntries(
            this IObservable<HealthCheckResult> source,
            IEnumerable<string> tags,
            IScheduler scheduler)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return source.TimeInterval(scheduler)
                .Select(x => new HealthCheckEntry(x.Value.Status, x.Value.Message, x.Interval, x.Value.Data, tags));
        }

        private static IObservable<HealthCheckResult> CatchExceptionHandler(Exception exception, IScheduler scheduler)
        {
            var data = new Dictionary<string, string>();
            foreach (object key in exception.Data.Keys)
            {
                if (key != null && exception.Data[key] != null)
                {
                    data[key.ToString()] = exception.Data[key].ToString();
                }
            }

            return Observable.Return(new HealthCheckResult(HealthStatus.Unhealthy, exception.Message, data), scheduler);
        }
    }
}