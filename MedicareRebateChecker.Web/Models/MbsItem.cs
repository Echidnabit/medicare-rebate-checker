namespace MedicareRebateChecker.Web.Models;

public sealed record MbsItem
{
    public string ItemNumber { get; init; } = "";
    public string? SubItemNumber { get; init; }
    public string Category { get; init; } = "";
    public string CategoryName { get; init; } = "";
    public string Group { get; init; } = "";
    public string? SubGroup { get; init; }
    public string? SubHeading { get; init; }
    public string FeeType { get; init; } = "";
    public string BenefitType { get; init; } = "";
    public decimal? ScheduleFee { get; init; }
    public decimal? Benefit75 { get; init; }
    public decimal? Benefit85 { get; init; }
    public decimal? Benefit100 { get; init; }
    public string? DerivedFee { get; init; }
    public string Description { get; init; } = "";
    public string? ItemStartDate { get; init; }
    public string? ItemEndDate { get; init; }
    public string? FeeStartDate { get; init; }
    public string? DescriptionStartDate { get; init; }
    public bool IsPathology { get; init; }

    public string GroupLabel => string.IsNullOrWhiteSpace(SubGroup)
        ? Group
        : $"{Group}.{SubGroup}";
}
