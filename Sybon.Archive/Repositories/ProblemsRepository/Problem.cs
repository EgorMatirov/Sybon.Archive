using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Archive.Repositories.CollectionsRepository;
using Sybon.Auth;

namespace Sybon.Archive.Repositories.ProblemsRepository
{
    public class Problem : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? CollectionId { get; set; }
        public Collection Collection { get; set; }
        [Required]
        [MaxLength(100)]
        public string InternalProblemId { get; set; }
    }
}