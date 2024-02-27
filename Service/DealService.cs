using AutoMapper;
using Contract;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contract;
using Shared.DTOs.Request;
using Shared.DTOs.Response;
using Utilities.Constants;

namespace Service
{
    internal sealed class DealService : IDealService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public DealService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<DealResponseDto> CreateDeal(DealRequestDto dealRequest, bool trackChanges)
        {
            await DealExist(dealRequest.ClientName, dealRequest.Name, false);

            var dealToCreate = _mapper.Map<DealsModel>(dealRequest);
            _repository.Deal.CreateDeal(dealToCreate);
            _repository.Save();
            var dealToReturn = _mapper.Map<DealResponseDto>(dealToCreate);
            return dealToReturn;
        }

        public async Task DeleteProject(Guid id, bool trackChanges)
        {
            var dealToDelete = await DealExist(id, trackChanges);
            dealToDelete.ToDeletedEntity();
            _repository.Save();
        }

        public async Task<DealResponseDto> GetDeal(Guid id, bool trackChanges)
        {
            var DealToReturn = _mapper.Map<DealResponseDto>(await DealExist(id, trackChanges));
            return DealToReturn;
        }

        public async Task<IEnumerable<DealResponseDto>> GetDeals(bool trackChanges)
        {
            var dealsToReturn = _mapper.Map<IEnumerable<DealResponseDto>>(await _repository.Deal.GetAllDeals(trackChanges));
            return dealsToReturn;
        }

        public async Task UpdateDeal(Guid id, DealUpdateDto dealUpdate, bool trackChanges)
        {
            var dealToUpdate = await DealExist(id, trackChanges);
            var updatedDeal = _mapper.Map(dealUpdate, dealToUpdate);
            updatedDeal.ToUpdate();
            _repository.Save();
        }

        private protected async Task DealExist(string name,string client, bool trackChanges)
        {
            var deal = await _repository.Deal.GetDealByName(client, name, trackChanges);
            if (deal != null)
            {
                throw new ClientDealExistsException(ErrorMessage.ClientAreadyHasThisDeal);
            }
            
        }
        private protected async Task<DealsModel> DealExist(Guid id, bool trackChanges) 
        {
            var deal = await _repository.Deal.GetDeal(id, trackChanges);
            if (deal == null)
            {
                _logger.LogInfo(string.Format(ErrorMessage.ObjectNotFound, Constants.Deal));
                throw new NotFoundException(string.Format(ErrorMessage.ObjectNotFound, Constants.Deal));
            }
            return deal;
        }
        
    }
}
