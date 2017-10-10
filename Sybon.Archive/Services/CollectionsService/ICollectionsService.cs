using System.Threading.Tasks;
using Sybon.Archive.Services.CollectionsService.Models;

namespace Sybon.Archive.Services.CollectionsService
{
    public interface ICollectionsService
    {
        Task<long> AddAsync(long userId, ProblemCollection collection);
        Task<ProblemCollection> FindAsync(long id);
        Task<ProblemCollection[]> GetRangeAsync(int offset, int limit);
    }
}