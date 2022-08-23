using Autofac;
using RuleProcessor.Extensions;

namespace RuleProcessor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddInfrastructure()
                .AddWorkerProcess(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


        }


    }
}
