using JetBrains.Annotations;

namespace Sybon.Archive.Services.CollectionsService.Models
{
    [UsedImplicitly]
    public class ProblemCollectionWithoutProblems
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProblemsCount { get; set; }
    }
}