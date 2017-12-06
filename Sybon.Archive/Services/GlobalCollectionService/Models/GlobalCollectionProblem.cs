using Sybon.Archive.Services.ProblemsService.Models;

namespace Sybon.Archive.Services.GlobalCollectionService.Models
{
    public class GlobalCollectionProblem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string StatementUrl { get; set; }
        public int TestsCount { get; set; }
        public int PretestsCount { get; set; }
        public ResourceLimits ResourceLimits { get; set; }
        public string InternalProblemId { get; set; }
    }
}