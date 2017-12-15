using System.Threading.Tasks;
using Sybon.Common;

namespace Sybon.Archive.Repositories.ProblemsRepository
{
    public interface IProblemsRepository : IBaseEntityRepository<Problem>
    {
        Task RemoveRangeAsync(long[] problemIds);
    }
}