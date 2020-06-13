using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CodeJar.Domain
{
    public interface ICodeRepository
    {
        Task<CodeDto> GetAsync(int value);
        Task<List<CodeDto>> GetAsync(Guid batchID, int pageNumber, int pageSize);
        Task UpdateAsync(List<Code> codes);
        Task UpdateAsync(Code code);
        Task AddAsync(IEnumerable<GeneratedCode> codes);
        Task<RedeemingCode> GetRedeemingAsync(int value);
        Task<DeactivatingCode> GetDeactivatingAsync(int value);
        IAsyncEnumerable<ActivatingCode> GetActivatingAsync(DateTime date);
        IAsyncEnumerable<ExpiringCode> GetExpiringAsync(DateTime date);
    }
}