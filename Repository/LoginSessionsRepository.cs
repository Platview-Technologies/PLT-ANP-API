using Contract;
using Entities.SystemModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class LoginSessionsRepository : RepositoryBase<LoginSessions, Guid>, ILoginSessionRepository
    {
        private readonly RepositoryContext _context;

        public LoginSessionsRepository(RepositoryContext context) : base(context)
        {
            _context = context;
        }

        public  void CreateLoginSession(LoginSessions sessions)
        {
            Create(sessions);
        }

        public async Task<LoginSessions> GetLoginSessionById(Guid sessionId, string RefreshToken, bool trackChanges)
        {
            return await FindByCondition(x => (x.Id == sessionId && x.RefreshToken == RefreshToken), trackChanges).Include(x => x.User).SingleOrDefaultAsync();
        }

        public async Task<LoginSessions> GetLoginSessionByToken(string RefreshToken, bool trackChanges)
        {
            return await FindByCondition(x => x.RefreshToken == RefreshToken, trackChanges).Include(x => x.User).SingleOrDefaultAsync();
        }

        public void DeleteSession(LoginSessions sessions)
        {
            _context.Remove(sessions);
        }

        public async Task<LoginSessions> GetLoginSessionByDeviceId(Guid deviceId, bool trackChanges)
        {
            return await FindByCondition(x => x.DeviceId == deviceId, trackChanges).Include(x => x.User).SingleOrDefaultAsync();
        }
    }
}
