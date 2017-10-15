using System.Threading.Tasks;
using Sybon.Auth;

namespace Sybon.Archive.Repositories.CollectionsRepository
{
    public interface ICollectionsRepository : IBaseEntityRepository<Collection>
    {
        Task<Collection[]> GetRangeAsync(int offset, int limit);
        Task<bool> ExistsAsync(long id);
    }
}