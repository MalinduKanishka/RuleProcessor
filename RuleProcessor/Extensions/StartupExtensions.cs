using RuleProcessor.Application.Abstractions;
using RuleProcessor.Application.Models;
using RuleProcessor.Application.Repository;
using RuleProcessor.RetentionApplication;
using Environment = RuleProcessor.Application.Models.Environment;

namespace RuleProcessor.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IDevopsRepository<Project>, ProjectRepository>();
            services.AddTransient<IDevopsRepository<Environment>, EnvironmentRepository>();
            services.AddTransient<IDevopsRepository<Release>, ReleaseRepository>();
            services.AddTransient<IDevopsRepository<Deployment>, DeploymentRepository>();
            services.AddTransient<IRuleProcessor<ReleaseItem>, RetentionRuleProcessor>();
            return services;
        }

        public static IServiceCollection AddWorkerProcess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHostedService, RetentionRuleProcessor>(context =>
            {
                return new RetentionRuleProcessor(
                    new ProjectRepository(configuration, context.GetRequiredService<ILogger<ProjectRepository>>()),
                    new EnvironmentRepository(configuration, context.GetRequiredService<ILogger<EnvironmentRepository>>()),
                    new ReleaseRepository(configuration, context.GetRequiredService<ILogger<ReleaseRepository>>()),
                    new DeploymentRepository(configuration, context.GetRequiredService<ILogger<DeploymentRepository>>()),
                    configuration,
                    context.GetRequiredService<ILogger<RetentionRuleProcessor>>());
            });
            return services;
        }
    }
}
