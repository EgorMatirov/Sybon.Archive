using System.Collections.Generic;
using Sybon.Archive.Services.ProblemsService.Models;

namespace Sybon.Archive.Services.CollectionsService.Models
{
    public class ProblemCollection
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProblemCount { get; set; }
        public ICollection<Problem> Problems { get; set; }
    }
}