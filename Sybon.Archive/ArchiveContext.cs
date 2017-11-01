using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Archive.Repositories.CollectionsRepository;
using Sybon.Archive.Repositories.ProblemsRepository;

namespace Sybon.Archive
{
    public class ArchiveContext : DbContext
    {
        public ArchiveContext(DbContextOptions<ArchiveContext> options) : base(options)
        {
        }

        public DbSet<Problem> Problems { get; [UsedImplicitly] set; }
        public DbSet<Collection> Collections { get; [UsedImplicitly] set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Problem>()
                .HasIndex(x => x.InternalProblemId);
            modelBuilder.Entity<Collection>()
                .HasMany(x => x.Problems)
                .WithOne(x => x.Collection)
                .HasForeignKey(x => x.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}