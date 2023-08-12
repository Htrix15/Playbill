﻿using Microsoft.Extensions.Configuration;

namespace Playbill.Infrastructure.Configure;

public static class Configurations
{
    public static IConfigurationRoot Configure()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }
}