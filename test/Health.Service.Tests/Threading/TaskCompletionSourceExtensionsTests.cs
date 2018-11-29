// -----------------------------------------------------------------------
// <copyright file="TaskCompletionSourceExtensionsTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Diagnostics.Health.Threading;

    using Xunit;

    public class TaskCompletionSourceExtensionsTests
    {
        [Fact]
        public async Task WaitAsync_TaskCompleted_Ok()
        {
            var expected = new object();
            var tcs = new TaskCompletionSource<object>();
            var cts = new CancellationTokenSource();

            Task<object> task = tcs.WaitAsync(cts.Token);

            Assert.False(tcs.Task.IsCompleted);
            tcs.SetResult(expected);
            Assert.StrictEqual(expected, await task);
        }

        [Fact]
        public async Task WaitAsync_Cancelled_OperationCanceledException()
        {
            var tcs = new TaskCompletionSource<object>();
            var cts = new CancellationTokenSource();

            Task<object> task = tcs.WaitAsync(cts.Token);

            Assert.False(tcs.Task.IsCompleted);
            cts.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
        }

        [Fact]
        public async Task WaitAsync_AlreadyCancelledToken_OperationCanceledException()
        {
            var tcs = new TaskCompletionSource<object>();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await tcs.WaitAsync(cts.Token));
        }
    }
}