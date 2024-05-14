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
        public async Task<Guid> CreateTempUser(string email)
        {
            await GetUserByEmail(email);
            TempUserModel tempUser = new()
            {
                Email = email
            };
            _repository.TempUser.CreateTempUser(tempUser);
            await _repository.SaveAsync();
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
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<UserToReturnUserDto>> GetAllTempUser()
        {
            var users = _mapper.Map<IEnumerable<UserToReturnUserDto>>(await _repository.TempUser.GetAllTempUser(false));
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

        private async Task<bool> GetUserByEmail(string email)
        {
            var tempUser = await _repository.TempUser.GetTempUser(email, false);
            if (tempUser != null)
            {
                throw new UserAlreadyExistException();
            }
            return true;

        }
        public async Task<UserToReturnUserDto> GetUser(string UserId)
        {
            try
            {
                var user_ = await _userManager.FindByIdAsync(UserId);
                var roles = await _userManager.GetRolesAsync(user_);
                var _ = await _repository.TempUser.GetTempUserByUserId(UserId, false);
                var user = _mapper.Map<UserToReturnUserDto>(_);
                user.Roles = roles;
                return user;
            }catch (Exception ex)
            {
                _logger.LogWarn(string.Format(Constants.User, ErrorMessage.ObjectNotFound));
                throw new NotFoundException(string.Format(Constants.User, ErrorMessage.ObjectNotFound));
            }
        }
    }
}
