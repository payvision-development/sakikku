namespace Payvision.Diagnostics.Health
{
    using System;

    /// <summary>
    /// Represents the status of a health check result.
    /// </summary>
    public struct HealthStatus : IEquatable<HealthStatus>, IComparable<HealthStatus>
    {
        private readonly byte value;

        private HealthStatus(byte value) => this.value = value;

        /// <summary>
        /// Gets the value indicating that the health check determined that the component is healthy.
        /// </summary>
        public static HealthStatus Healthy { get; } = new HealthStatus(0);

        /// <summary>
        /// Gets the value indicating that the application is either starting or in a warm-up state.
        /// </summary>
        public static HealthStatus Starting { get; } = new HealthStatus(1);

        /// <summary>
        /// Gets the value that indicating that the application is in a degraded state.
        /// </summary>
        public static HealthStatus Degraded { get; } = new HealthStatus(2);

        /// <summary>
        /// Gets the value indicating that the health check determined that the component is unhealthy, or
        /// an exception was thrown while executing such check.
        /// </summary>
        public static HealthStatus Unhealthy { get; } = new HealthStatus(byte.MaxValue);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="HealthStatus"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">If the specified value is an invalid value.</exception>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator HealthStatus(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "healthy":
                    return Healthy;
                case "starting":
                    return Starting;
                case "degraded":
                    return Degraded;
                case "unhealthy":
                    return Unhealthy;
                default:
                    throw new ArgumentException(nameof(value));
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="byte"/> to <see cref="HealthStatus"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">If the specified value is an invalid value.</exception>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator HealthStatus(byte value) =>
            value <= Unhealthy.value ? new HealthStatus(value) : throw new ArgumentException(nameof(value));

        /// <summary>
        /// Performs an implicit conversion from <see cref="short"/> to <see cref="HealthStatus"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">If the specified value is an invalid value.</exception>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator HealthStatus(short value) =>
            value <= Unhealthy.value ? new HealthStatus((byte)value) : throw new ArgumentException(nameof(value));

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> to <see cref="HealthStatus"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">If the specified value is an invalid value.</exception>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator HealthStatus(int value) =>
            value <= Unhealthy.value ? new HealthStatus((byte)value) : throw new ArgumentException(nameof(value));

        /// <summary>
        /// Performs an implicit conversion from <see cref="long"/> to <see cref="HealthStatus"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">If the specified value is an invalid value.</exception>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator HealthStatus(long value) =>
            value <= Unhealthy.value ? new HealthStatus((byte)value) : throw new ArgumentException(nameof(value));

        /// <summary>
        /// Performs an implicit conversion from <see cref="HealthStatus"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(HealthStatus status) => status.ToString();

        /// <summary>
        /// Performs an implicit conversion from <see cref="HealthStatus"/> to <see cref="byte"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte(HealthStatus status) => status.value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="HealthStatus"/> to <see cref="short"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator short(HealthStatus status) => status.value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="HealthStatus"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(HealthStatus status) => status.value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="HealthStatus"/> to <see cref="long"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator long(HealthStatus status) => status.value;

        /// <summary>
        /// Implements the operator greater than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if left operand is greater than right operand, false otherwise.</returns>
        public static bool operator >(HealthStatus left, HealthStatus right) => left.value > right.value;

        /// <summary>
        /// Implements the operator less than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if left operand is lower than right operand, false otherwise.</returns>
        public static bool operator <(HealthStatus left, HealthStatus right) => left.value < right.value;

        /// <summary>
        /// Implements the operator greater or equals than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if left operand is greater or equals than right operand, false otherwise.</returns>
        public static bool operator >=(HealthStatus left, HealthStatus right) => left.value >= right.value;

        /// <summary>
        /// Implements the operator less or equals than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if left operand is lower or equals than right operand, false otherwise.</returns>
        public static bool operator <=(HealthStatus left, HealthStatus right) => left.value <= right.value;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if statuses are equal, false otherwise.</returns>
        public static bool operator ==(HealthStatus left, HealthStatus right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if statuses are different, false otherwise.</returns>
        public static bool operator !=(HealthStatus left, HealthStatus right) => !left.Equals(right);

        /// <inheritdoc />
        public bool Equals(HealthStatus other) => string.Equals(this.value, other.value);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is HealthStatus status && this.Equals(status);

        /// <inheritdoc />
        public override int GetHashCode() => this.value.GetHashCode();

        /// <inheritdoc />
        public int CompareTo(HealthStatus other)
        {
            return this.value.CompareTo(other.value);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            switch (this.value)
            {
                case 0:
                    return nameof(Healthy);
                case 1:
                    return nameof(Starting);
                case 2:
                    return nameof(Degraded);
                case byte.MaxValue:
                    return nameof(Unhealthy);
                default:
                    return string.Empty;
            }
        }
    }
}