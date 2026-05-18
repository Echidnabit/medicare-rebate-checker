using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

const string SourceUrl = "https://www.mbsonline.gov.au/internet/mbsonline/publishing.nsf/650f3eec0dfb990fca25692100069854/dd6984c45a944962ca258d8600139d55/$FILE/MBS-XML-20260301-version%202.XML";
const string SourceVersion = "MBS XML 2026-03-01 version 2";

var outputDirectory = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../MedicareRebateChecker.Web/wwwroot/data"));

Directory.CreateDirectory(outputDirectory);

using var httpClient = new HttpClient();
await using var xmlStream = await httpClient.GetStreamAsync(SourceUrl);
var document = await XDocument.LoadAsync(xmlStream, LoadOptions.None, CancellationToken.None);

var items = document.Root?
    .Elements("Data")
    .Select(ToMbsItem)
    .OrderBy(item => ParseItemNumber(item.ItemNumber))
    .ThenBy(item => item.ItemNumber, StringComparer.Ordinal)
    .ToList() ?? [];

var metadata = new MbsMetadata
{
    SourceUrl = SourceUrl,
    SourceVersion = SourceVersion,
    GeneratedAtUtc = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture),
    ItemCount = items.Count,
    PathologyItemCount = items.Count(item => item.IsPathology)
};

var jsonOptions = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

await File.WriteAllTextAsync(
    Path.Combine(outputDirectory, "mbs-items.json"),
    JsonSerializer.Serialize(items, jsonOptions));

await File.WriteAllTextAsync(
    Path.Combine(outputDirectory, "mbs-metadata.json"),
    JsonSerializer.Serialize(metadata, jsonOptions));

Console.WriteLine($"Wrote {items.Count:N0} MBS items to {outputDirectory}");
Console.WriteLine($"Pathology items: {metadata.PathologyItemCount:N0}");

static MbsItem ToMbsItem(XElement element)
{
    var category = Value(element, "Category");
    return new MbsItem
    {
        ItemNumber = Value(element, "ItemNum"),
        SubItemNumber = NullIfEmpty(Value(element, "SubItemNum")),
        Category = category,
        CategoryName = CategoryName(category),
        Group = Value(element, "Group"),
        SubGroup = NullIfEmpty(Value(element, "SubGroup")),
        SubHeading = NullIfEmpty(Value(element, "SubHeading")),
        FeeType = Value(element, "FeeType"),
        BenefitType = Value(element, "BenefitType"),
        ScheduleFee = DecimalValue(element, "ScheduleFee"),
        Benefit75 = DecimalValue(element, "Benefit75"),
        Benefit85 = DecimalValue(element, "Benefit85"),
        Benefit100 = DecimalValue(element, "Benefit100"),
        DerivedFee = NullIfEmpty(NormalizeText(Value(element, "DerivedFee"))),
        Description = NormalizeText(Value(element, "Description")),
        ItemStartDate = NullIfEmpty(Value(element, "ItemStartDate")),
        ItemEndDate = NullIfEmpty(Value(element, "ItemEndDate")),
        FeeStartDate = NullIfEmpty(Value(element, "FeeStartDate")),
        DescriptionStartDate = NullIfEmpty(Value(element, "DescriptionStartDate")),
        IsPathology = category == "6"
    };
}

static string Value(XElement element, string name)
{
    return element.Element(name)?.Value.Trim() ?? "";
}

static decimal? DecimalValue(XElement element, string name)
{
    var value = Value(element, name);
    return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result)
        ? result
        : null;
}

static int ParseItemNumber(string itemNumber)
{
    return int.TryParse(itemNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
        ? result
        : int.MaxValue;
}

static string? NullIfEmpty(string value)
{
    return string.IsNullOrWhiteSpace(value) ? null : value;
}

static string NormalizeText(string value)
{
    return string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}

static string CategoryName(string category)
{
    return category switch
    {
        "1" => "Professional attendances",
        "2" => "Diagnostic procedures and investigations",
        "3" => "Therapeutic procedures",
        "4" => "Oral and maxillofacial services",
        "5" => "Diagnostic imaging services",
        "6" => "Pathology services",
        "7" => "Cleft lip and cleft palate services",
        "8" => "Miscellaneous services",
        "10" => "Dental services",
        _ => string.IsNullOrWhiteSpace(category) ? "Uncategorised" : $"Category {category}"
    };
}

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
}

public sealed record MbsMetadata
{
    public string SourceUrl { get; init; } = "";
    public string GeneratedAtUtc { get; init; } = "";
    public string SourceVersion { get; init; } = "";
    public int ItemCount { get; init; }
    public int PathologyItemCount { get; init; }
}
