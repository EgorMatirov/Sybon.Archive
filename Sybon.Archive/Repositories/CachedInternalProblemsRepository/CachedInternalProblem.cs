﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Archive.Repositories.CachedTestsRepository;
using Sybon.Common;

namespace Sybon.Archive.Repositories.CachedInternalProblemsRepository
{
    public class CachedInternalProblem : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        public string InternalProblemId { get; set; }
        public byte[] Revision { get; set; }
        
        public string Name { get; set; }
        public string StatementUrl { get; set; }
        
        public int TestsCount { get; set; }
        public ICollection<CachedTest> Pretests { get; set; }
        
        public long TimeLimitMillis { get; set; }
        public long MemoryLimitBytes { get; set; }
    }
}