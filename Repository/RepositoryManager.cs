using Contract;
using System.Runtime.CompilerServices;

namespace Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IDealsRepository> _dealsRepository;
        private readonly Lazy<IEmailRepository> _emailRepository;
        private readonly Lazy<IEmailTemplateRepository> _emailTemplateRepository;
        private readonly Lazy<ITempUserRepository> _tempUserRepository;
        private readonly Lazy<INotificationRepository> _notificationRepository;
        private readonly Lazy<ILoginSessionRepository> _loginSessionRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _dealsRepository = new Lazy<IDealsRepository>(() => new DealsRepository(repositoryContext));
            _emailRepository = new Lazy<IEmailRepository>(() => new EmailRepository(repositoryContext));
            _emailTemplateRepository = new Lazy<IEmailTemplateRepository>(() => new EmailTemplateRepository(repositoryContext));
            _tempUserRepository = new Lazy<ITempUserRepository>(() => new TempUserRepository(repositoryContext));
            _notificationRepository = new Lazy<INotificationRepository>(() => new NotificationRepository(repositoryContext));
            _loginSessionRepository = new Lazy<ILoginSessionRepository>(() => new LoginSessionsRepository(repositoryContext));
        }
        public IDealsRepository Deal => _dealsRepository.Value;
        public IEmailRepository Email => _emailRepository.Value;
        public IEmailTemplateRepository EmailTemplate => _emailTemplateRepository.Value;
        public ITempUserRepository TempUser => _tempUserRepository.Value;

        public INotificationRepository Notification => _notificationRepository.Value;
        public ILoginSessionRepository LoginSession => _loginSessionRepository.Value;

        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }

        
    }
}
