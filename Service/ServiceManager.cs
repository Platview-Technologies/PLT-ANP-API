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
        public ServiceManager(
            IRepositoryManager repositoryManager,
            ILoggerManager logger,
            UserManager<UserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtConfiguration> configuration,
            IMapper mapper)
        {
            _dealService = new Lazy<IDealService>(() => new DealService(repositoryManager, logger, mapper));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, mapper, userManager, configuration, roleManager, repositoryManager));
        }
        public IDealService DealService => _dealService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}
