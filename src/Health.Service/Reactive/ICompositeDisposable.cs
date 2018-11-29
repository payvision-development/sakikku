// -----------------------------------------------------------------------
// <copyright file="ICompositeDisposable.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Reactive
{
    using System;

    /// <summary>
    /// Associates different object life cycles.
    /// </summary>
    internal interface ICompositeDisposable
    {
        /// <summary>
        /// Attaches the specified disposable object to be disposed among this life cycle.
        /// </summary>
        /// <param name="disposable">The disposable to relate.</param>
        void Attach(IDisposable disposable);
    }
}