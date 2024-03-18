using Entities.Models;
using Shared.DTOs.Response;
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
        Task<IEnumerable<UserToReturnDto>> GetAllTempUser();
        Task DeleteUser(Guid Id);
        Task<TempUserModel> GetTempUser(Guid? Id, bool trackChanges);
        
    }
}
