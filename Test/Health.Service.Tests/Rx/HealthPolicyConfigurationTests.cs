﻿namespace Health.Service.Tests.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Reactive.Testing;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using Payvision.Diagnostics.Health;
    using Payvision.Diagnostics.Health.Rx;

    using Xunit;

    public class HealthPolicyConfigurationTests : ReactiveTest
    {
        [Fact]
        public void Build_PolicyOk_Equivalent()
        {
            var expected = new HealthCheckEntry("TEST", HealthStatus.Healthy, "TEST OK", TimeSpan.FromTicks(1));
            var scheduler = new TestScheduler();
            var healthPolicy = Substitute.For<IHealthPolicy>();
            healthPolicy.CheckAsync(Arg.Any<CancellationToken>()).Returns(new HealthCheck(expected.Status, expected.Message));

            var sut = new HealthPolicyConfiguration(expected.Policy, healthPolicy);
            var observer = scheduler.Start(() => sut.Build(scheduler, Substitute.For<ICollection<IDisposable>>()));

            observer.Messages.AssertEqual(
                OnNext<HealthCheckEntry>(
                    Subscribed + 1,
                    x => x.Policy == expected.Policy &&
                         x.Status == expected.Status &&
                         x.Message == expected.Message &&
                         x.Duration == expected.Duration),
                OnCompleted<HealthCheckEntry>(Subscribed + 1));
        }

        [Fact]
        public void Build_PolicyException_Equivalent()
        {
            var expected = new HealthCheckEntry("TEST", HealthStatus.Unhealthy, "SHOULD MATCH", TimeSpan.FromTicks(1));
            var scheduler = new TestScheduler();
            var healthPolicy = Substitute.For<IHealthPolicy>();
            healthPolicy.CheckAsync(Arg.Any<CancellationToken>()).Throws(new NullReferenceException(expected.Message));

            var sut = new HealthPolicyConfiguration(expected.Policy, healthPolicy);
            var observer = scheduler.Start(() => sut.Build(scheduler, Substitute.For<ICollection<IDisposable>>()));

            observer.Messages.AssertEqual(
                OnNext<HealthCheckEntry>(
                    Subscribed + 1,
                    x => x.Policy == expected.Policy &&
                         x.Status == expected.Status &&
                         x.Message == expected.Message &&
                         x.Duration == expected.Duration),
                OnCompleted<HealthCheckEntry>(Subscribed + 1));
        }

        [Fact]
        public void Build_WithTimeout()
        {
            var scheduler = new TestScheduler();
            var timeout = TimeSpan.FromMilliseconds(100);
            var policy = Substitute.For<IHealthPolicy>();
            policy.CheckAsync(Arg.Any<CancellationToken>())
                .Returns(_ =>
                {
                    scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
                    return new HealthCheck(HealthStatus.Healthy, "SHOULDN'T BE RECEIVED.");
                });

            var sut = new HealthPolicyConfiguration("TEST", policy).Timeout(timeout) as HealthPolicyConfiguration;
            var observer = scheduler.CreateObserver<HealthCheckEntry>();
            sut.Build(scheduler, Substitute.For<ICollection<IDisposable>>()).Subscribe(observer).Dispose();

            observer.Messages.AssertEqual(
                OnNext<HealthCheckEntry>(
                    timeout.Ticks + 1,
                    x => x.Duration == timeout && x.Policy == "TEST" && x.Status == HealthStatus.Unhealthy),
                OnCompleted<HealthCheckEntry>(timeout.Ticks + 1));
        }
    }
}
