using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RuleProcessor.Application.Abstractions;
using RuleProcessor.Application.Models;
using System.Linq;

namespace RuleProcessor.RetentionApplication
{
    public class RetentionRuleProcessor : IRuleProcessor<ReleaseItem>, IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RetentionRuleProcessor> _logger;
        private readonly IDevopsRepository<Project> _projectRepository;
        private readonly IDevopsRepository<Application.Models.Environment> _enviornmentRepository;
        private readonly IDevopsRepository<Release> _releaseRepository;
        private readonly IDevopsRepository<Deployment> _deploymentRepository;
        private bool isDisposed;
        private Timer? _timer;


        public RetentionRuleProcessor(IDevopsRepository<Project> projectRepository, IDevopsRepository<Application.Models.Environment> enviornmentRepository, 
                                      IDevopsRepository<Release> releaseRepository, IDevopsRepository<Deployment> deploymentRepository,
                                      IConfiguration configuration, ILogger<RetentionRuleProcessor> logger)
        {
            _enviornmentRepository = enviornmentRepository;            
            _projectRepository = projectRepository;
            _releaseRepository = releaseRepository;
            _deploymentRepository = deploymentRepository;
            _configuration = configuration;
            _logger = logger;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
                _timer?.Dispose();

            _timer = null;
            isDisposed = true;
        }

        public async Task<IList<ReleaseItem>> ProcessRule(int numberOfRetentions)
        {
           
            List<Deployment>? initialData = await setupInitialData();
            List<ReleaseItem>? finalReleases = await calculateFinalReleases(initialData, numberOfRetentions);

            _logger.LogInformation("******************************************************************");
            _logger.LogInformation("Number of Retentions Per Project and Environment - " + numberOfRetentions);

            foreach (var releaseItem in finalReleases!)
            {
                _logger.LogInformation("Project:- " + releaseItem.Release!.Project!.Id+ " Enviornment:- " + releaseItem!.Deployments!.Select(x=>x.EnvironmentId).FirstOrDefault() + "\n");
                _logger.LogInformation("Release " + releaseItem.Release.Id + " will be retained due to the latest deployment date " + releaseItem!.Deployments!.Max(x => x.DeployedAt) + "\n");                
            }
            _logger.LogInformation("******************************************************************");

            return finalReleases;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start the Retention Rule Processor");

            try
            {
                //_timer = new Timer(ProcessRule, null, TimeSpan.Zero, TimeSpan.FromSeconds(120));
                int numberOfRetentions = _configuration.GetValue<int>("NumberOfRetentions");
                await ProcessRule(numberOfRetentions);

                while (cancellationToken.IsCancellationRequested)
                    await Task.Delay(-1, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to Start the Retention Rule Processor");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop the Retention Rule Processor");

            try
            {
                _timer?.Change(Timeout.Infinite, 0);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to Stop the Retention Rule Processor");
                return Task.FromResult(false);
            }
        }

        private async Task<List<Deployment>?> setupInitialData()
        {
            List<Deployment>? initialData = new List<Deployment>();

            try
            {
                _deploymentRepository.LoadData();
                _releaseRepository.LoadData();
                _projectRepository.LoadData();
                _enviornmentRepository.LoadData();

                foreach (var deployment in _deploymentRepository.FindAll())
                {
                    deployment.Release = _releaseRepository.FindById(deployment!.ReleaseId!);
                    deployment.Environment = _enviornmentRepository.FindById(deployment!.EnvironmentId!);
                    deployment.Release.Project = _projectRepository.FindById(deployment!.Release!.ProjectId!);
                    initialData.Add(deployment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to Set up initial Data");
            }

            return await Task.FromResult(initialData);
        }

        private async Task<List<ReleaseItem>?> calculateFinalReleases(List<Deployment>? initialData, int numberOfRetentions)
        {
            List<ReleaseItem>? finalReleases = new List<ReleaseItem>();

            try
            {
                var validatedDeployments = initialData!.Where(x => x.Environment != null && x.Release != null && x.Release.Project != null).ToList();

                //First group by Project and Enviornment
                var deploymentsGroupedByProjectNEnv = validatedDeployments.GroupBy(x => new { projectId = x.Release!.Project!.Id, enviornmentId = x.Environment!.Id }).ToList();

                foreach (var eachGroup in deploymentsGroupedByProjectNEnv)
                {
                    List<ReleaseItem>? releasesPerProjectNEnv = new List<ReleaseItem>();
                    IEnumerable<IGrouping<string?, Deployment>> groupByReleases = eachGroup.GroupBy(x => x.Release!.Id);

                    //Group by Release
                    foreach (IGrouping<string?, Deployment> groupData in groupByReleases)
                    {
                        //Create Release Item beacuse we need releases with Deployment data in it. Not otherway around
                        //This is the whole reason I have create the ReleaseItem Model
                        ReleaseItem releaseItem = new ReleaseItem();
                        releaseItem.Release = groupData.Select(x => x.Release!).First();
                        releaseItem!.Deployments = new List<Deployment>();
                        foreach (Deployment data in groupData)
                        {
                            releaseItem!.Deployments!.Add(data);
                        }

                        releasesPerProjectNEnv.Add(releaseItem);
                    }

                    var retainedReleases = releasesPerProjectNEnv.OrderByDescending(x => x.Deployments!.Max(y => y.DeployedAt))
                                                       .Take(numberOfRetentions).ToList();
                    finalReleases.AddRange(retainedReleases);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to Calculate Final Releases");
            }            

            return await Task.FromResult(finalReleases);
        }
        
    }
}