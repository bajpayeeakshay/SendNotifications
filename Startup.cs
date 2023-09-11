using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SendNotification.Services.Models;
using SendNotification.Services.Services;
using Serilog;

[assembly: FunctionsStartup(typeof(SendNotifications.Functions.Startup))]
namespace SendNotifications.Functions;

public class Startup : FunctionsStartup
{

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Configure Serilog
        var logger = new Serilog.LoggerConfiguration()
            .WriteTo.File("Logs/SendNotificationFunction.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Serilog.Log.Logger = logger;
        builder.Services.AddSingleton<ILogger>(logger);

        // Add and configure your services here
        builder.Services.Configure<FunctionConfiguration>(builder.GetContext().Configuration.GetSection("Values"));
        builder.Services.AddTransient<INotificationProcessingService, NotificationProcessingService>();
        builder.Services.AddTransient<IUserDataService, UserDataService>();


        // Configure Service Bus connection
        builder.Services.AddSingleton(x =>
        {
            var configuration = x.GetRequiredService<IOptions<FunctionConfiguration>>().Value;
            return new ServiceBusClient(configuration.ServiceBusConnectionString);
        });
        builder.Services.AddSingleton(x =>
        {
            var client = x.GetRequiredService<ServiceBusClient>();
            var configuration = x.GetRequiredService<IOptions<FunctionConfiguration>>().Value;
            return client.CreateSender(configuration.ServiceBusQueueName);
        });
        builder.Services.AddTransient<IServiceBusMessageService, ServiceBusMessageService>();
    }
}
