using JetBrains.Annotations;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CachedTestsRepository
{
    [UsedImplicitly]
    public class CachedTestsRepository : BaseEntityRepository<CachedTest, ArchiveContext>, ICachedTestsRepository
    {
        public CachedTestsRepository(ArchiveContext context) : base(context)
        {
        }
    }
}