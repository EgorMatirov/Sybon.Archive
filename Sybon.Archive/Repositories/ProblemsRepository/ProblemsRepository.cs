﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Sybon.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sybon.Archive.Repositories.ProblemsRepository
{
    [UsedImplicitly]
    public class ProblemsRepository : BaseEntityRepository<Problem, ArchiveContext>, IProblemsRepository
    {
        public ProblemsRepository(ArchiveContext context) : base(context)
        {
        }

        public async Task RemoveRangeAsync(long[] problemIds)
        {
            object[] dbEntities = await Context.Problems.Where(x => problemIds.Contains(x.Id)).ToArrayAsync();
            Context.RemoveRange(dbEntities);
        }

        public new Task<Problem> FindAsync(long key)
        {
            return Context.Problems
                .Include(x => x.CachedInternalProblem)
                .ThenInclude(x => x.Pretests)
                .SingleOrDefaultAsync(x => x.Id == key);
        }
    }
}