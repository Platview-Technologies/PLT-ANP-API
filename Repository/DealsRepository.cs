using Contract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    internal class DealsRepository : RepositoryBase<DealsModel, Guid>, IDealsRepository
    {
        public DealsRepository(RepositoryContext context): base(context)
        {
        }
        public void CreateDeal(DealsModel deal) => Create(deal);

        public async Task<IEnumerable<DealsModel>> GetAllActiveDeals(bool trackChanges)
        {
            return  await FindByCondition(x => x.IsActive, trackChanges).Include(x => x.Notifications).ToListAsync();
            
        }

        public async Task<IEnumerable<DealsModel>> GetAllDeals(bool trackChanges) => await FindAll(trackChanges).OrderBy(x => x.Name).ToListAsync();



        public async Task<DealsModel> GetDeal(Guid id, bool trackChanges) => 
            await FindByCondition(x => x.Id == id, trackChanges).SingleOrDefaultAsync();

        public async Task<DealsModel> GetDealByName(string client, string name, bool trackChanges) =>
            await FindByCondition(x => x.ClientName.ToLower() == client, trackChanges)
            .Where(x => x.Name.ToLower() == name.ToLower())
            .SingleOrDefaultAsync();
    }
    
}
