using Playbill.Billboards.Common.Interfaces;
using Playbill.Common;

namespace Playbill.Services.LoadEvents.Common.Extension;

public static class FilterSupportedBillboardExtension
{
    public static IEnumerable<IBillboardService> FilterSupportedBillboards(this IEnumerable<IBillboardService> allBillboardService,
        HashSet<BillboardTypes>? supportedTypes) => supportedTypes is null
            ? allBillboardService
            : allBillboardService.Where(service => supportedTypes.Contains(service.BillboardType));

}
