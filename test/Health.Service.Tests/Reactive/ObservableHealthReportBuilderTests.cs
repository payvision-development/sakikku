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
            TimeSpan firstHealthDuration = TimeSpan.FromMilliseconds(200);
            TimeSpan secondHealthDuration = TimeSpan.FromMilliseconds(300);
            var expected = new HealthReport(
                                            new Dictionary<string, HealthCheckEntry>
                                            {
                                                ["firstEntry"] = new HealthCheckEntry(
                                                                                      HealthStatus.Healthy,
                                                                                      "FIRST",
                                                                                      firstHealthDuration + secondHealthDuration + TimeSpan.FromTicks(-1),
                                                                                      new Dictionary<string, string>
                                                                                      {
                                                                                          ["Policy"] = "first"
                                                                                      },
                                                                                      new[] { "FIRST" }),
                                                ["secondEntry"] = new HealthCheckEntry(
                                                                                       HealthStatus.Healthy,
                                                                                       "SECOND",
                                                                                       secondHealthDuration,
                                                                                       new Dictionary<string, string>
                                                                                       {
                                                                                           ["Policy"] = "second"
                                                                                       },
                                                                                       new[] { "SECOND" })
                                            },
                                            firstHealthDuration + secondHealthDuration);

            var scheduler = new TestScheduler();
            var firstCheck = Substitute.For<IHealthCheck>();
            firstCheck.CheckAsync(Arg.Any<CancellationToken>()).Returns(_ =>
            {
                HealthCheckEntry entry = expected.Entries["firstEntry"];
                scheduler.Sleep(firstHealthDuration.Ticks);
                return new HealthCheckResult(entry.Status, entry.Message, entry.Data);
            });
            var secondCheck = Substitute.For<IHealthCheck>();
            secondCheck.CheckAsync(Arg.Any<CancellationToken>()).Returns(_ =>
            {
                HealthCheckEntry entry = expected.Entries["secondEntry"];
                scheduler.Sleep(secondHealthDuration.Ticks - 1);
                return new HealthCheckResult(entry.Status, entry.Message, entry.Data);
            });
            var disposable = Substitute.For<ICompositeDisposable>();

            var subject = new ObservableHealthReportBuilder();
            subject.Add("firstEntry").For(firstCheck).Tags(expected.Entries["firstEntry"].Tags);
            subject.Add("secondEntry").For(secondCheck).Tags(expected.Entries["secondEntry"].Tags);
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