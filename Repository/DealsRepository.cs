using Contract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    public class DealsRepository : RepositoryBase<DealsModel, Guid>, IDealsRepository
    {
        public DealsRepository(RepositoryContext context): base(context)
        {
        }
        public void CreateDeal(DealsModel deal) => Create(deal);
        

        public async Task<IEnumerable<DealsModel>> GetAllDeals(bool trackChanges) => await FindAll(trackChanges).OrderBy(x => x.Name).ToListAsync();

        

        public async Task<DealsModel> GetDeal(Guid id, bool trackChanges) => 
            await FindByCondition(x => x.Id == id, trackChanges).SingleOrDefaultAsync();
        
    }
    
}
