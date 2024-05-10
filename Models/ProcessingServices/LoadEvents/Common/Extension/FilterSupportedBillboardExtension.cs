using Models.Billboards.Common.Enums;
using Models.Billboards.Common.Interfaces;

namespace Models.ProcessingServices.LoadEvents.Common.Extension;

public static class FilterSupportedBillboardExtension
{
    public static IEnumerable<IBillboardService> FilterSupportedBillboards(this IEnumerable<IBillboardService> allBillboardService,
        HashSet<BillboardTypes>? supportedTypes) => supportedTypes is null
            ? allBillboardService
            : allBillboardService.Where(service => supportedTypes.Contains(service.BillboardType));

}
