using Sybon.Archive.Repositories.CachedInternalProblemsRepository;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CachedTestsRepository
{
    public class CachedTest : IEntity
    {
        public long Id { get; set; }
        
        public string InternalId { get; set; }
        
        public long ProblemId { get; set; }
        public CachedInternalProblem Problem { get; set; }
        
        public string Input { get; set; }
        public string Output { get; set; }
    }
}