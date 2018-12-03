// -----------------------------------------------------------------------
// <copyright file="ReactiveHealthCheck.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    internal sealed class ReactiveHealthCheck : IHealthCheckConfiguration
    {
        private readonly List<string> currentTags = new List<string>();

        private readonly IHealthCheck currentHealthCheck;

        public ReactiveHealthCheck(IHealthCheck currentHealthCheck)
        {
            this.currentHealthCheck = currentHealthCheck;
        }

        /// <inheritdoc />
        public IHealthCheckConfiguration Tags(IEnumerable<string> tags)
        {
            this.currentTags.AddRange(tags);
            return this;
        }

        internal IObservable<HealthCheckEntry> Build(IScheduler scheduler)
        {
            return Observable.FromAsync(this.currentHealthCheck.CheckAsync, scheduler)
                .Catch<HealthCheckResult, Exception>(exception => CatchExceptionHandler(exception, scheduler))
                .TimeInterval(scheduler)
                .Select(x => new HealthCheckEntry(x.Value.Status, x.Value.Message, x.Interval, x.Value.Data, this.currentTags));
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