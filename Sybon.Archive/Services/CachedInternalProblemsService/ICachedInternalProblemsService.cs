using System.Threading.Tasks;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Sybon.Archive.Services.ProblemsService.Models;

namespace Sybon.Archive.Services.CachedInternalProblemsService
{
    public interface ICachedInternalProblemsService
    {
        Task<string> ReadCacheRevisionAsync();
        Task UpdateCacheRevisionAsync(string cacheRevision);
        Task AddAsync(string internalId);
        Task<CachedInternalProblemRevision[]> GetAllRevisionsAsync();
        Task UpdateAsync(string internalId);
        Task<string> ExtractStatementUrlAsync(string internalId);
        Task<bool> ExistsAsync(params string[] internalProblemIds);
        Task<Problem> FetchProblemInfoAsync(Problem problem);
    }
}