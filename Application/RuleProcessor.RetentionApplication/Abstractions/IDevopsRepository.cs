using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleProcessor.Application.Abstractions
{
    public interface IDevopsRepository<T>
    {
        void LoadData();

        T FindById(string id);

        IList<T> FindAll();
    }
}
