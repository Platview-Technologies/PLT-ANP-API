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
        Task<DealResponseDto> GetDeal(Guid id, bool trackChanges);
        Task<DealResponseDto> CreateDeal(DealRequestDto dealRequest, bool trackChanges);
        Task UpdateDeal(Guid id, DealUpdateDto dealUpdate, bool trackChanges);
        Task DeleteProject(Guid id, bool trackChanges);

    }
}
