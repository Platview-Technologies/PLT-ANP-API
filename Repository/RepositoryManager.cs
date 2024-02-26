using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
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
