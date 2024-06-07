using Entities.SystemModel;


namespace Contract
{
    public interface ILoginSessionRepository
    {
        Task<LoginSessions> GetLoginSessionById(Guid sessionId, string RefreshToken, bool trackChanges);

        Task<LoginSessions> GetLoginSessionByToken(string RefreshToken, bool trackChanges);

        void CreateLoginSession(LoginSessions sessions);
        void DeleteSession(LoginSessions sessions);
        Task<LoginSessions> GetLoginSessionByDeviceId(Guid deviceId, bool trackChanges);
    }
        
}
