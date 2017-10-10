using JetBrains.Annotations;
using Sybon.Common;

namespace Sybon.Archive.Repositories.ProblemsRepository
{
    [UsedImplicitly]
    public class ProblemsRepository : BaseEntityRepository<Problem, ArchiveContext>, IProblemsRepository
    {
        public ProblemsRepository(ArchiveContext context) : base(context)
        {
        }
    }
}