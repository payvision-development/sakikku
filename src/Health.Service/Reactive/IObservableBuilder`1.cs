// -----------------------------------------------------------------------
// <copyright file="IObservableBuilder`1.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Implementors builds observable sequences.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    internal interface IObservableBuilder<out TSource>
    {
        /// <summary>
        /// Builds the observable sequence as configured.
        /// </summary>
        /// <param name="scheduler">The execution scheduler.</param>
        /// <param name="disposable">The <see cref="ICompositeDisposable"/> to attach instance life cycle.</param>
        /// <returns>The <see cref="IObservable{T}"/> instance ready to subscribe to.</returns>
        IObservable<TSource> Build(IScheduler scheduler, ICompositeDisposable disposable);
    }
}