using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IUserManagementService
    {
        Guid CreateTempUser(string email);
        Task<IEnumerable<TempUserModel>> GetAllTempUser();
        void DeleteUser(Guid Id);
    }
}
