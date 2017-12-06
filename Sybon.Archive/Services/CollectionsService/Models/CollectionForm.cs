using System.Collections.Generic;
using JetBrains.Annotations;

namespace Sybon.Archive.Services.CollectionsService.Models
{
    public class CollectionForm
    {
        [UsedImplicitly]
        public string Name { get; set; }
        [UsedImplicitly]
        public string Description { get; set; }
        public ICollection<ProblemModel> Problems { get; set; }

        [UsedImplicitly]
        public class ProblemModel
        {
            public string GlobalProblemId { get; set; }
        }
    }
}