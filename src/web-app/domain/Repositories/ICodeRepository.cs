using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CodeJar.Domain
{
    public interface ICodeRepository
    {
        Task AddCodesAsync(IEnumerable<Code> codes);
        Task<List<Code>> GetCodesAsync(int batchID, int pageNumber, string alphabet, int pageSize);
        Task UpdateCodesAsync(List<Code> codes);
        Task<List<Code>> GetCodesForActivationAsync(DateTime forDate, string alphabet);
    }
}