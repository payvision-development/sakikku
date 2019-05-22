namespace Health.Service.Tests.Rx
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NSubstitute;

    using Payvision.Diagnostics.Health;
    using Payvision.Diagnostics.Health.Rx;

    using Xunit;

    public class ReactiveHealthServiceTests
    {
        [Fact]
        public async Task CheckAsync_ObservableAwaited()
        {
            var expected = new HealthReport(
                new[]
                {
                    new HealthCheckEntry("TEST", HealthStatus.Healthy, "OK", TimeSpan.Zero)
                },
                TimeSpan.Zero);
            var disposable = Substitute.For<IDisposable>();

            var sut = new ReactiveHealthService(Observable.Return(expected), disposable);
            HealthReport result = await sut.CheckAsync(CancellationToken.None);

            result.Should().BeSameAs(expected);
        }

        [Fact]
        public void Dispose_InnerDisposableCalled()
        {
            var source = Substitute.For<IObservable<HealthReport>>();
            var disposable = Substitute.For<IDisposable>();

            var sut = new ReactiveHealthService(source, disposable);
            sut.Dispose();

            disposable.Received().Dispose();
        }
    }
}