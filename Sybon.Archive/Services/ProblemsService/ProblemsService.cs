using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Sybon.Archive.Repositories.ProblemsRepository;
using Sybon.Archive.Services.CollectionsService;
using Sybon.Archive.Services.GlobalCollectionService;
using Sybon.Archive.Services.InternalProblemsService;
using Sybon.Common;
using Problem = Sybon.Archive.Services.ProblemsService.Models.Problem;

namespace Sybon.Archive.Services.ProblemsService
{
    [UsedImplicitly]
    public class ProblemsService : IProblemsService
    {
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;
        private readonly IInternalProblemsService _internalProblemsService;
        private readonly IGlobalCollectionService _globalCollectionService;
        private readonly ICollectionsService _collectionsService;
        private readonly IMapper _mapper;

        public ProblemsService(IRepositoryUnitOfWork repositoryUnitOfWork, IMapper mapper, IInternalProblemsService internalProblemsService, ICollectionsService collectionsService, IGlobalCollectionService globalCollectionService)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _mapper = mapper;
            _internalProblemsService = internalProblemsService;
            _collectionsService = collectionsService;
            _globalCollectionService = globalCollectionService;
        }

        public async Task<string> GetStatementUrlAsync(long id)
        {
            var problem = await _repositoryUnitOfWork.GetRepository<IProblemsRepository>().FindAsync(id);
            if(problem == null) throw new ArgumentException($"Problem with id = {id} does not exist");

            return _internalProblemsService.ExtractStatementUrl(_mapper.Map<Problem>(problem));
        }

        public async Task<Problem> GetAsync(long id)
        {
            var problem = await _repositoryUnitOfWork.GetRepository<IProblemsRepository>().FindAsync(id);
            if(problem == null) throw new ArgumentException($"Problem with id = {id} does not exist");
            
            return _internalProblemsService.FetchProblemInfo(_mapper.Map<Problem>(problem));
        }

        public async Task<long> AddAsync(long collectionId, long globalProblemId)
        {
            var globalProblem = await _globalCollectionService.GetProblemAsync(globalProblemId);
            if(globalProblem == null)
                throw new KeyNotFoundException("Global problem not found");
            if(!_internalProblemsService.Exists(globalProblem.InternalProblemId))
                throw new KeyNotFoundException("Internal problem not found");
            if(!await _collectionsService.ExistsAsync(collectionId))
                throw new KeyNotFoundException("Collection not found");
            
            var dbEntry = new Repositories.ProblemsRepository.Problem
            {
                CollectionId = collectionId,
                GlobalProblemId = globalProblem.Id
            };
            await _repositoryUnitOfWork.GetRepository<IProblemsRepository>().AddAsync(dbEntry);
            await _repositoryUnitOfWork.SaveChangesAsync();
            return dbEntry.Id;
        }

        public async Task RemoveRangeAsync(long[] problemIds)
        {
            await _repositoryUnitOfWork.GetRepository<IProblemsRepository>().RemoveRangeAsync(problemIds);
            _repositoryUnitOfWork.SaveChanges();
        }
    }
}