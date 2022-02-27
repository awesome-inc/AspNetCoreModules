using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SampleApp.Api.Health;

internal static class HealthReportExtensions
{
    public static object PrettyJson(this HealthReport report)
    {
        return new
        {
            status = report.Status.ToString(),
            components = report.Entries.Select(PrettyJson)
        };
    }

    public static object PrettyJson(this KeyValuePair<string, HealthReportEntry> kvp)
    {
        var (name, entry) = kvp;
        return new
        {
            name,
            status = entry.Status.ToString(),
            data = entry.Data,
            // ---
            duration = entry.Duration,
            error = entry.Exception,
            tags = entry.Tags
        };
    }
}
