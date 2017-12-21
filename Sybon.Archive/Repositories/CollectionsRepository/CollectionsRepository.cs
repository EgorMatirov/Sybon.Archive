using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CollectionsRepository
{
    [UsedImplicitly]
    public class CollectionsRepository : BaseEntityRepository<Collection, ArchiveContext>, ICollectionsRepository
    {
        public CollectionsRepository(ArchiveContext context) : base(context)
        {
        }

        public new Task<Collection> FindAsync(long key)
        {
            return Context.Collections
                .Include(x => x.Problems)
                .SingleOrDefaultAsync(collection => collection.Id == key);
        }

        public Task<bool> ExistsAsync(long id)
        {
            return Context.Collections.AnyAsync(x => x.Id == id);
        }

        public async Task RemoveRangeAsync(long[] collectionIds)
        {
            object[] dbEntities = await Context.Collections.Where(x => collectionIds.Contains(x.Id)).ToArrayAsync();
            Context.RemoveRange(dbEntities);
        }

        public Task<CollectionModelWithProblemsCount[]> GetAllWithProblemsCount()
        {
            return Context.Collections
                .Select(x => new CollectionModelWithProblemsCount
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.Name,
                    ProblemsCount = x.Problems.Count
                })
                .ToArrayAsync();
        }
    }
}