using Entities.Models;
using Shared.DTOs;
using Shared.DTOs.Request;
using Shared.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IDealService
    {
        Task<IEnumerable<DealResponseDto>> GetDeals(bool trackChanges);
        Task<IEnumerable<DealsModel>> GetActiveDeals(bool trackChanges);
        Task<DealResponseDto> GetDeal(Guid id, bool trackChanges);
        Task<DealResponseDto> CreateDeal(DealRequestDto dealRequest, bool trackChanges);
        Task UpdateDeal(Guid id, DealUpdateDto dealUpdate, bool trackChanges);
        Task DeleteDeals(Guid id, bool trackChanges);
        Task<(IEnumerable<PaginatedDealResponseDto> deals, MetaData metaData)> GetDeals(bool trackChanges, int page);
    }
}
