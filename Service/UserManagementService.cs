using AutoMapper;
using Contract;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contract;
using Utilities.Constants;

namespace Service
{
    public class UserManagementService : IUserManagementService
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        public UserManagementService(ILoggerManager logger, IRepositoryManager repository)
        {
            _logger = logger;
            _repository = repository;
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

        public async void DeleteUser(Guid Id)
        {
            var tempUser = await TempUserExist(Id, true);
            tempUser.ToDeletedEntity();
            _repository.Save();
        }

        public async Task<IEnumerable<TempUserModel>> GetAllTempUser()
        {
            return await _repository.TempUser.GetAllTempUser(false);
        }
        private protected async Task<TempUserModel> TempUserExist(Guid Id, bool trackChanges)
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
