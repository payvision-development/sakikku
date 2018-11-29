// -----------------------------------------------------------------------
// <copyright file="HealthServiceTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests
{
    using System;

    using Diagnostics.Health;

    using NSubstitute;

    using Xunit;

    public class HealthServiceTests
    {
        [Fact]
        public void Create()
        {
            var configure = Substitute.For<Action<IHealthCheckSet>>();

            IHealthServiceBuilder builder = HealthService.Create(configure);

            Assert.NotNull(builder);
            configure.Received().Invoke(Arg.Any<IHealthCheckSet>());
        }
    }
}