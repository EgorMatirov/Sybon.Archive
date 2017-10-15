namespace Sybon.Archive.Repositories.CollectionsRepository
{
    public class CollectionModelWithProblemsCount
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProblemsCount { get; set; }
    }
}