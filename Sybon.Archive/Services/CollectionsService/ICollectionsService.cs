using System.Threading.Tasks;
using Sybon.Archive.Services.CollectionsService.Models;

namespace Sybon.Archive.Services.CollectionsService
{
    public interface ICollectionsService
    {
        Task<long> AddAsync(long userId, CollectionForm collection);
        Task<ProblemCollectionWithProblems> FindAsync(long id);
        Task<ProblemCollectionWithoutProblems[]> GetRangeAsync(int offset, int limit);
        Task<bool> ExistsAsync(long id);
    }
}