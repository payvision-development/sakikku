namespace Payvision.Health.Service.Tests.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Threading;

    using Diagnostics.Health;
    using Diagnostics.Health.Reactive;

    using Microsoft.Reactive.Testing;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using Xunit;

    public class ReactiveHealthCheckTests : ReactiveTest
    {
        [Fact]
        public void Build_Ok()
        {
            var expected = new HealthCheckEntry(
                                                HealthStatus.Healthy,
                                                "OK",
                                                TimeSpan.FromMilliseconds(55),
                                                new Dictionary<string, string> { ["Name"] = "Test" },
                                                new[] { "TEST", "OK" });
            var scheduler = new TestScheduler();
            var healthCheck = Substitute.For<IHealthCheck>();
            healthCheck.CheckAsync(Arg.Any<CancellationToken>())
                .Returns(
                         async info =>
                         {
                             await scheduler.Sleep(expected.Duration.Add(TimeSpan.FromTicks(-1))); // TestScheduler will add 1 tick to execution.
                             return new HealthCheckResult(expected.Status, expected.Message, expected.Data);
                         });
            var reactiveHealthCheck = new ReactiveHealthCheck();
            reactiveHealthCheck.For(healthCheck);
            reactiveHealthCheck.Tags(expected.Tags);
            ITestableObserver<HealthCheckEntry> observer = scheduler.CreateObserver<HealthCheckEntry>();

            using (reactiveHealthCheck.Build(scheduler).Subscribe(observer))
            {
                scheduler.Start();
            }

            observer.Messages.AssertEqual(
                                          OnNext<HealthCheckEntry>(expected.Duration.Ticks, entry => expected.AreEqual(entry)),
                                          OnCompleted<HealthCheckEntry>(scheduler.Clock));
        }

        [Fact]
        public void Build_Exception()
        {
            var expected = new HealthCheckEntry(
                                                HealthStatus.Unhealthy,
                                                "test exception",
                                                TimeSpan.FromTicks(1),
                                                new Dictionary<string, string> { ["Name"] = "Test" },
                                                new[] { "TEST", "OK" });
            var healthCheck = Substitute.For<IHealthCheck>();
            healthCheck.CheckAsync(Arg.Any<CancellationToken>()).Throws(
                                                                        info =>
                                                                        {
                                                                            var exception = new NotImplementedException(expected.Message);
                                                                            foreach (KeyValuePair<string, string> pair in expected.Data)
                                                                            {
                                                                                exception.Data[pair.Key] = pair.Value;
                                                                            }

                                                                            throw exception;
                                                                        });
            var scheduler = new TestScheduler();
            var reactiveHealthCheck = new ReactiveHealthCheck();
            reactiveHealthCheck.For(healthCheck);
            reactiveHealthCheck.Tags(expected.Tags);

            var result = scheduler.Start(() => reactiveHealthCheck.Build(scheduler));

            result.Messages.AssertEqual(
                                        OnNext<HealthCheckEntry>(Subscribed + 1, entry => expected.AreEqual(entry)),
                                        OnCompleted<HealthCheckEntry>(Subscribed + 1));
        }
    }
}
