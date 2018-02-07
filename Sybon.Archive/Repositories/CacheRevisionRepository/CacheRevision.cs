using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CacheRevisionRepository
{
    public class CacheRevision : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Revision { get; set; }
    }
}