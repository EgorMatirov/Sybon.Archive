using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Auth;

namespace Sybon.Archive.Repositories.GlobalCollectionRepository
{
    public class GlobalCollectionProblem : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string InternalProblemId { get; set; }
    }
}