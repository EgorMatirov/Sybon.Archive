﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Sybon.Archive;
using System;

namespace Sybon.Archive.Migrations
{
    [DbContext(typeof(ArchiveContext))]
    partial class ArchiveContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sybon.Archive.Repositories.CachedInternalProblemsRepository.CachedInternalProblem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Format");

                    b.Property<string>("InputFileName");

                    b.Property<string>("InternalProblemId")
                        .IsRequired();

                    b.Property<long>("MemoryLimitBytes");

                    b.Property<string>("Name");

                    b.Property<string>("OutputFileName");

                    b.Property<byte[]>("Revision");

                    b.Property<string>("StatementUrl");

                    b.Property<int>("TestsCount");

                    b.Property<long>("TimeLimitMillis");

                    b.HasKey("Id");

                    b.ToTable("CachedInternalProblems");
                });

            modelBuilder.Entity("Sybon.Archive.Repositories.CachedTestsRepository.CachedTest", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Input");

                    b.Property<string>("InternalId");

                    b.Property<string>("Output");

                    b.Property<long>("ProblemId");

                    b.HasKey("Id");

                    b.HasIndex("ProblemId");

                    b.ToTable("CachedTests");
                });

            modelBuilder.Entity("Sybon.Archive.Repositories.CacheRevisionRepository.CacheRevision", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Revision");

                    b.HasKey("Id");

                    b.ToTable("CacheRevisions");
                });

            modelBuilder.Entity("Sybon.Archive.Repositories.CollectionsRepository.Collection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("Sybon.Archive.Repositories.ProblemsRepository.Problem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("CollectionId");

                    b.Property<string>("InternalProblemId")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.HasIndex("InternalProblemId");

                    b.ToTable("Problems");
                });

            modelBuilder.Entity("Sybon.Archive.Repositories.CachedTestsRepository.CachedTest", b =>
                {
                    b.HasOne("Sybon.Archive.Repositories.CachedInternalProblemsRepository.CachedInternalProblem", "Problem")
                        .WithMany("Pretests")
                        .HasForeignKey("ProblemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sybon.Archive.Repositories.ProblemsRepository.Problem", b =>
                {
                    b.HasOne("Sybon.Archive.Repositories.CollectionsRepository.Collection", "Collection")
                        .WithMany("Problems")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sybon.Archive.Repositories.CachedInternalProblemsRepository.CachedInternalProblem", "CachedInternalProblem")
                        .WithMany("Problems")
                        .HasForeignKey("InternalProblemId")
                        .HasPrincipalKey("InternalProblemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
