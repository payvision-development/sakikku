// -----------------------------------------------------------------------
// <copyright file="HealthCheckFactoryExtensionsTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Diagnostics.Health;

    using NSubstitute;

    using Xunit;

    public class HealthCheckFactoryExtensionsTests
    {
        [Fact]
        public void For_Generic_Ok()
        {
            var factory = Substitute.For<IHealthCheckFactory>();

            factory.For<MockHealthCheck>();

            factory.Received().For(Arg.Any<MockHealthCheck>());
        }

        #region MockHealthCheck

        private sealed class MockHealthCheck : IHealthCheck
        {
            /// <inheritdoc />
            public Task<HealthCheckResult> CheckAsync(CancellationToken cancellationToken) =>
                Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "TEST", new Dictionary<string, string>()));
        }

        #endregion
    }
}