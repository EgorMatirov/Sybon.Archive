using System;
using System.Threading.Tasks;
using AutoMapper;
using Sybon.Archive.Repositories.GlobalCollectionRepository;
using Sybon.Archive.Services.InternalProblemsService;
using Sybon.Archive.Services.ProblemsService.Models;
using Sybon.Common;
using GlobalCollectionProblem = Sybon.Archive.Services.GlobalCollectionService.Models.GlobalCollectionProblem;

namespace Sybon.Archive.Services.GlobalCollectionService
{
    public class GlobalCollectionService : IGlobalCollectionService
    {
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;
        private readonly IMapper _mapper;
        private readonly IInternalProblemsService _internalProblemsService;
        
        public GlobalCollectionService(
            IRepositoryUnitOfWork repositoryUnitOfWork,
            IMapper mapper,
            IInternalProblemsService internalProblemsService)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _mapper = mapper;
            _internalProblemsService = internalProblemsService;
        }
        public async Task<GlobalCollectionProblem[]> GetAllAsync()
        {
            var dbEntries = await _repositoryUnitOfWork.GetRepository<IGlobalCollectionRepository>().GetAllAsync();
            return _mapper.Map<GlobalCollectionProblem[]>(dbEntries);
        }

        public async Task<long> AddAsync(string internalProblemId)
        {
            var dbEntry = new Repositories.GlobalCollectionRepository.GlobalCollectionProblem
            {
                InternalProblemId = internalProblemId
            };
            await _repositoryUnitOfWork.GetRepository<IGlobalCollectionRepository>().AddAsync(dbEntry);
            _repositoryUnitOfWork.SaveChanges();
            return dbEntry.Id;
        }

        public async Task RemoveRangeAsync(long[] problemIds)
        {
            await _repositoryUnitOfWork.GetRepository<IGlobalCollectionRepository>().RemoveRangeAsync(problemIds);
            _repositoryUnitOfWork.SaveChanges();
        }

        public async Task<GlobalCollectionProblem> GetProblemAsync(long problemId)
        {
            var problem = await _repositoryUnitOfWork.GetRepository<IGlobalCollectionRepository>().FindAsync(problemId);
            if(problem == null) throw new ArgumentException($"Problem with id = {problemId} does not exist");
            
            return _internalProblemsService.FetchProblemInfo(_mapper.Map<GlobalCollectionProblem>(problem));
        }
    }
}