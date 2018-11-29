// -----------------------------------------------------------------------
// <copyright file="TestHelpers.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Diagnostics.Health;

    internal static class TestHelpers
    {
        public static bool AreEqual(this HealthReport expected, HealthReport report)
        {
            if (expected.Status != report.Status || expected.TotalDuration != report.TotalDuration)
            {
                return false;
            }

            HashSet<string> expectedKeys = new HashSet<string>();
            foreach (string key in expected.Entries.Keys)
            {
                if (!report.Entries.TryGetValue(key, out HealthCheckEntry entry) || !expected.Entries[key].AreEqual(entry))
                {
                    return false;
                }

                expectedKeys.Add(key);
            }

            return report.Entries.Keys.All(x => expectedKeys.Contains(x));
        }

        public static bool AreEqual(this HealthCheckEntry expected, HealthCheckEntry entry) =>
            expected.Status == entry.Status &&
            expected.Message == entry.Message &&
            expected.Data.SequenceEqual(entry.Data) &&
            expected.Tags.SequenceEqual(entry.Tags) &&
            expected.Duration == entry.Duration;

        public static bool AreEqual(this HealthCheckResult expected, HealthCheckResult result) =>
            expected.Status == result.Status &&
            expected.Message == result.Message &&
            expected.Data.SequenceEqual(result.Data);
    }
}