using Microsoft.Extensions.DependencyInjection;
using Playbill.Billboards.Sample;
using Playbill.ConfigureInfrastructure;


var services = ConfigureServices.Configure();
var configuration = ConfigureConfigurations.Configure();
ConfigureOptions.Configure(services, configuration);

using var serviceProvider = services.BuildServiceProvider();
serviceProvider.GetService<SampleService>()?.Test();

