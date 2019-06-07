namespace Health.Service.Tests.Rx
{
    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;

    using FluentAssertions;

    using Microsoft.Reactive.Testing;

    using Payvision.Diagnostics.Health.Rx;

    using Xunit;

    public class PollingObservableTests : ReactiveTest
    {
        [Theory]
        [InlineData(7)]
        [InlineData(0)]
        [InlineData(50)]
        public void Subscribe_LastPolled(int numberOfTicks)
        {
            var pollingInterval = TimeSpan.FromMilliseconds(100);
            if (numberOfTicks == 0)
            {
                numberOfTicks++; // Polling is executed before first tick
            }

            var subscriptionTime = pollingInterval.Ticks * (numberOfTicks > 0 ? numberOfTicks : 1);

            var scheduler = new TestScheduler();
            int cont = 0;
            IObservable<int> source = Observable.Create<int>(x =>
            {
                x.OnNext(Interlocked.Increment(ref cont));
                x.OnCompleted();
                return Disposable.Empty;
            });

            ITestableObserver<int> observer = scheduler.CreateObserver<int>();
            using (var sut = new PollingObservable<int>(source, pollingInterval, scheduler))
            {
                scheduler.AdvanceTo(subscriptionTime);
                using (sut.Subscribe(observer))
                {
                    scheduler.AdvanceBy(1);
                }
            }

            observer.Messages.AssertEqual(
                OnNext<int>(subscriptionTime + 1, x => x == numberOfTicks && cont == numberOfTicks + 1),
                OnCompleted<int>(subscriptionTime + 1));
        }


        [Fact]
        public void Subscribe_WaitToFirstTick_OneMessage()
        {
            long executionTicks = TimeSpan.FromMilliseconds(100).Ticks + 1;
            var scheduler = new TestScheduler();
            int cont = 0;
            IObservable<int> source = Observable.Create<int>(x =>
            {
                scheduler.Sleep(executionTicks - 1);
                x.OnNext(Interlocked.Increment(ref cont));
                x.OnCompleted();
                return Disposable.Empty;
            });

            ITestableObserver<int> observer = scheduler.CreateObserver<int>();
            using (var sut = new PollingObservable<int>(source, TimeSpan.FromHours(1), scheduler))
            using (sut.Subscribe(observer))
            {
                scheduler.AdvanceTo(executionTicks);
            }

            observer.Messages.AssertEqual(
                OnNext<int>(executionTicks, x => x == 1),
                OnCompleted<int>(executionTicks));
        }

        [Fact]
        public void Subscribe_StartsImmediately()
        {
            long expectedTicks = 1;
            var scheduler = new TestScheduler();
            int cont = 0;
            IObservable<int> source = Observable.Create<int>(x =>
            {
                x.OnNext(Interlocked.Increment(ref cont));
                x.OnCompleted();
                return Disposable.Empty;
            });

            ITestableObserver<int> observer = scheduler.CreateObserver<int>();
            using (var sut = new PollingObservable<int>(source, TimeSpan.FromHours(1), scheduler))
            using (sut.Subscribe(observer))
            {
                scheduler.AdvanceTo(expectedTicks);
            }

            observer.Messages.AssertEqual(
                OnNext<int>(expectedTicks, x => x == 1),
                OnCompleted<int>(expectedTicks));
        }

        [Fact]
        public void Subscribe_OverlappedTicks_OnlyFirstExecuted()
        {
            var executionDuration = TimeSpan.FromMilliseconds(300);
            var scheduler = new TestScheduler();
            int calls = 0;
            IObservable<Unit> source = Observable.Create<Unit>(_ =>
            {
                Interlocked.Increment(ref calls);
                scheduler.Sleep(executionDuration.Ticks);
                return Disposable.Empty;
            });

            using (var sut = new PollingObservable<Unit>(source, TimeSpan.FromMilliseconds(100), scheduler))
            {
                scheduler.AdvanceTo(executionDuration.Ticks);
            }

            calls.Should().Be(1);
        }

        [Fact]
        public void Subscribe_CancelledBeforeFirstResult_EmptyMessages()
        {
            long executionTicks = TimeSpan.FromMinutes(1).Ticks;
            long waitingTicks = TimeSpan.FromMilliseconds(100).Ticks;
            var scheduler = new TestScheduler();
            IObservable<int> source = Observable.Create<int>(x =>
            {
                scheduler.Sleep(executionTicks);
                return Disposable.Empty;
            });

            ITestableObserver<int> observer = scheduler.CreateObserver<int>();
            using (var sut = new PollingObservable<int>(source, TimeSpan.FromHours(1), scheduler))
            using (sut.Subscribe(observer))
            {
                scheduler.AdvanceTo(waitingTicks);
            }

            scheduler.Clock.Should().Be(waitingTicks);
            observer.Messages.Should().BeEmpty();
        }
    }
}