using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bacs.Archive.Client.CSharp;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Sybon.Archive.Services.CachedInternalProblemsService;
using Sybon.Archive.Services.ProblemsService;

namespace Sybon.Archive.HostedServices
{
    public class ArchiveCacheSynchronizer : BaseHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ArchiveCacheSynchronizer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        private static IEnumerable<string> GetOutdatedProblems(
            IReadOnlyDictionary<string, byte[]> newRevisions,
            IReadOnlyDictionary<string, byte[]> currentRevisions)
        {
            foreach (var newRevision in newRevisions)
            {
                if (!currentRevisions.TryGetValue(newRevision.Key, out var currentRevision)) continue;
                if (!currentRevision.SequenceEqual(newRevision.Value))
                    yield return newRevision.Key;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var failedIds = new List<string>();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var archiveClient = scope.ServiceProvider.GetService<IArchiveClient>();
                var cachedInternalProblemService = scope.ServiceProvider.GetService<ICachedInternalProblemsService>();
                var problemsService = scope.ServiceProvider.GetService<IProblemsService>();

                while (!cancellationToken.IsCancellationRequested)
                {
                    try // We want to retry update in case of fall
                    {
                        var globalRevision = await cachedInternalProblemService.ReadCacheRevisionAsync();

                        var statusResults = await archiveClient.StatusAllIfChangedAsync(globalRevision);
                        if (globalRevision != statusResults.revision)
                        {
                            var newRevisions = statusResults.statuses.ToDictionary(x => x.Key,
                                x => x.Value?.Status?.Revision?.ToByteArray() ?? new byte[0]);

                            var cachedProblems = await cachedInternalProblemService.GetAllRevisionsAsync();
                            var currentRevisions = cachedProblems.ToDictionary(x => x.InternalId, x => x.Revision);

                            var notCachedProblems =
                                newRevisions.Keys.Where(x => !currentRevisions.ContainsKey(x)).ToArray();
                            var outdatedProblems = GetOutdatedProblems(newRevisions, currentRevisions).ToArray();

                            failedIds.AddRange(await HandleBatchOperation(notCachedProblems,
                                cachedInternalProblemService.AddAsync));

                            failedIds.AddRange(await HandleBatchOperation(outdatedProblems,
                                cachedInternalProblemService.UpdateAsync));

                            var toBeAddedToGlobal = notCachedProblems.Except(failedIds);
                            await HandleBatchOperation(toBeAddedToGlobal,
                                async s => await problemsService.AddAsync(1, s));
                        }

                        failedIds.Clear();
                        if (statusResults.revision == null) statusResults.revision = string.Empty;

                        await cachedInternalProblemService.UpdateCacheRevisionAsync(statusResults.revision);
                        await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        private static async Task<List<string>> HandleBatchOperation(
            IEnumerable<string> internalIds,
            Func<string, Task> operation)
        {
            var failedIds = new List<string>();
            foreach (var internalId in internalIds)
            {
                try
                {
                    await operation(internalId);
                }
                catch (Exception)
                {
                    failedIds.Add(internalId);
                }
            }

            return failedIds;
        }
    }
}