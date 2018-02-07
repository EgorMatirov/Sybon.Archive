using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Sybon.Archive.Repositories.CachedInternalProblemsRepository;
using Sybon.Archive.Repositories.CollectionsRepository;
using Sybon.Archive.Services.CachedInternalProblemsService;
using Sybon.Archive.Services.CollectionsService.Models;
using Sybon.Common;

namespace Sybon.Archive.Services.CollectionsService
{
    [UsedImplicitly]
    public class CollectionsService : ICollectionsService
    {
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;
        private readonly IMapper _mapper;
        private readonly ICachedInternalProblemsService _cachedInternalProblemsService;
        
        public CollectionsService(IRepositoryUnitOfWork repositoryUnitOfWork, IMapper mapper, ICachedInternalProblemsService cachedInternalProblemsService)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _mapper = mapper;
            _cachedInternalProblemsService = cachedInternalProblemsService;
        }

        public async Task<long> AddAsync(long userId, CollectionForm collection)
        {
            // TODO: Add permission by userId?
            if (collection.Problems != null)
            {
                var problemsIds = collection.Problems.Select(x => x.InternalProblemId).ToArray();
                var allProblemsExists = await _cachedInternalProblemsService.ExistsAsync(problemsIds);
                if(!allProblemsExists)
                    throw new KeyNotFoundException("Problem not found");
            }
            var dbEntry = _mapper.Map<Collection>(collection);
            await _repositoryUnitOfWork.GetRepository<ICollectionsRepository>().AddAsync(dbEntry);
            await _repositoryUnitOfWork.SaveChangesAsync();
            return dbEntry.Id;
        }

        public async Task<ProblemCollection> FindAsync(long id)
        {
            var dbEntry = await _repositoryUnitOfWork.GetRepository<ICollectionsRepository>().FindAsync(id);
            var problemCollection = _mapper.Map<ProblemCollection>(dbEntry);
            var tasks = problemCollection.Problems.Select(_cachedInternalProblemsService.FetchProblemInfoAsync);
            var results = await Task.WhenAll(tasks);
            problemCollection.Problems = results
                .Where(x => x != null)
                .ToArray();
            return problemCollection;
        }

        public async Task<ProblemCollection[]> GetAll()
        {
            var dbEntries = await _repositoryUnitOfWork.GetRepository<ICollectionsRepository>().GetAllWithProblemsCount();
            var mapped = dbEntries.Select(e => _mapper.Map<ProblemCollection>(e));
            return mapped.ToArray();
        }

        public Task<bool> ExistsAsync(long id)
        {
            return _repositoryUnitOfWork.GetRepository<ICollectionsRepository>().ExistsAsync(id);
        }

        public async Task RemoveRangeAsync(long[] collectionIds)
        {
            await _repositoryUnitOfWork.GetRepository<ICollectionsRepository>().RemoveRangeAsync(collectionIds);
            _repositoryUnitOfWork.SaveChanges();
        }
    }
}