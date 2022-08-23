using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RuleProcessor.Application.Abstractions;
using RuleProcessor.Application.Models;

namespace RuleProcessor.Application.Repository
{
    public class EnvironmentRepository : IDevopsRepository<Models.Environment>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EnvironmentRepository> _logger;
        private List<Models.Environment>? _environment;
        public EnvironmentRepository(IConfiguration configuration, ILogger<EnvironmentRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void LoadData()
        {  
            string EnviornmentDataPath = Directory.GetCurrentDirectory() + _configuration.GetValue<string>("EnvironmentFile");

            if (File.Exists(EnviornmentDataPath))
            {
                using (StreamReader r = new StreamReader(EnviornmentDataPath))
                {
                    string json = r.ReadToEnd();
                    _environment = JsonConvert.DeserializeObject<List<Models.Environment>>(json)!;
                }
            }
            else
            {
                _logger.LogInformation("EnvironmentFile does not exist");
            }
        }

        public Models.Environment FindById(string id)
        {
            return _environment!.SingleOrDefault(x => x.Id == id)!;
        }

        public IList<Models.Environment> FindAll()
        {
            return _environment!;
        }
    }
}
