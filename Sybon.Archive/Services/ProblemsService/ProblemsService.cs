﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Sybon.Archive.Repositories.ProblemsRepository;
using Sybon.Archive.Services.CollectionsService;
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
        private readonly ICollectionsService _collectionsService;
        private readonly IMapper _mapper;

        public ProblemsService(IRepositoryUnitOfWork repositoryUnitOfWork, IMapper mapper, IInternalProblemsService internalProblemsService, ICollectionsService collectionsService)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _mapper = mapper;
            _internalProblemsService = internalProblemsService;
            _collectionsService = collectionsService;
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

        public async Task<long> AddAsync(long collectionId, string internalProblemId)
        {
            if(!_internalProblemsService.Exists(internalProblemId))
                throw new KeyNotFoundException("Problem not found");
            if(!await _collectionsService.ExistsAsync(collectionId))
                throw new KeyNotFoundException("Collection not found");
            
            var dbEntry = new Repositories.ProblemsRepository.Problem
            {
                CollectionId = collectionId,
                InternalProblemId = internalProblemId
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