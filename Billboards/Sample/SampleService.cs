using Microsoft.Extensions.Options;

namespace Playbill.Billboards.Sample;

public class SampleService
{
    private readonly SampleApiOptions _sampleApiOptions;
    private readonly SamplePageParseOptions _samplePageParseOptions;
    public SampleService(IOptions<SampleApiOptions> sampleApiOptions, IOptions<SamplePageParseOptions> samplePageParseOptions)
    {
        _sampleApiOptions = sampleApiOptions.Value;
        _samplePageParseOptions = samplePageParseOptions.Value;
    }

    public void Test()
    {

    }
}
