namespace Payvision.Diagnostics.Health
{
    /// <summary>
    /// Contains the health policies that will be executed within a health service.
    /// </summary>
    public interface IHealthPolicyCollection
    {
        /// <summary>
        /// Adds the specified health policy within the configuring health service.
        /// </summary>
        /// <param name="name">The name of the health check which must be unique within the collection.</param>
        /// <param name="healthPolicy">The health policy instance to be executed within the owning <see cref="IHealthService"/>.</param>
        /// <returns>A <see cref="IHealthPolicyConfiguration"/> to configure the health check.</returns>
        IHealthPolicyConfiguration Add(string name, IHealthPolicy healthPolicy);
    }
}
