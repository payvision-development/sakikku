// -----------------------------------------------------------------------
// <copyright file="HealthCheckExtensionsTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading;

    using Diagnostics.Health;
    using Diagnostics.Health.Reactive;

    using Microsoft.Reactive.Testing;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using Xunit;

    public class HealthCheckExtensionsTests : ReactiveTest
    {
        [Fact]
        public void ToObservable_Ok_OnNext()
        {
            var expected = new HealthCheckResult(HealthStatus.Healthy, "OK", new Dictionary<string, string> { ["Name"] = "Test" });

            var scheduler = new TestScheduler();
            var healthCheck = Substitute.For<IHealthCheck>();
            healthCheck.CheckAsync(Arg.Any<CancellationToken>())
                .Returns(info => new HealthCheckResult(expected.Status, expected.Message, expected.Data));

            ITestableObserver<HealthCheckResult> result = scheduler.Start(() => healthCheck.ToObservable(scheduler));

            result.Messages.AssertEqual(
                                        OnNext<HealthCheckResult>(Subscribed + 1, entry => expected.AreEqual(entry)),
                                        OnCompleted<HealthCheckResult>(Subscribed + 1));
        }

        [Fact]
        public void ToObservable_Exception_OnNext()
        {
            var expected = new HealthCheckResult(
                                                 HealthStatus.Unhealthy,
                                                 "Exception expected",
                                                 new Dictionary<string, string> { ["Name"] = "Test" });

            var scheduler = new TestScheduler();
            var healthCheck = Substitute.For<IHealthCheck>();
            healthCheck.CheckAsync(Arg.Any<CancellationToken>())
                .Throws(
                        info =>
                        {
                            var exception = new NotImplementedException(expected.Message);
                            foreach (KeyValuePair<string, string> entry in expected.Data)
                            {
                                exception.Data[entry.Key] = entry.Value;
                            }

                            return exception;
                        });

            ITestableObserver<HealthCheckResult> result = scheduler.Start(() => healthCheck.ToObservable(scheduler));

            result.Messages.AssertEqual(
                                        OnNext<HealthCheckResult>(Subscribed + 1, entry => expected.AreEqual(entry)),
                                        OnCompleted<HealthCheckResult>(Subscribed + 1));
        }

        [Fact]
        public void ToHealthCheckEntries_Ok_OnNext()
        {
            var expected = new HealthCheckEntry(
                                                HealthStatus.Healthy,
                                                "OK",
                                                TimeSpan.FromMilliseconds(55),
                                                new Dictionary<string, string> { ["Name"] = "Test" },
                                                new[] { "TEST", "OK" });
            var scheduler = new TestScheduler();
            ITestableObservable<HealthCheckResult> source =
                scheduler.CreateHotObservable(
                                              OnNext(expected.Duration.Ticks, new HealthCheckResult(expected.Status, expected.Message, expected.Data)),
                                              OnCompleted<HealthCheckResult>(expected.Duration.Ticks));

            ITestableObserver<HealthCheckEntry> result = scheduler.CreateObserver<HealthCheckEntry>();
            using (source.ToHealthCheckEntries(expected.Tags, scheduler).Subscribe(result))
            {
                scheduler.Start();
            }

            result.Messages.AssertEqual(
                                          OnNext<HealthCheckEntry>(expected.Duration.Ticks, x => expected.AreEqual(x)),
                                          OnCompleted<HealthCheckEntry>(scheduler.Clock));
        }
    }
}