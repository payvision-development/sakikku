﻿namespace Payvision.Health.Service.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Diagnostics.Health;

    using Xunit;

    public class HealthReportTests
    {
        public static TheoryData<IEnumerable<HealthStatus>, HealthStatus> CompositeStatusData =>
            new TheoryData<IEnumerable<HealthStatus>, HealthStatus>
            {
                { new[] { HealthStatus.Unhealthy }, HealthStatus.Unhealthy },
                { new[] { HealthStatus.Healthy }, HealthStatus.Healthy },
                { new[] { HealthStatus.Healthy, HealthStatus.Healthy }, HealthStatus.Healthy },
                { new[] { HealthStatus.Healthy, HealthStatus.Unhealthy }, HealthStatus.Unhealthy },
                { new[] { HealthStatus.Unhealthy, HealthStatus.Healthy }, HealthStatus.Unhealthy },
                { new[] { HealthStatus.Unhealthy, HealthStatus.Unhealthy }, HealthStatus.Unhealthy },
                { new[] { HealthStatus.Degraded, HealthStatus.Starting }, HealthStatus.Starting }
            };

        [Theory]
        [MemberData(nameof(CompositeStatusData))]
        public void CompositeStatus(IEnumerable<HealthStatus> statuses, HealthStatus expected)
        {
            var report = new HealthReport(
                                          statuses.ToDictionary(
                                                                x => Guid.NewGuid().ToString(),
                                                                x => new HealthCheckEntry(
                                                                                          new HealthCheckResult(x, string.Empty, null),
                                                                                          TimeSpan.Zero,
                                                                                          Enumerable.Empty<string>())),
                                          TimeSpan.Zero);

            Assert.Equal(expected, report.Status);
        }
    }
}
