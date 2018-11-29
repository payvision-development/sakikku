// -----------------------------------------------------------------------
// <copyright file="PolledObservableTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Diagnostics.Health.Reactive;

    using Microsoft.Reactive.Testing;

    using Xunit;

    public class PolledObservableTests : ReactiveTest
    {
        public static TheoryData<TimeSpan, TimeSpan> SubscribeData =>
            new TheoryData<TimeSpan, TimeSpan>
            {
                { TimeSpan.Zero, TimeSpan.FromSeconds(1) },
                { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1) }
            };

        [Theory]
        [MemberData(nameof(SubscribeData))]
        public void Subscribe_InvocationsPerMinute(TimeSpan sourceDuration, TimeSpan pollingInterval)
        {
            TimeSpan executionDuration = TimeSpan.FromMinutes(1);
            var scheduler = new TestScheduler();
            var timesCalled = 0;
            IObservable<int> source = Observable.FromAsync(
                                                           () =>
                                                           {
                                                               scheduler.Sleep(sourceDuration.Ticks);
                                                               return Task.FromResult(Interlocked.Increment(ref timesCalled));
                                                           });

            using (var subject = new PollingObservable<int>(source, pollingInterval, scheduler))
            using (subject.Subscribe(scheduler.CreateObserver<int>()))
            {
                scheduler.AdvanceTo(executionDuration.Ticks);
                long period = Math.Max(sourceDuration.Ticks, pollingInterval.Ticks);
                Assert.Equal(executionDuration.Ticks / period, timesCalled);
            }
        }

        [Fact]
        public void Subscribe_WaitUntilFirstResult_OnNext()
        {
            const string Expected = "OK";
            TimeSpan pollingInterval = TimeSpan.FromMinutes(1);
            TimeSpan executionDuration = TimeSpan.FromSeconds(10);

            var scheduler = new TestScheduler();
            IObservable<string> source = Observable.FromAsync(
                                                              ct =>
                                                              {
                                                                  scheduler.Sleep(executionDuration.Ticks);
                                                                  return Task.FromResult(Expected);
                                                              });
            ITestableObserver<string> observer = scheduler.CreateObserver<string>();

            using (var subject = new PollingObservable<string>(source, pollingInterval, scheduler))
            using (subject.Subscribe(observer))
            {
                scheduler.AdvanceTo(executionDuration.Ticks + 1);
            }

            observer.Messages.AssertEqual(
                                          OnNext<string>(executionDuration.Ticks + 1, x => Expected == x),
                                          OnCompleted<string>(executionDuration.Ticks + 1));
        }

        [Fact]
        public void Subscribe_CancelledBeforeFirstResult_EmptyMessages()
        {
            var scheduler = new TestScheduler();
            IObservable<string> source = Observable.FromAsync(
                                                              async ct =>
                                                              {
                                                                  await scheduler.Sleep(TimeSpan.FromSeconds(1), ct);
                                                                  return "OK";
                                                              });
            ITestableObserver<string> observer = scheduler.CreateObserver<string>();

            using (var subject = new PollingObservable<string>(source, TimeSpan.FromMinutes(1), scheduler))
            using (subject.Subscribe(observer))
            {
                scheduler.AdvanceTo(TimeSpan.FromMilliseconds(500).Ticks);
            }

            Assert.Empty(observer.Messages);
        }

        [Fact]
        public void Subscribe_SourceTimeout_TimeoutExcptionExpected()
        {
            var scheduler = new TestScheduler();
            IObservable<string> source = Observable.FromAsync(
                                                              async ct =>
                                                              {
                                                                  await scheduler.Sleep(TimeSpan.FromSeconds(10), ct);
                                                                  return "OK";
                                                              })
                .Timeout(TimeSpan.FromMilliseconds(500), scheduler);
            var observer = scheduler.CreateObserver<string>();

            using (var subject = new PollingObservable<string>(source, TimeSpan.FromMilliseconds(100), scheduler))
            using (subject.Subscribe(observer))
            {
                Assert.Throws<TimeoutException>(() => scheduler.AdvanceTo(TimeSpan.FromMilliseconds(501).Ticks));
            }
        }
    }
}