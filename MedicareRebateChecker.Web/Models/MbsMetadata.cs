namespace MedicareRebateChecker.Web.Models;

public sealed record MbsMetadata
{
    public string SourceUrl { get; init; } = "";
    public string GeneratedAtUtc { get; init; } = "";
    public string SourceVersion { get; init; } = "";
    public int ItemCount { get; init; }
    public int PathologyItemCount { get; init; }
}
