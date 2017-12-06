using System.Threading.Tasks;
using Sybon.Archive.Services.ProblemsService.Models;

namespace Sybon.Archive.Services.ProblemsService
{
    public interface IProblemsService
    {
        Task<string> GetStatementUrlAsync(long id);
        Task<Problem> GetAsync(long id);
        Task<long> AddAsync(long collectionId, long globalProblemId);
        Task RemoveRangeAsync(long[] problemIds);
    }
}