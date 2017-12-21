using System.Threading.Tasks;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CollectionsRepository
{
    public interface ICollectionsRepository : IBaseEntityRepository<Collection>
    {
        Task<bool> ExistsAsync(long id);
        Task RemoveRangeAsync(long[] collectionIds);
        Task<CollectionModelWithProblemsCount[]> GetAllWithProblemsCount();
    }
}