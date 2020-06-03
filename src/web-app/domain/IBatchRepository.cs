using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeJar.Domain
{
    public interface IBatchRepository
    {
        Task<List<Batch>> GetBatchesAsync();
        Task CreateBatchAsync(Batch batch);
        Task<Batch> GetBatchAsync(int id);
    }
}