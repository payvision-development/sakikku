// -----------------------------------------------------------------------
// <copyright file="ReactiveHealthServiceTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Diagnostics.Health;
    using Diagnostics.Health.Reactive;

    using NSubstitute;

    using Xunit;

    public class ReactiveHealthServiceTests
    {
        [Fact]
        public async Task CheckAsync_Ok()
        {
            var expected = new HealthReport(
                                            new Dictionary<string, HealthCheckEntry>
                                            {
                                                ["test"] = new HealthCheckEntry(
                                                                                HealthStatus.Healthy,
                                                                                "OK",
                                                                                TimeSpan.FromMilliseconds(300),
                                                                                new Dictionary<string, string> { ["TEST"] = "OK" },
                                                                                new[] { "TEST", "OK" })
                                            },
                                            TimeSpan.FromMilliseconds(300));
            var builder = Substitute.For<IObservableBuilder<HealthReport>>();
            builder.Build(Arg.Any<IScheduler>(), Arg.Any<ICompositeDisposable>())
                .Returns(Observable.Return(expected));

            using (var subject = new ReactiveHealthService(builder))
            {
                HealthReport result = await subject.CheckAsync(CancellationToken.None);

                Assert.True(expected.AreEqual(result));
            }
        }

        [Fact]
        public void Dispose_DisposedHealthChecks()
        {
            var disposable = Substitute.For<IDisposable>();
            var builder = Substitute.For<IObservableBuilder<HealthReport>>();
            builder.Build(Arg.Any<IScheduler>(), Arg.Any<ICompositeDisposable>())
                .Returns(info =>
                {
                    info.Arg<ICompositeDisposable>().Attach(disposable);
                    return Substitute.For<IObservable<HealthReport>>();
                });
            var subject = new ReactiveHealthService(builder);

            subject.Dispose();

            disposable.Received().Dispose();
        }
    }
}