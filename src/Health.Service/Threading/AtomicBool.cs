// -----------------------------------------------------------------------
// <copyright file="AtomicBool.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Diagnostics.Health.Threading
{
    using System.Threading;

    /// <summary>
    /// Contains a boolean value that allows atomic operations.
    /// </summary>
    internal sealed class AtomicBool
    {
        private const int TrueIntValue = 1;

        private const int FalseIntValue = 0;

        private int currentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicBool"/> class with false value.
        /// </summary>
        public AtomicBool()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicBool"/> class with the specified value.
        /// </summary>
        /// <param name="initialValue">The boolean initial value.</param>
        public AtomicBool(bool initialValue)
        {
            this.Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the current value in a non-thread-sage access.
        /// </summary>
        public bool Value
        {
            get => this.currentValue == TrueIntValue;

            set => this.currentValue = ToInt(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="bool"/> to <see cref="AtomicBool"/>.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator AtomicBool(bool value) => new AtomicBool(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="AtomicBool"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="value">The <see cref="AtomicBool"/> value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator bool(AtomicBool value) => value?.Value ?? false;

        /// <summary>
        /// Exchanges the current value to the specified one.
        /// </summary>
        /// <param name="value">The value to which this instance is set.</param>
        /// <returns><c>true</c> if exchanged, false otherwise.</returns>
        public bool Exchange(bool value)
        {
            int valueInt = ToInt(value);
            return Interlocked.Exchange(ref this.currentValue, ToInt(value)) != valueInt;
        }

        /// <summary>
        /// Compares two value for equality, and, if they are equal, replaces the first value.
        /// </summary>
        /// <param name="value">The value that replaces the destination value if the comparison results in equality.</param>
        /// <param name="comparand">The value that is compared to the current value.</param>
        /// <returns><c>true</c> if exchanged, false otherwise.</returns>
        public bool CompareExchange(bool value, bool comparand)
        {
            int valueInt = ToInt(value);
            int comparandInt = ToInt(comparand);
            return Interlocked.CompareExchange(ref this.currentValue, valueInt, comparandInt) == comparandInt;
        }

        /// <inheritdoc />
        public override string ToString() => this.Value.ToString();

        private static int ToInt(bool value) => value ? TrueIntValue : FalseIntValue;
    }
}