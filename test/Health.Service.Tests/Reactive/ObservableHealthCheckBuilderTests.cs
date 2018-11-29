// -----------------------------------------------------------------------
// <copyright file="ObservableHealthCheckBuilderTests.cs" company="Payvision">
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

    public class ObservableHealthCheckBuilderTests : ReactiveTest
    {
        [Fact]
        public void Build_Tags_Ok()
        {
            var expected = new HealthCheckEntry(
                                                HealthStatus.Healthy,
                                                "OK",
                                                TimeSpan.FromMilliseconds(35),
                                                new Dictionary<string, string> { ["TEST"] = "OK" },
                                                new[] { "TEST", "OK" });
            var scheduler = new TestScheduler();
            var healthCheck = Substitute.For<IHealthCheck>();
            healthCheck.CheckAsync(Arg.Any<CancellationToken>())
                .Returns(
                         _ =>
                         {
                             scheduler.Sleep(expected.Duration.Ticks - 1);
                             return new HealthCheckResult(expected.Status, expected.Message, expected.Data);
                         });
            var disposable = Substitute.For<ICompositeDisposable>();

            var subject = new ObservableHealthCheckBuilder(healthCheck);
            subject.Tags(expected.Tags);
            ITestableObserver<HealthCheckEntry> observer = scheduler.CreateObserver<HealthCheckEntry>();
            using (subject.Build(scheduler, disposable).Subscribe(observer))
            {
                scheduler.Start();
            }

            observer.Messages.AssertEqual(
                                          OnNext<HealthCheckEntry>(expected.Duration.Ticks, x => expected.AreEqual(x)),
                                          OnCompleted<HealthCheckEntry>(expected.Duration.Ticks));
            disposable.DidNotReceiveWithAnyArgs().Attach(Arg.Any<IDisposable>());
        }

        [Fact]
        public void Build_Polled_DisposeCalled()
        {
            var expected = new HealthCheckEntry(
                                                HealthStatus.Healthy,
                                                "OK",
                                                TimeSpan.FromMilliseconds(35),
                                                new Dictionary<string, string> { ["TEST"] = "OK" },
                                                new string[0]);
            var scheduler = new TestScheduler();
            var healthCheck = Substitute.For<IHealthCheck>();
            healthCheck.CheckAsync(Arg.Any<CancellationToken>())
                .Returns(
                         _ =>
                         {
                             scheduler.Sleep(expected.Duration.Ticks - 1); // scheduling on the same scheduler will add an extra tick.
                             return new HealthCheckResult(expected.Status, expected.Message, expected.Data);
                         });
            IDisposable pollingHandler = null;
            var disposable = Substitute.For<ICompositeDisposable>();
            disposable.WhenForAnyArgs(x => x.Attach(Arg.Any<IDisposable>())).Do(info => pollingHandler = info.Arg<IDisposable>());

            var subject = new ObservableHealthCheckBuilder(healthCheck);
            subject.Polling(TimeSpan.FromSeconds(1));
            ITestableObserver<HealthCheckEntry> observer = scheduler.CreateObserver<HealthCheckEntry>();
            using(subject.Build(scheduler, disposable).Subscribe(observer))
            using (pollingHandler)
            {
                scheduler.AdvanceTo(expected.Duration.Ticks + 5);
            }

            observer.Messages.AssertEqual(
                                          OnNext<HealthCheckEntry>(expected.Duration.Ticks + 1, x => expected.AreEqual(x)),
                                          OnCompleted<HealthCheckEntry>(expected.Duration.Ticks + 1));
            disposable.Received().Attach(Arg.Any<IDisposable>());
        }

        [Fact]
        public void Build_Polling_ZeroTimeInterval()
        {
            var expected = new HealthCheckEntry(
                                                HealthStatus.Healthy,
                                                "OK",
                                                TimeSpan.FromMilliseconds(35),
                                                new Dictionary<string, string> { ["TEST"] = "OK" },
                                                new string[0]);
            var scheduler = new TestScheduler();
            var healthCheck = Substitute.For<IHealthCheck>();
            healthCheck.CheckAsync(Arg.Any<CancellationToken>())
                .Returns(
                         _ =>
                         {
                             scheduler.Sleep(expected.Duration.Ticks - 1); // scheduling on the same scheduler will add an extra tick.
                             return new HealthCheckResult(expected.Status, expected.Message, expected.Data);
                         });
            IDisposable pollingHandler = null;
            var disposable = Substitute.For<ICompositeDisposable>();
            disposable.WhenForAnyArgs(x => x.Attach(Arg.Any<IDisposable>())).Do(info => pollingHandler = info.Arg<IDisposable>());
            var subject = new ObservableHealthCheckBuilder(healthCheck);
            subject.Polling(TimeSpan.FromMinutes(1));

            IObservable<HealthCheckEntry> sequence = subject.Build(scheduler, disposable);
            using (pollingHandler)
            {
                ITestableObserver<HealthCheckEntry> observer = scheduler.CreateObserver<HealthCheckEntry>();
                using (sequence.Subscribe(observer))
                {
                    scheduler.AdvanceTo(expected.Duration.Ticks + 1);
                    observer.Messages.AssertEqual(
                                                  OnNext<HealthCheckEntry>(expected.Duration.Ticks + 1, x => expected.AreEqual(x)),
                                                  OnCompleted<HealthCheckEntry>(expected.Duration.Ticks + 1));
                }

                observer = scheduler.CreateObserver<HealthCheckEntry>();
                using (sequence.Subscribe(observer))
                {
                    scheduler.AdvanceBy(1);
                    observer.Messages.AssertEqual(
                                                  OnNext<HealthCheckEntry>(scheduler.Clock, x => expected.AreEqual(x)),
                                                  OnCompleted<HealthCheckEntry>(scheduler.Clock));
                }
            }

            disposable.Received().Attach(Arg.Any<IDisposable>());
        }
    }
}