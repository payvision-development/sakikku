// -----------------------------------------------------------------------
// <copyright file="HealthCheckSetTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Threading;
    using System.Threading.Tasks;

    using Diagnostics.Health;
    using Diagnostics.Health.Reactive;

    using Microsoft.Reactive.Testing;

    using NSubstitute;

    using Xunit;

    public class HealthCheckSetTests : ReactiveTest
    {
        [Fact]
        public void Add_ExistingName_ArgumentException()
        {
            IHealthCheckSet set = new HealthCheckSet();
            set.Add("TEST");
            Assert.Throws<ArgumentException>(() => set.Add("TEST"));
        }

        [Fact]
        public void Build_Ok()
        {
            var expected = new HealthReport(
                                            new Dictionary<string, HealthCheckEntry>
                                            {
                                                ["first"] = new HealthCheckEntry(
                                                                                 HealthStatus.Healthy,
                                                                                 "first",
                                                                                 TimeSpan.FromMilliseconds(33),
                                                                                 new Dictionary<string, string>(),
                                                                                 new string[0]),
                                                ["second"] = new HealthCheckEntry(
                                                                                  HealthStatus.Healthy,
                                                                                  "first",
                                                                                  TimeSpan.FromMilliseconds(15),
                                                                                  new Dictionary<string, string>(),
                                                                                  new string[0])
                                            },
                                            TimeSpan.FromMilliseconds(33));
            var scheduler = new TestScheduler();

            async Task<HealthCheckResult> ToResult(HealthCheckEntry entry)
            {
                await scheduler.Sleep(entry.Duration.Add(TimeSpan.FromTicks(-1)));
                return new HealthCheckResult(entry.Status, entry.Message, entry.Data);
            }

            var firstCheck = Substitute.For<IHealthCheck>();
            firstCheck.CheckAsync(Arg.Any<CancellationToken>()).Returns(info => ToResult(expected.Entries["first"]));
            var secondCheck = Substitute.For<IHealthCheck>();
            secondCheck.CheckAsync(Arg.Any<CancellationToken>()).Returns(info => ToResult(expected.Entries["second"]));

            var subject = new HealthCheckSet();
            subject.Add("first").For(firstCheck);
            subject.Add("second").For(secondCheck);
            ITestableObserver<HealthReport> observer = scheduler.CreateObserver<HealthReport>();

            using (subject.Build(scheduler).Subscribe(observer))
            {
                scheduler.Start();
            }

            observer.Messages.AssertEqual(
                                          OnNext<HealthReport>(
                                                               expected.Entries["first"].Duration.Ticks,
                                                               report => expected.AreEqual(report)),
                                          OnCompleted<HealthReport>(scheduler.Clock));
        }
    }
}