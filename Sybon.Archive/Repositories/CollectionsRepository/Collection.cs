using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Archive.Repositories.ProblemsRepository;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CollectionsRepository
{
    public class Collection : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Problem> Problems { get; set; }
    }
}