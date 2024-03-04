using AutoMapper;
using Contract;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
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
        public void CreateTempUser(string email)
        {
            TempUserModel tempUser = new()
            {
                Email = email
            };
            _repository.TempUser.CreateTempUser(tempUser);
            _repository.Save();
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
                throw new NotFoundException(string.Format(ErrorMessage.ObjectNotFound, "User"));
            }
            return tempUser;
        }
    }
}
