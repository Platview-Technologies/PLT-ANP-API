using AutoMapper;
using Contract;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Service.Contract;
using Shared.DTOs.Response;
using Utilities.Constants;

namespace Service
{
    public class UserManagementService : IUserManagementService
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<UserModel> _userManager;

        public UserManagementService(ILoggerManager logger, IRepositoryManager repository, IMapper mapper, UserManager<UserModel> userManager)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }
        public Guid CreateTempUser(string email)
        {
            TempUserModel tempUser = new()
            {
                Email = email
            };
            _repository.TempUser.CreateTempUser(tempUser);
            _repository.Save();
            return tempUser.Id;
        }

        public async Task DeleteUser(Guid Id)
        {
            var tempUser = await _repository.TempUser.GetTempUser(Id, false);
            _repository.TempUser.DeleteTempUser(tempUser);
            var user = tempUser.UserModel;
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            _repository.Save();
        }

        public async Task<IEnumerable<UserToReturnDto>> GetAllTempUser()
        {
            var users = _mapper.Map<IEnumerable<UserToReturnDto>>(await _repository.TempUser.GetAllTempUser(false));
            return users;
        }
        public  async Task<TempUserModel> GetTempUser(Guid? Id, bool trackChanges)
        {
            var tempUser = await _repository.TempUser.GetTempUser(Id, trackChanges);
            if (tempUser == null)
            {
                _logger.LogWarn(string.Format(ErrorMessage.ObjectNotFound, "User"));
                throw new NotFoundException(string.Format(ErrorMessage.ObjectNotFound, "User"));
            }
            return tempUser;
        }
    }
}
