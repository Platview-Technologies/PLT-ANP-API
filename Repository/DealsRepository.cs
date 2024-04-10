using Contract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace Repository
{
    internal class DealsRepository : RepositoryBase<DealsModel, Guid>, IDealsRepository
    {
        public DealsRepository(RepositoryContext context) : base(context)
        {
        }
        public void CreateDeal(DealsModel deal) => Create(deal);

        public async Task<IEnumerable<DealsModel>> GetAllActiveDeals(bool trackChanges)
        {
            return await FindByCondition(x => x.IsActive, trackChanges).Include(x => x.Notifications).ToListAsync();

        }
        public async Task<PagedList<DealsModel>> GetDealsByPage(bool trackChanges, int page, int pageSize)
        {
            var deals = await FindAll(trackChanges)
                .Include(x => x.Notifications)
                .OrderBy(r => r.UpdatedDate)
                .ToListAsync();
            return PagedList<DealsModel>
                .ToPagedList(deals, page, pageSize);
        }
        public async Task<IEnumerable<DealsModel>> GetAllDeals(bool trackChanges) => await FindAll(trackChanges).OrderBy(x => x.Name).ToListAsync();



        public async Task<DealsModel> GetDeal(Guid id, bool trackChanges) =>
            await FindByCondition(x => x.Id == id, trackChanges).SingleOrDefaultAsync();

        public async Task<DealsModel> GetDealByName(string client, string name, bool trackChanges) {
            return await FindByCondition(x => x.ClientName.ToLower() == client.ToLower(), trackChanges)
            .Where(x => x.Name.ToLower() == name.ToLower())
            .FirstOrDefaultAsync();
            
            }
    }
    
}
