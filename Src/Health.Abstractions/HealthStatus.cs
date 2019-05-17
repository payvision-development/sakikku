namespace Payvision.Diagnostics.Health
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the status of a health check result.
    /// </summary>
    public struct HealthStatus : IEquatable<HealthStatus>, IComparable<HealthStatus>
    {
        private static readonly Dictionary<byte, string> NameMap = new Dictionary<byte, string>();

        private readonly byte value;

        static HealthStatus()
        {
            Healthy = CreateAndMap(0, nameof(Healthy));
            Starting = CreateAndMap(1, nameof(Starting));
            Degraded = CreateAndMap(2, nameof(Degraded));
            Unhealthy = CreateAndMap(3, nameof(Unhealthy));
        }

        private HealthStatus(byte value) => this.value = value;

        /// <summary>
        /// Gets the value indicating that the health check determined that the component is healthy.
        /// </summary>
        public static HealthStatus Healthy { get; }

        /// <summary>
        /// Gets the value indicating that the application is either starting or in a warm-up state.
        /// </summary>
        public static HealthStatus Starting { get; }

        /// <summary>
        /// Gets the value that indicating that the application is in a degraded state.
        /// </summary>
        public static HealthStatus Degraded { get; }

        /// <summary>
        /// Gets the value indicating that the health check determined that the component is unhealthy, or
        /// an exception was thrown while executing such check.
        /// </summary>
        public static HealthStatus Unhealthy { get; }

        /// <inheritdoc />
        public int CompareTo(HealthStatus other) => this.value.CompareTo(other.value);

        /// <inheritdoc />
        public bool Equals(HealthStatus other) => this.value == other.value;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is HealthStatus other && this.Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => NameMap.TryGetValue(this.value, out string name) ? name : string.Empty;

        private static HealthStatus CreateAndMap(byte value, string name)
        {
            NameMap[value] = name;
            return new HealthStatus(value);
        }
    }
}
