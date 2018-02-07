using System.Threading.Tasks;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CacheRevisionRepository
{
    
    public interface ICacheRevisionRepository : IBaseEntityRepository<CacheRevision>
    {
        Task<CacheRevision> ReadAsync();
        Task<CacheRevision> FindAsync();
    }
}