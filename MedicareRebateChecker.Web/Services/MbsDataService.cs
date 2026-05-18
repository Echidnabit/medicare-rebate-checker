using System.Net.Http.Json;
using MedicareRebateChecker.Web.Models;

namespace MedicareRebateChecker.Web.Services;

public sealed class MbsDataService(HttpClient httpClient)
{
    private IReadOnlyList<MbsItem>? items;
    private MbsMetadata? metadata;

    public async Task<IReadOnlyList<MbsItem>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        items ??= await httpClient.GetFromJsonAsync<List<MbsItem>>("data/mbs-items.json", cancellationToken)
            ?? [];

        return items;
    }

    public async Task<MbsMetadata?> GetMetadataAsync(CancellationToken cancellationToken = default)
    {
        metadata ??= await httpClient.GetFromJsonAsync<MbsMetadata>("data/mbs-metadata.json", cancellationToken);
        return metadata;
    }
}
