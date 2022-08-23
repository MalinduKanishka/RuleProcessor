using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessor.Application.Models
{
    public class Release
    {
        public string? Id { get; set; }
        public string? ProjectId { get; set; }
        public string? Version { get; set; }
        public DateTime Created { get; set; }
        public Project? Project { get; set; }
        public List<Deployment>? Deployments { get; set; }       
                
    }
}
