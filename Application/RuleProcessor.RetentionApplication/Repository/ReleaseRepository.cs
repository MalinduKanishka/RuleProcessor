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
    public class ReleaseRepository : IDevopsRepository<Release>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReleaseRepository> _logger;
        private List<Release>? _release;

        public ReleaseRepository(IConfiguration configuration, ILogger<ReleaseRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void LoadData()
        {
            string releaseDataPath = Directory.GetCurrentDirectory() + _configuration.GetValue<string>("ReleaseFile");

            if (File.Exists(releaseDataPath))
            {
                using (StreamReader r = new StreamReader(releaseDataPath))
                {
                    string json = r.ReadToEnd();
                    _release = JsonConvert.DeserializeObject<List<Release>>(json)!;
                }
            }
            else 
            {
                _logger.LogInformation("ReleaseFile does not exist");
            }
        }

        public Release FindById(string id)
        {
            return _release!.SingleOrDefault(x => x.Id == id)!;
        }

        public IList<Release> FindAll()
        {
            return _release!;
        }
    }
}
