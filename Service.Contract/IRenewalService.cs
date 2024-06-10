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
    public interface IRenewalService
    {
        Task<RenewalResponseDto> CreateRenewal(RenewalRequestDto renewal);
        Task<RenewalResponseDto> GetRenewalAsync(Guid Id);
        Task<ICollection<RenewalResponseDto>> GetAllRenewalAsync();
        Task UpdateRenewal(Guid id);
        Task DeleteRenewal(Guid id);
        Task<(IEnumerable<RenewalResponseDto> renewals, MetaData metaData)> GetRenewalsPaginated(int page);
    }
}
