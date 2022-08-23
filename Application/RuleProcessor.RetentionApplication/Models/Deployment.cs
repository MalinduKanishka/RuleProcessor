using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessor.Application.Models
{
    public class Deployment
    {
        public string? Id { get; set; }
        public string? ReleaseId { get; set; }
        public string? EnvironmentId { get; set; }
        public DateTime DeployedAt { get; set; }
        public Release? Release { get; set; }
        public Environment? Environment { get; set; }


    }
}
