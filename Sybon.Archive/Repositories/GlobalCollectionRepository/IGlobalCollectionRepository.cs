using System.Threading.Tasks;
using Sybon.Auth;

namespace Sybon.Archive.Repositories.GlobalCollectionRepository
{
    public interface IGlobalCollectionRepository : IBaseEntityRepository<GlobalCollectionProblem>
    {
        Task<GlobalCollectionProblem[]> GetAllAsync();
        Task RemoveRangeAsync(long[] problemIds);
    }
}