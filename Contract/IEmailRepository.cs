using Entities.SystemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enum;

namespace Contract
{
    public interface IEmailRepository
    {
        void CreateEmailLog(EmailModel email);
        Task<IEnumerable<EmailModel>> GetAllPendingEmails(bool trackChanges, int page, int pageSize);
    }
}
