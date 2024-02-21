﻿using System;
using OrderCloud.SDK;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Catalyst.Common;
using OrderCloud.Catalyst;
using Catalyst.Common.ProductUpload.Commands;
using Catalyst.Functions;
using Catalyst.Functions.Jobs.ForwardOrdersToThirdParty;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Catalyst.Functions
{
    public class Startup : FunctionsStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var azureConnectionString = Environment.GetEnvironmentVariable("appConfigConnectionString");
            var settings = BuildSettings(azureConnectionString);

            var orderCloudClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                AuthUrl = settings.OrderCloudSettings.ApiUrl,
                ClientId = settings.OrderCloudSettings.MiddlewareClientID,
                ClientSecret = settings.OrderCloudSettings.MiddlewareClientSecret,
                Roles = new[] { ApiRole.FullAccess }
            });
            builder.Services.AddSingleton(settings);
            builder.Services.AddSingleton<IOrderCloudClient>(orderCloudClient);
            builder.Services.AddSingleton<IProductCommand, ProductCommand>();
            builder.Services.AddSingleton<ForwardOrderJob>();
        }

        private AppSettings BuildSettings(string azureConnectionString)
		{
            var settings = new AppSettings();

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddAzureAppConfiguration(azureConnectionString)
                .Build();

            config.Bind(settings);
            return settings;
        }
    }           
}
