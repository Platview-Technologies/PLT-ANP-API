using Entities.Models;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public interface IRenewalRepository
    {
        void CreateRenewal(RenewalsModel renewals);
        Task<RenewalsModel> GetRenewalByIdAsync(Guid Id, bool trackChanges);
        Task<ICollection<RenewalsModel>> GetRenewals(bool trackChanges);
        Task<PagedList<RenewalsModel>> GetPaginatedRenewal(bool trackChanges, int page, int pageSize);
    }
}
