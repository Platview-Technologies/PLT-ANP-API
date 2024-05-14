using AutoMapper;
using Contract;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contract;
using Shared.DTOs;
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
            await DealExist(dealRequest.Name, dealRequest.ClientName, false);

            var dealToCreate = _mapper.Map<DealsModel>(dealRequest);
            _repository.Deal.CreateDeal(dealToCreate);
            await _repository.SaveAsync();
            var dealToReturn = _mapper.Map<DealResponseDto>(dealToCreate);
            return dealToReturn;
        }

        public async Task DeleteDeals(Guid id, bool trackChanges)
        {
            var dealToDelete = await DealExist(id, trackChanges);
            dealToDelete.ToDeletedEntity();
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<DealsModel>> GetActiveDeals(bool trackChanges)
        {
            var deals = await _repository.Deal.GetAllActiveDeals(trackChanges);
            return deals;
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
            await DealExist(dealUpdate.Name, dealUpdate.ClientName, trackChanges);
            var updatedDeal = _mapper.Map(dealUpdate, dealToUpdate);
            updatedDeal.ToUpdate();
            await _repository.SaveAsync();
        }

        private protected async Task DealExist(string name, string client, bool trackChanges)
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
       
                throw new NotFoundException(string.Format(ErrorMessage.ObjectNotFound, Constants.Deal));
            }
            return deal;
        }
        public async Task<(IEnumerable<PaginatedDealResponseDto> deals, MetaData metaData)> GetDeals(bool trackChanges, int page)
        {
            string pageSize = Environment.GetEnvironmentVariable("dealsPageSize");
            int DealPageSize;
            if (pageSize != null || pageSize == "")
            {
                DealPageSize = 10;
            } else
            {
                try
                {

                DealPageSize = int.Parse(pageSize);
                }
                catch
                {
                    DealPageSize = 10;
                }
            }
            var dealsWithMetaData = await _repository.Deal.GetDealsByPage(trackChanges, page, DealPageSize);
            var deals = _mapper.Map<IEnumerable<PaginatedDealResponseDto>>(dealsWithMetaData);
            foreach (var deal in deals)
            {
                // Query the database for the last notification associated with the deal
                var lastNotification = await _repository.Notification.GetLastSuccessFuleNotificationForDeal(deal.Id, trackChanges);

                // Map the last notification to DTO format if needed and set it to the deal's property
                deal.LastNotificationSent = _mapper.Map<NotificationResponseDto>(lastNotification);

            }
            return (deals, dealsWithMetaData.MetaData);
        }
    }
}
