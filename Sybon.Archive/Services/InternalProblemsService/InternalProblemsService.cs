using System;
using System.Collections.Generic;
using System.Linq;
using Bacs.Archive.Client.CSharp;
using Bacs.Problem.Single;
using Bacs.StatementProvider;
using Google.Protobuf;
using JetBrains.Annotations;
using Sybon.Archive.Repositories.CollectionsRepository;
using Sybon.Archive.Services.ProblemsService.Models;

namespace Sybon.Archive.Services.InternalProblemsService
{
    [UsedImplicitly]
    public class InternalProblemsService : IInternalProblemsService
    {
        private readonly IArchiveClient _archiveClient;
        private readonly StatementProvider _statementProvider;

        public InternalProblemsService(IArchiveClient archiveClient, StatementProvider statementProvider)
        {
            _archiveClient = archiveClient;
            _statementProvider = statementProvider;
        }

        public Problem FetchProblemInfo(Problem problem)
        {
            var info = _archiveClient.ImportResult(problem.InternalProblemId);
            var internalProblem = info.SingleOrDefault(x => x.Key == problem.InternalProblemId).Value.Problem;
            if (internalProblem == null) throw new ArgumentException($"Problem with internal id '{problem.InternalProblemId}' was not found. Problem id is '{problem.Id}'.");
            var statementUrl = ExtractStatementUrl(internalProblem);

            var testGroups = ProfileExtension.Parser.ParseFrom(internalProblem.Profile.First().Extension.Value).TestGroup;
            var testsCount = testGroups.Sum(x => x.Tests.Query.Count);

            var pretestsCount = testGroups.FirstOrDefault(x => x.Id == "pre")?.Tests?.Query?.Count ?? 0;

            var resourceLimits = testGroups.First().Process.ResourceLimits;

            return new Problem
            {
                Id = problem.Id,
                Name = internalProblem.Info.Name.First().Value,
                StatementUrl = statementUrl,
                CollectionId = problem.CollectionId,
                TestsCount = testsCount,
                PretestsCount = pretestsCount,
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