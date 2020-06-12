﻿using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SharesiesToSharesight.SharesiesClient;
using SharesiesToSharesight.SharesightClient;

namespace SharesiesToSharesight
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new Configuration.Configuration();
            new ConfigurationBuilder().AddYamlFile("config.yml").Build().Bind(configuration);
            Validator.ValidateObject(configuration, new ValidationContext(configuration), true);
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console(outputTemplate: "{Level:u4} {Timestamp:HH:mm:ss} -- {Message:lj}{NewLine}{Exception}").CreateLogger();
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(
                    loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddSerilog(dispose: true);
                    }
                )
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(configuration);
                    services.AddHttpClient();
                    services.AddTransient<ISharesiesClient, SharesiesClient.SharesiesClient>();
                    services.AddTransient<ISharesightClient, SharesightClient.SharesightClient>();
                    services.AddHostedService<Worker>();
                }); 
        }
    }
}
