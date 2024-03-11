using AutoMapper;
using Contract;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Service.Contract;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IDealService> _dealService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IUserManagementService> _userManagementService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            ILoggerManager logger,
            UserManager<UserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtConfiguration> configuration,
            IEmailService emailService,
            IMapper mapper)
        {
            _dealService = new Lazy<IDealService>(() => new DealService(repositoryManager, logger, mapper));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, mapper, userManager, configuration, roleManager, repositoryManager, emailService));
            _userManagementService = new Lazy<IUserManagementService>(() => new UserManagementService(logger, repositoryManager, mapper));
        }
        public IDealService DealService => _dealService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IUserManagementService UserManagementService => _userManagementService.Value;
    }
}
