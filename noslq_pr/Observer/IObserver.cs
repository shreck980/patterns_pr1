using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.Observer
{
    public interface IObserver
    {
        void Update(string operation, object criteria, object result);
    }
}
