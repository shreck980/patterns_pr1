using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace noslq_pr.FakeDataGenerators
{
    public interface IFakeDataGenerator<T>
    {
        T GetFakeData();

    }
}
