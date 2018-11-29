// -----------------------------------------------------------------------
// <copyright file="TaskCompletionSourceExtensions.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides <see cref="TaskCompletionSourceExtensions"/> extension methods.
    /// </summary>
    internal static class TaskCompletionSourceExtensions
    {
        /// <summary>
        /// Waits for the task completion source to complete, or for the cancellation token to be canceled.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tcs">The <see cref="TaskCompletionSource{TResult}"/> to wait for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous task.</returns>
        /// <exception cref="ArgumentNullException">tcs</exception>
        public static async Task<TResult> WaitAsync<TResult>(
            this TaskCompletionSource<TResult> tcs,
            CancellationToken cancellationToken)
        {
            if (tcs == null)
            {
                throw new ArgumentNullException(nameof(tcs));
            }

            var cancelTaskSource = new TaskCompletionSource<TResult>();
            using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            using (linkedTokenSource.Token.Register(() => cancelTaskSource.TrySetResult(default)))
            {
                try
                {
                    await Task.WhenAny(tcs.Task, cancelTaskSource.Task);
                }
                catch
                {
                    // capture in further await.
                }

                if (tcs.Task.IsCompleted)
                {
                    return await tcs.Task;
                }

                throw new OperationCanceledException();
            }
        }
    }
}