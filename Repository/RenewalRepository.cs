using Contract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RenewalRepository : RepositoryBase<RenewalsModel, Guid>, IRenewalRepository
    {
        public RenewalRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateRenewal(RenewalsModel renewals) => Create(renewals);

        public async Task<PagedList<RenewalsModel>> GetPaginatedRenewal(bool trackChanges, int page, int pageSize)
        {
            var renewals = await FindByCondition(x => !x.IsDeleted, trackChanges)
               .Include(x => x.Deal)
               .OrderByDescending(r => r.CreatedDate)
               .ToListAsync();
            return PagedList<RenewalsModel>
                .ToPagedList(renewals, page, pageSize);
        }

        public async Task<RenewalsModel> GetRenewalByIdAsync(Guid Id, bool trackChanges) => await FindByCondition(x => x.Id == Id, trackChanges).Include(x => x.Deal).SingleOrDefaultAsync();

        public async Task<ICollection<RenewalsModel>> GetRenewals(bool trackChanges)
        {
            return await FindByCondition(x => !x.IsDeleted, trackChanges).Include(x => x.Deal).ToListAsync();
        }
    }
}
