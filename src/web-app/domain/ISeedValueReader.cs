using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeJar.Domain
{
    public interface ISeedValueReader
    {
        IEnumerable<int> ReadSeedValues(int count);
    }
}