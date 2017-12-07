using System.Threading.Tasks;
using Sybon.Archive.Services.GlobalCollectionService.Models;
using Problem = Sybon.Archive.Services.ProblemsService.Models.Problem;

namespace Sybon.Archive.Services.GlobalCollectionService
{
    public interface IGlobalCollectionService
    {
        Task<GlobalCollectionProblem[]> GetAllAsync();
        Task<long> AddAsync(string internalProblemId);
        Task RemoveRangeAsync(long[] problemIds);
        Task<GlobalCollectionProblem> GetProblemAsync(long problemId);
    }
}