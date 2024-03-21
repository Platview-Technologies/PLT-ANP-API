using Entities.Models;


namespace Contract
{
    public interface IDealsRepository
    {
        void CreateDeal(DealsModel deal);
        Task<IEnumerable<DealsModel>> GetAllDeals(bool trackChanges);
        Task<IEnumerable<DealsModel>> GetAllActiveDeals(bool trackChanges);
        Task<DealsModel> GetDeal(Guid id, bool trackChanges);
        Task<DealsModel> GetDealByName(string client,string name, bool trackChanges);

    }
}
