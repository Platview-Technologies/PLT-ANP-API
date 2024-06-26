﻿using Entities.Models;
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
        Task<TempUserModel> GetTempUser(Guid? Id, bool trackChanges);
        Task<TempUserModel> GetTempUser(string email, bool trackChanges);
        void DeleteTempUser(TempUserModel user);
        Task<TempUserModel> GetTempUserByUserId(string Id, bool trackChanges);
    }
}
