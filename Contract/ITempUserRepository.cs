using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public interface ITempUserRepository
    {
        void CreateTempUser(TempUserModel email);
        Task<IEnumerable<TempUserModel>> GetAllTempUser(bool trackChanges);
        Task<TempUserModel> GetTempUser(Guid Id, bool trackChanges);
    }
}
