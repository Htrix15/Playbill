using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Options;

namespace Playbill.Billboards.Common.Service;

public abstract class ApiService<T> : BaseBillboardService
{
    protected ApiService(IOptions<ApiOptions<T>> options) : base(options) { }
}
