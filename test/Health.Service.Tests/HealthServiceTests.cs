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

            IHealthService result = HealthService.Create(configure);

            Assert.NotNull(result);
            configure.Received().Invoke(Arg.Any<IHealthCheckSet>());
        }
    }
}