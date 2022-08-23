using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RuleProcessor.Application.Abstractions;
using RuleProcessor.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessor.Application.Repository
{
    public class DeploymentRepository : IDevopsRepository<Deployment>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeploymentRepository> _logger;
        private List<Deployment>? _deployments;
        public DeploymentRepository(IConfiguration configuration, ILogger<DeploymentRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
        }

        public void LoadData()
        {
            string deploymentDataPath = Directory.GetCurrentDirectory() + _configuration.GetValue<string>("DeploymentFile");                      

            if (File.Exists(deploymentDataPath)) 
            {
                using (StreamReader r = new StreamReader(deploymentDataPath))
                {
                    string json = r.ReadToEnd();
                    _deployments = JsonConvert.DeserializeObject<List<Deployment>>(json)!;
                }
            }
            else
            {
                _logger.LogInformation("DeploymentFile does not exist");
            }
        }

        public Deployment FindById(string id)
        {
            return _deployments!.SingleOrDefault(x => x.Id == id)!;
        }

        public IList<Deployment> FindAll()
        {
            return _deployments!;
        }
    }
}
