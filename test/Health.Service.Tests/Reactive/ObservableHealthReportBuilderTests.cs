// -----------------------------------------------------------------------
// <copyright file="ObservableHealthReportBuilderTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Diagnostics.Health;
    using Diagnostics.Health.Reactive;

    using Microsoft.Reactive.Testing;

    using NSubstitute;

    using Xunit;

    public class ObservableHealthReportBuilderTests : ReactiveTest
    {
        [Fact]
        public void Build_Ok()
        {
            var firstEntry = new HealthCheckEntry(
                                                  HealthStatus.Healthy,
                                                  "FIRST",
                                                  TimeSpan.FromMilliseconds(200),
                                                  new Dictionary<string, string>
                                                  {
                                                      ["Policy"] = "first"
                                                  },
                                                  new[] { "FIRST" });
            var secondEntry = new HealthCheckEntry(
                                                   HealthStatus.Healthy,
                                                   "SECOND",
                                                   TimeSpan.FromMilliseconds(300),
                                                   new Dictionary<string, string>
                                                   {
                                                       ["Policy"] = "second"
                                                   },
                                                   new[] { "SECOND" });
            var expected = new HealthReport(
                                            new Dictionary<string, HealthCheckEntry>
                                            {
                                                [nameof(firstEntry)] = firstEntry,
                                                [nameof(secondEntry)] = secondEntry
                                            },
                                            firstEntry.Duration + secondEntry.Duration + TimeSpan.FromTicks(1));

            var scheduler = new TestScheduler();
            var firstCheck = Substitute.For<IHealthCheck>();
            firstCheck.CheckAsync(Arg.Any<CancellationToken>()).Returns(_ =>
            {
                scheduler.Sleep(firstEntry.Duration.Ticks - 1);
                return new HealthCheckResult(firstEntry.Status, firstEntry.Message, firstEntry.Data);
            });
            var secondCheck = Substitute.For<IHealthCheck>();
            secondCheck.CheckAsync(Arg.Any<CancellationToken>()).Returns(_ =>
            {
                scheduler.Sleep(secondEntry.Duration.Ticks - 1);
                return new HealthCheckResult(secondEntry.Status, secondEntry.Message, secondEntry.Data);
            });
            var disposable = Substitute.For<ICompositeDisposable>();

            var subject = new ObservableHealthReportBuilder();
            subject.Add(nameof(firstEntry), firstCheck).Tags(firstEntry.Tags);
            subject.Add(nameof(secondEntry), secondCheck).Tags(secondEntry.Tags);
            ITestableObserver<HealthReport> observer = scheduler.CreateObserver<HealthReport>();

            using (subject.Build(scheduler, disposable).Subscribe(observer))
            {
                scheduler.AdvanceTo(expected.TotalDuration.Ticks + 1);
            }

            observer.Messages.AssertEqual(
                                          OnNext<HealthReport>(expected.TotalDuration.Ticks, x => expected.AreEqual(x)),
                                          OnCompleted<HealthReport>(expected.TotalDuration.Ticks));
        }
    }
}