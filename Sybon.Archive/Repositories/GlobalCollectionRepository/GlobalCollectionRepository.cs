using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sybon.Common;

namespace Sybon.Archive.Repositories.GlobalCollectionRepository
{
    public class GlobalCollectionRepository : BaseEntityRepository<GlobalCollectionProblem, ArchiveContext>, IGlobalCollectionRepository
    {
        public GlobalCollectionRepository(ArchiveContext context) : base(context)
        {
        }

        public Task<GlobalCollectionProblem[]> GetAllAsync()
        {
            return Context.GlobalCollectionProblems.ToArrayAsync();
        }

        public async Task RemoveRangeAsync(long[] problemIds)
        {
            object[] dbEntities = await Context.GlobalCollectionProblems.Where(x => problemIds.Contains(x.Id)).ToArrayAsync();
            Context.RemoveRange(dbEntities);
        }
    }
}