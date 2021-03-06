using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CodeJar.Domain
{
    public interface ICodeRepository
    {
        Task AddCodesAsync(IEnumerable<Code> codes);
        Task<Code> GetCodeAsync(int value);
        Task<List<Code>> GetCodesAsync(Guid batchID, int pageNumber, int pageSize);
        Task UpdateCodesAsync(List<Code> codes);
        Task UpdateCodeAsync(Code code);
        Task<RedeemCode> GetCodeForRedemptionAsync(int value);
        Task<DeactivateCode> GetCodeForDeactivationAsync(int value);
        IAsyncEnumerable<ActivateCode> GetCodesForActivationAsync(DateTime date);
        IAsyncEnumerable<ExpireCode> GetCodesForExpirationAsync(DateTime date);
    }
}