using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TrackerRunner;

public static class Program
{
    private static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureServices)
            .Build();

        RunProgram(host.Services);
    }

    private static void RunProgram(IServiceProvider services)
    {
        using IServiceScope serviceScope = services.CreateScope();
        var application = services.GetRequiredService<Application>();
        application.Run();
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
            .AddEnvironmentVariables()
            .Build();

        var applicationConfig = config.Get<ApplicationConfiguration>();

        services.AddSingleton(applicationConfig);
        services.AddSingleton<Application>();
        services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();

        services.AddScoped<IAppointmentProvider, AppointmentProvider>();
        services.AddScoped<INotificationDispatcher, EmailNotificationService>();
        services.AddScoped<INotificationMessageBuilder, NotificationMessageBuilder>();
        services.AddScoped<IRetrieveAppointmentsRequestBuilder, RetrieveAppointmentsRequestBuilder>();
    }
}