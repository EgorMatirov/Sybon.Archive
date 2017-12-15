using System;
using Microsoft.AspNetCore.Hosting;
using Sybon.Common;

namespace Sybon.Archive
{
    public class ArchiveService : BaseService
    {
        public ArchiveService(Func<string[], IWebHost> buildWebHostFunc) : base(buildWebHostFunc)
        {
        }

        public override string ServiceName => "Sybon.Archive";
    }
}