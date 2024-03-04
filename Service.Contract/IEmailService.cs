using Entities.SystemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IEmailService
    {
        void CreateEmail(string message, string subject, string email);
        Task<IEnumerable<EmailModel>> GetPendingEmails(int page, int pageSize);
        //Task<EmailContent> GetEmailContent(EmailModel pendingEmail);
        Task SendEmail(EmailModel pendingEmail, SMTPSettings sMTP);
    }
}
