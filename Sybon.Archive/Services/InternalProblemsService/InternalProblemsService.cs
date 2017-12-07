using System;
using System.Linq;
using AutoMapper;
using Bacs.Archive.Client.CSharp;
using Bacs.Archive.TestFetcher;
using Bacs.Problem.Single;
using Bacs.StatementProvider;
using Google.Protobuf;
using JetBrains.Annotations;
using Sybon.Archive.Services.ProblemsService.Models;
using Test = Sybon.Archive.Services.ProblemsService.Models.Test;

namespace Sybon.Archive.Services.InternalProblemsService
{
    [UsedImplicitly]
    public class InternalProblemsService : IInternalProblemsService
    {
        private readonly IArchiveClient _archiveClient;
        private readonly StatementProvider _statementProvider;
        private readonly ITestsFetcher _testsFetcher;
        private readonly IMapper _mapper;

        public InternalProblemsService(IArchiveClient archiveClient, StatementProvider statementProvider, ITestsFetcher testsFetcher, IMapper mapper)
        {
            _archiveClient = archiveClient;
            _statementProvider = statementProvider;
            _testsFetcher = testsFetcher;
            _mapper = mapper;
        }

        public Problem FetchProblemInfo(Problem problem)
        {
            var info = _archiveClient.ImportResult(problem.InternalProblemId);
            var internalProblem = info.SingleOrDefault(x => x.Key == problem.InternalProblemId).Value.Problem;
            if (internalProblem == null) throw new ArgumentException($"Problem with internal id '{problem.InternalProblemId}' was not found. Problem id is '{problem.Id}'.");
            var statementUrl = ExtractStatementUrl(internalProblem);

            var testGroups = ProfileExtension.Parser.ParseFrom(internalProblem.Profile.First().Extension.Value).TestGroup;
            var testsCount = testGroups.Sum(x => x.Tests.Query.Count);

            var problemArchive = _archiveClient.Download(SevenZipArchive.ZipFormat, problem.InternalProblemId);
            var pretestIds = testGroups.FirstOrDefault(x => x.Id == "pre")?.Tests?.Query?.Select(x => x.Id)?.ToArray();
            var pretests = pretestIds == null ? null : _mapper.Map<Test[]>(_testsFetcher.FetchTests(problemArchive, problem.InternalProblemId, pretestIds).ToArray());

            
            var resourceLimits = testGroups.First().Process.ResourceLimits;

            return new Problem
            {
                Id = problem.Id,
                Name = internalProblem.Info.Name.First().Value,
                StatementUrl = statementUrl,
                CollectionId = problem.CollectionId,
                TestsCount = testsCount,
                Pretests = pretests,
                InternalProblemId = problem.InternalProblemId,
                ResourceLimits = new ResourceLimits
                {
                    MemoryLimitBytes = (long) resourceLimits.MemoryLimitBytes,
                    TimeLimitMillis = (long) resourceLimits.TimeLimitMillis
                }
            };
        }
        
        public string ExtractStatementUrl(Problem problem)
        {
            var info = _archiveClient.ImportResult(problem.InternalProblemId);
            var internalProblem = info.SingleOrDefault(x => x.Key == problem.InternalProblemId).Value.Problem;
            if (internalProblem == null) throw new ArgumentException($"Problem with internal id '{problem.InternalProblemId}' was not found. Problem id is '{problem.Id}'.");
            return ExtractStatementUrl(internalProblem);
        }

        public bool Exists(params string[] problemsIds)
        {
            var sortedProblemsIds = problemsIds.OrderBy(x => x);
            var existing = _archiveClient.Existing(problemsIds).OrderBy(x => x);
            return sortedProblemsIds.SequenceEqual(existing);
        }

        private string ExtractStatementUrl([NotNull] Bacs.Problem.Problem internalProblem)
        {
            var package = internalProblem.Statement.Version.First().Package;
            var hash = internalProblem.System.Revision.ToByteArray();

            return _statementProvider.GetStatementUrl(package, hash);
        }
    }
}