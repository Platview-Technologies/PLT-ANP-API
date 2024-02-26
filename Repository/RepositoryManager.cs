using Contract;


namespace Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IDealsRepository> _dealsRepository;
        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _dealsRepository = new Lazy<IDealsRepository>(() => new DealsRepository(repositoryContext));
        }
        public IDealsRepository Deal => _dealsRepository.Value;

        public void Save()
        {
            _repositoryContext.SaveChanges();
        }
    }
}
