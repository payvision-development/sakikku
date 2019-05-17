namespace Health.Service.Tests.Rx
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

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

            var sut = new ReactiveHealthService(Observable.Return(expected));
            HealthReport result = await sut.CheckAsync(CancellationToken.None);

            result.Should().BeSameAs(expected);
        }
    }
}