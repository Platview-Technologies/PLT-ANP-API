using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public interface IRepositoryManager
    {
        IDealsRepository Deal { get; }
        IEmailTemplateRepository EmailTemplate { get; }
        IEmailRepository Email { get; }
        ITempUserRepository TempUser { get; }
        void Save();
    }
}
