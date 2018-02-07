using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CachedInternalProblemsRepository
{
    [UsedImplicitly]
    public class CachedInternalProblemsRepository : BaseEntityRepository<CachedInternalProblem, ArchiveContext>,
        ICachedInternalProblemsRepository
    {
        public CachedInternalProblemsRepository(ArchiveContext context) : base(context)
        {
        }

        public Task<CachedInternalProblemRevision[]> ReadAllRevisionsAsync()
        {
            return Context.CachedInternalProblems.Select(x => new CachedInternalProblemRevision
            {
                CachedInternalProblemId = x.Id,
                InternalId = x.InternalProblemId,
                Revision = x.Revision
            }).ToArrayAsync();
        }

        public Task<CachedInternalProblem> ReadByInternalIdAsync(string internalId)
        {
            return Context.CachedInternalProblems.Include(x => x.Pretests)
                .SingleAsync(x => x.InternalProblemId == internalId);
        }

        public Task<CachedInternalProblem[]> FindByInternalIdsAsync(string[] internalIds)
        {
            return Context.CachedInternalProblems.Include(x => x.Pretests)
                .Where(x => internalIds.Contains(x.InternalProblemId))
                .ToArrayAsync();
        }
    }
}