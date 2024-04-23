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
        Task<Guid> CreateTempUser(string email);
        Task<IEnumerable<UserToReturnUserDto>> GetAllTempUser();
        Task DeleteUser(Guid Id);
        Task<TempUserModel> GetTempUser(Guid? Id, bool trackChanges);
        Task<UserToReturnUserDto> GetUser(string UserId);


    }
}
