namespace Health.Service.Tests.Rx
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Reactive.Testing;

    using NSubstitute;

    using Payvision.Diagnostics.Health;
    using Payvision.Diagnostics.Health.Rx;

    using Xunit;

    public class HealthPolicyCollectionTests : ReactiveTest
    {
        [Fact]
        public void Build_MergedSequences_Ok()
        {
            var scheduler = new TestScheduler();
            var firstEntry = new HealthCheckEntry("FIRST", HealthStatus.Healthy, "FIRST OK", TimeSpan.FromTicks(1));
            var secondEntry = new HealthCheckEntry("SECOND", HealthStatus.Healthy, "SECOND OK", TimeSpan.FromTicks(1));
            var first = Substitute.For<IHealthPolicy>();
            first.CheckAsync(Arg.Any<CancellationToken>()).Returns(new HealthCheck(firstEntry.Status, firstEntry.Message));
            var second = Substitute.For<IHealthPolicy>();
            second.CheckAsync(Arg.Any<CancellationToken>()).Returns(new HealthCheck(secondEntry.Status, secondEntry.Message));
            var collection = new HealthPolicyCollection();
            collection.Add(firstEntry.Policy, first);
            collection.Add(secondEntry.Policy, second);

            var observer = scheduler.Start(() => collection.Build(scheduler, Substitute.For<ICollection<IDisposable>>()));

            long scheduledTicks = Subscribed + 1;
            observer.Messages.AssertEqual(
                OnNext<HealthCheckEntry>(
                    scheduledTicks += firstEntry.Duration.Ticks,
                    x => x.Duration == firstEntry.Duration &&
                         x.Policy == firstEntry.Policy &&
                         x.Status == firstEntry.Status &&
                         x.Message == firstEntry.Message),
                OnNext<HealthCheckEntry>(
                    scheduledTicks += secondEntry.Duration.Ticks,
                    x => x.Duration == secondEntry.Duration &&
                         x.Policy == secondEntry.Policy &&
                         x.Status == secondEntry.Status &&
                         x.Message == secondEntry.Message),
                OnCompleted<HealthCheckEntry>(scheduledTicks));
        }
    }
}