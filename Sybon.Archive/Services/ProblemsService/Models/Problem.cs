namespace Sybon.Archive.Services.ProblemsService.Models
{
    public class Problem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Format { get; set; }
        public string StatementUrl { get; set; }
        public long CollectionId { get; set; }
        public int TestsCount { get; set; }
        public Test[] Pretests { get; set; }

        public string InputFileName { get; set; }
        public string OutputFileName { get; set; }

        public string InternalProblemId { get; set; }
        public ResourceLimits ResourceLimits { get; set; }
    }
}