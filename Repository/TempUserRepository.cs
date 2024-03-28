using Contract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TempUserRepository : RepositoryBase<TempUserModel, Guid>, ITempUserRepository
    {
        public TempUserRepository(RepositoryContext context): base(context)
        {
                
        }
        public void CreateTempUser(TempUserModel email)
        {
            Create(email);
        }

        public void DeleteTempUser(TempUserModel user)
        {
            Delete(user);
        }

        public async Task<IEnumerable<TempUserModel>> GetAllTempUser(bool trackChanges)
        {
            return await FindAll(trackChanges).OrderBy(x => x.CreatedDate).ToListAsync();
        }

        public async Task<TempUserModel> GetTempUser(Guid? Id, bool trackChanges)
        {
            var User = await FindByCondition(x => x.Id == Id, trackChanges).Include(x => x.UserModel).SingleOrDefaultAsync();
            return User;
        }

        public async Task<TempUserModel> GetTempUser(string email, bool trackChanges)
        {
            return await FindByCondition(x => x.Email == email, trackChanges).Include(x => x.UserModel).SingleOrDefaultAsync();
        }

        public async Task<TempUserModel> GetTempUserByUserId(string Id, bool trackChanges)
        {
            return await FindByCondition(x => x.UserId == Id, trackChanges).Include(x => x.UserModel).SingleOrDefaultAsync();
        }

    }
}
