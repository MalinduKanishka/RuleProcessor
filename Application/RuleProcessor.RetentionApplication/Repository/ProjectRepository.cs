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
    public class ProjectRepository : IDevopsRepository<Project>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProjectRepository> _logger;
        private List<Project>? _projects;
        public ProjectRepository(IConfiguration configuration, ILogger<ProjectRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void LoadData()
        {
            string projectDataPath = Directory.GetCurrentDirectory() + _configuration.GetValue<string>("ProjectFile");

            if (File.Exists(projectDataPath))
            {
                using (StreamReader r = new StreamReader(projectDataPath))
                {
                    string json = r.ReadToEnd();
                    _projects = JsonConvert.DeserializeObject<List<Project>>(json)!;
                }
            }
            else
            {
                _logger.LogInformation("ProjectFile does not exist");
            }
        }

        public Project FindById(string id)
        {
            return _projects!.SingleOrDefault(x => x.Id == id)!;
        }

        public IList<Project> FindAll()
        {
            return _projects!;
        }
    }
}
