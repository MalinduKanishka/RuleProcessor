using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RuleProcessor;
using RuleProcessor.RetentionApplication;
using Autofac.Extensions.DependencyInjection;


public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)            
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureHostConfiguration(config =>
            {
                config.AddJsonFile($"appsettings.json");
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<RetentionRuleProcessor>();
            });
}