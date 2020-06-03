using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CodeJar.Domain
{
    public interface ICodeRepository
    {
        Task AddCodesAsync(Batch batch);
    }
}