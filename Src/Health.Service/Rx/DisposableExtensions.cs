using System;
using System.Reactive.Disposables;

namespace Payvision.Diagnostics.Health.Rx
{
    /// <summary>
    /// <see cref="IDisposable"/> extension methods.
    /// </summary>
    internal static class DisposableExtensions
    {
        /// <summary>
        /// Combines the two specified disposable instances.
        /// </summary>
        /// <param name="first">The first disposable.</param>
        /// <param name="second">The second disposable.</param>
        /// <returns>The composite disposable.</returns>
        public static IDisposable Combine(this IDisposable first, IDisposable second)
        {
            if (first == null)
            {
                return second;
            }

            if (second == null)
            {
                return first;
            }

            if (first is CompositeDisposable composite)
            {
                composite.Add(second);
                return first;
            }

            return new CompositeDisposable(first, second);
        }
    }
}
