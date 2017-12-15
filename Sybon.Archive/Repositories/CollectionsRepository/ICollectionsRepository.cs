using System.Threading.Tasks;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CollectionsRepository
{
    public interface ICollectionsRepository : IBaseEntityRepository<Collection>
    {
        Task<CollectionModelWithProblemsCount[]> GetRangeAsync(int offset, int limit);
        Task<bool> ExistsAsync(long id);
        Task RemoveRangeAsync(long[] collectionIds);
    }
}