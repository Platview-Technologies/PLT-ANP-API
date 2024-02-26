using Contract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DealsRepository : RepositoryBase<DealsModel, Guid>, IDealsRepository
    {
        public DealsRepository(RepositoryContext context): base(context)
        {
                   
        }
        public void CreateDeal(DealsModel deal)
        {
            Create(deal);
        }

        public async Task<IEnumerable<DealsModel>> GetAllDeals(bool trackChanges)
        {
            return await FindAll(trackChanges).OrderBy(x => x.Name).ToListAsync();

        }

        public async Task<DealsModel> GetDeal(Guid id, bool trackChanges)
        {
            return await FindByCondition(x => x.Id == id, trackChanges).SingleOrDefaultAsync();
        }
    }
    
}
