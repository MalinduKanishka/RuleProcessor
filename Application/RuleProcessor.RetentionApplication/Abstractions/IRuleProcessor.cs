using RuleProcessor.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessor.Application.Abstractions
{
    public interface IRuleProcessor<T>
    {
        Task<IList<T>> ProcessRule(int numberOfRetentions);
    }
}
