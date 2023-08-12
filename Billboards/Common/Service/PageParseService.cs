using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Options;

namespace Playbill.Billboards.Common.Service;

public abstract class PageParseService : BaseBillboardService
{
    protected PageParseService(IOptions<PageParseOptions> options) : base(options) { }
}
