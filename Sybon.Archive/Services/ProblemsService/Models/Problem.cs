namespace Sybon.Archive.Services.ProblemsService.Models
{
    public class Problem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string StatementUrl { get; set; }
        public long CollectionId { get; set; }
        public int TestsCount { get; set; }
        public int PretestsCount { get; set; }
        public string InternalProblemId { get; set; }
        public ResourceLimits ResourceLimits { get; set; }
    }
}