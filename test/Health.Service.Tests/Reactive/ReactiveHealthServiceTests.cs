// -----------------------------------------------------------------------
// <copyright file="ReactiveHealthServiceTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Disposables;
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
                                                ["first"] = new HealthCheckEntry(
                                                                                 HealthStatus.Healthy,
                                                                                 "test",
                                                                                 TimeSpan.FromMilliseconds(7),
                                                                                 new Dictionary<string, string>(),
                                                                                 new string[0])
                                            },
                                            TimeSpan.FromMilliseconds(30));
            var sequence = Substitute.For<IObservable<HealthReport>>();
            sequence
                .Subscribe(Arg.Any<IObserver<HealthReport>>())
                .Returns(
                         info =>
                         {
                             var observer = info.Arg<IObserver<HealthReport>>();
                             observer.OnNext(expected);
                             observer.OnCompleted();
                             return Disposable.Empty;
                         });
            var service = new ReactiveHealthService(sequence);

            HealthReport result = await service.CheckAsync(CancellationToken.None);

            Assert.True(expected.AreEqual(result));
        }
    }
}