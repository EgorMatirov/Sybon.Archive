using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Archive.Repositories.CollectionsRepository;
using Sybon.Archive.Services.GlobalCollectionService.Models;
using Sybon.Auth;

namespace Sybon.Archive.Repositories.ProblemsRepository
{
    public class Problem : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? CollectionId { get; set; }
        public Collection Collection { get; set; }
        public long GlobalProblemId { get; set; }
        public GlobalCollectionProblem GlobalProblem { get; set; }
    }
}