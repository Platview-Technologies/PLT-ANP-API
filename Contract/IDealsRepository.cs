using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public interface IDealsRepository
    {
        void CreateDeal(DealsModel deal);
        Task<IEnumerable<DealsModel>> GetAllDeals(bool trackChanges);
        Task<DealsModel> GetDeal(Guid id, bool trackChanges);
    }
}
