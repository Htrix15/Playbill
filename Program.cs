using Microsoft.Extensions.DependencyInjection;
using Playbill;
using Playbill.ConfigureInfrastructure;


var services = ConfigureServices.Configure();
var configuration = ConfigureConfigurations.Configure();
ConfigureOptions.Configure(services, configuration);

using var serviceProvider = services.BuildServiceProvider();


