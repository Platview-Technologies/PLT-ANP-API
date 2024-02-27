using AutoMapper;
using Contract;
using Contracts;
using Service.Contract;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IDealService> _dealService;
        public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
        {
            _dealService = new Lazy<IDealService>(() => new DealService(repositoryManager, logger, mapper));
        }
        public IDealService DealService => _dealService.Value;
    }
}
