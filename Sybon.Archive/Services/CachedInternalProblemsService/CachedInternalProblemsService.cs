using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using AutoMapper;
using Bacs.Archive.Client.CSharp;
using Bacs.Archive.TestFetcher;
using Bacs.Problem.Single;
using Bacs.StatementProvider;
using Google.Protobuf;
using Google.Protobuf.Collections;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using Sybon.Archive.Repositories.CachedInternalProblemsRepository;
using Sybon.Archive.Repositories.CachedTestsRepository;
using Sybon.Archive.Repositories.CacheRevisionRepository;
using Sybon.Archive.Services.ProblemsService.Models;
using Sybon.Common;
using Problem = Bacs.Problem.Problem;
using Task = System.Threading.Tasks.Task;
using Test = Sybon.Archive.Services.ProblemsService.Models.Test;

namespace Sybon.Archive.Services.CachedInternalProblemsService
{
    [UsedImplicitly]
    public class CachedInternalProblemsService : ICachedInternalProblemsService
    {
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;
        private readonly IMapper _mapper;
        private readonly IArchiveClient _archiveClient;
        private readonly ITestsFetcher _testsFetcher;
        private readonly StatementProvider _statementProvider;

        public CachedInternalProblemsService(
            IRepositoryUnitOfWork repositoryUnitOfWork,
            IMapper mapper,
            IArchiveClient archiveClient,
            ITestsFetcher testsFetcher,
            StatementProvider statementProvider)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _mapper = mapper;
            _archiveClient = archiveClient;
            _testsFetcher = testsFetcher;
            _statementProvider = statementProvider;
        }

        public async Task<CachedInternalProblemRevision[]> GetAllRevisionsAsync()
        {
            var dbEntries = await _repositoryUnitOfWork.GetRepository<ICachedInternalProblemsRepository>()
                .ReadAllRevisionsAsync();
            return _mapper.Map<CachedInternalProblemRevision[]>(dbEntries);
        }

        public async Task<string> ReadCacheRevisionAsync()
        {
            var cacheRevision = await _repositoryUnitOfWork.GetRepository<ICacheRevisionRepository>().FindAsync();
            return cacheRevision == null ? string.Empty : cacheRevision.Revision;
        }

        public async Task UpdateCacheRevisionAsync(string cacheRevision)
        {
            var entity = await _repositoryUnitOfWork.GetRepository<ICacheRevisionRepository>().FindAsync();
            if (entity == null)
            {
                entity = new CacheRevision();
                await _repositoryUnitOfWork.GetRepository<ICacheRevisionRepository>().AddAsync(entity);
            }

            entity.Revision = cacheRevision;
            await _repositoryUnitOfWork.SaveChangesAsync();
        }

        public Task AddAsync(string internalId)
        {
            return Update(internalId, true);
        }

        public Task UpdateAsync(string internalId)
        {
            return Update(internalId, false);
        }

        public async Task<string> ExtractStatementUrlAsync(string internalProblemId)
        {
            var cachedProblem = await _repositoryUnitOfWork.GetRepository<ICachedInternalProblemsRepository>()
                .ReadByInternalIdAsync(internalProblemId);
            return cachedProblem.StatementUrl;
        }

        public async Task<bool> ExistsAsync(params string[] internalProblemIds)
        {
            var existingProblems = await _repositoryUnitOfWork.GetRepository<ICachedInternalProblemsRepository>()
                .FindByInternalIdsAsync(internalProblemIds);
            return existingProblems
                .Select(x => x.InternalProblemId)
                .OrderBy(x => x)
                .SequenceEqual(internalProblemIds.OrderBy(x => x));
        }

        public async Task<ProblemsService.Models.Problem> FetchProblemInfoAsync(ProblemsService.Models.Problem problem)
        {
            var cachedProblem = await _repositoryUnitOfWork.GetRepository<ICachedInternalProblemsRepository>()
                .ReadByInternalIdAsync(problem.InternalProblemId);

            problem.Name = cachedProblem.Name;
            problem.Pretests = _mapper.Map<Test[]>(cachedProblem.Pretests);
            problem.ResourceLimits = new ResourceLimits
            {
                MemoryLimitBytes = cachedProblem.MemoryLimitBytes,
                TimeLimitMillis = cachedProblem.TimeLimitMillis
            };
            problem.StatementUrl = cachedProblem.StatementUrl;
            problem.TestsCount = cachedProblem.TestsCount;
            return problem;
        }

        private async Task Update(string internalId, bool needCreation)
        {
            var importResult = await _archiveClient.ImportResultAsync(internalId);

            var internalProblem = importResult.SingleOrDefault(x => x.Key == internalId).Value.Problem;

            if (internalProblem == null)
                throw new ArgumentException($"Problem with internal id '{internalId}' was not found.");

            var testGroups = ExtractTestGroups(internalProblem);

            CachedInternalProblem cachedProblem;
            if (needCreation)
            {
                cachedProblem = new CachedInternalProblem {InternalProblemId = internalId};
                await _repositoryUnitOfWork.GetRepository<ICachedInternalProblemsRepository>().AddAsync(cachedProblem);
            }
            else
            {
                cachedProblem = await _repositoryUnitOfWork.GetRepository<ICachedInternalProblemsRepository>()
                    .ReadByInternalIdAsync(internalId);
            }

            cachedProblem.Name = internalProblem.Info.Name.FirstOrDefault()?.Value;
            cachedProblem.Author = string.Join(",", internalProblem.Info.Author.Concat(internalProblem.Info.Maintainer));
            cachedProblem.Format = internalProblem.Statement.Version.FirstOrDefault()?.Format;
            cachedProblem.Revision = internalProblem.System?.Revision?.ToByteArray();
            cachedProblem.StatementUrl = ExtractStatementUrl(internalProblem);
            cachedProblem.TestsCount = testGroups.Sum(x => x.Tests.Query.Count);
            cachedProblem.MemoryLimitBytes = (long) testGroups.First().Process.ResourceLimits.MemoryLimitBytes;
            cachedProblem.TimeLimitMillis = (long) testGroups.First().Process.ResourceLimits.TimeLimitMillis;
            cachedProblem.InputFileName = ExtractFileName(testGroups, "stdin");
            cachedProblem.OutputFileName = ExtractFileName(testGroups, "stdout");

            await UpdatePretests(internalId, cachedProblem, testGroups);
            await _repositoryUnitOfWork.SaveChangesAsync();
        }

        private static string ExtractFileName(IEnumerable<TestGroup> testGroups, string fileName)
        {
            var file = testGroups.First().Process.File.FirstOrDefault(x => x.Id.Equals(fileName));
            return file?.Path == null ? fileName.ToUpper() : file.Path.Element.Last();
        }

        private string ExtractStatementUrl([NotNull] Problem internalProblem)
        {
            var package = internalProblem.Statement.Version.First().Package;
            var hash = internalProblem.System.Revision.ToByteArray();

            return _statementProvider.GetStatementUrl(package, hash);
        }

        private Task UpdatePretests(
            string internalId,
            CachedInternalProblem cachedProblem,
            IEnumerable<TestGroup> testGroups)
        {
            var cachedTestsRepository = _repositoryUnitOfWork.GetRepository<ICachedTestsRepository>();

            if (cachedProblem.Pretests != null)
            {
                var removeTasks = cachedProblem.Pretests.Select(x => cachedTestsRepository.RemoveAsync(x.Id));
                Task.WaitAll(removeTasks.ToArray());
            }

            var pretests = ExtractPretests(internalId, cachedProblem, testGroups);
            var addTasks = pretests.Select(x => cachedTestsRepository.AddAsync(x));

            return Task.WhenAll(addTasks.ToArray());
        }

        private static RepeatedField<TestGroup> ExtractTestGroups(Problem internalProblem)
        {
            return ProfileExtension.Parser.ParseFrom(internalProblem.Profile.First().Extension.Value).TestGroup;
        }

        private IEnumerable<CachedTest> ExtractPretests(
            string internalId,
            CachedInternalProblem cachedProblem,
            IEnumerable<TestGroup> testGroups)
        {
            var pretestIds = testGroups.FirstOrDefault(x => x.Id == "pre")?.Tests?.Query?.Select(x => x.Id).ToArray();
            if (pretestIds == null)
            {
                return new CachedTest[0];
            }

            var archiveTests = _testsFetcher.FetchTests(_archiveClient, internalId, pretestIds).ToArray();
            return archiveTests.Select(x =>
                new CachedTest
                {
                    InternalId = x.Id,
                    Problem = cachedProblem,
                    Input = x.Input,
                    Output = x.Output
                });
        }
    }
}