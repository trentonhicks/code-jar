using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CodeJar.Domain
{
    public interface ICodeRepository
    {
        Task AddCodesAsync(IEnumerable<Code> codes);
    }
}