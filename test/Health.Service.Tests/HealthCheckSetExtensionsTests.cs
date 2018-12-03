// -----------------------------------------------------------------------
// <copyright file="HealthCheckSetExtensionsTests.cs" company="Payvision">
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

    public class HealthCheckSetExtensionsTests
    {
        [Fact]
        public void For_Generic_Ok()
        {
            const string ExpectedName = "TEST";
            var subject = Substitute.For<IHealthCheckSet>();

            subject.Add<MockHealthCheck>(ExpectedName);

            subject.Received().Add(ExpectedName, Arg.Any<MockHealthCheck>());
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