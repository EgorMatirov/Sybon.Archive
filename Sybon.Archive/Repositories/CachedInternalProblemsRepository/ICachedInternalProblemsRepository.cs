using System.Collections.Generic;
using System.Threading.Tasks;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CachedInternalProblemsRepository
{
    public class CachedInternalProblemRevision
    {
        public long CachedInternalProblemId { get; set; }
        public string InternalId { get; set; }
        public byte[] Revision { get; set; }
    }
    
    public interface ICachedInternalProblemsRepository : IBaseEntityRepository<CachedInternalProblem>
    {
        Task<CachedInternalProblemRevision[]> ReadAllRevisionsAsync();
        Task<CachedInternalProblem> ReadByInternalIdAsync(string internalId);
        Task<CachedInternalProblem[]> FindByInternalIdsAsync(string[] internalIds);
    }
}