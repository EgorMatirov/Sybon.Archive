using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CacheRevisionRepository
{
    [UsedImplicitly]
    public class CacheRevisionRepository : BaseEntityRepository<CacheRevision, ArchiveContext>, ICacheRevisionRepository
    {
        public CacheRevisionRepository(ArchiveContext context) : base(context)
        {
        }

        public Task<CacheRevision> ReadAsync()
        {
            return Context.CacheRevisions.SingleAsync();
        }

        public Task<CacheRevision> FindAsync()
        {
            return Context.CacheRevisions.SingleOrDefaultAsync();
        }
    }
}