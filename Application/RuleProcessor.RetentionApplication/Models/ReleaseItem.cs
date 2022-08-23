using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessor.Application.Models
{
    public class ReleaseItem
    {     
        public Release? Release { get; set; }        
        public List<Deployment>? Deployments { get; set; }
    }
}
