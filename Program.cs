using Microsoft.Extensions.DependencyInjection;
using Playbill.Infrastructure.Configure;

var services = Services.Configure();
var configuration = Configurations.Configure();
Options.Configure(services, configuration);

using var serviceProvider = services.BuildServiceProvider();


