using Contract;
using Entities.SystemModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Response;
using Utilities.Enum;

namespace Repository
{
    public class EmailTemplateRepository : RepositoryBase<EmailTemplateModel, Guid>, IEmailTemplateRepository
    {
        public EmailTemplateRepository(RepositoryContext context) : base(context)
        {

        }
        public void CreateEmailTemplate(EmailTemplateModel emailTemplate)
        {
            Create(emailTemplate);
        }

        public async Task<EmailTemplateModel> GetEmailTemplate(EmailTypeEnums emailType, bool trackChanges)
        {
            return await FindByCondition(x => x.EmailType == emailType, trackChanges).FirstOrDefaultAsync();
        }

        public async Task<EmailTemplateModel> GetEmailTemplate(Guid id, bool trackChanges)
        {
            return await FindByCondition(x => x.Id == id, trackChanges).FirstOrDefaultAsync();
        }

        public async Task<EmailTemplateResponseDto> GetEmailTemplateToResponse(EmailTypeEnums emailType, bool trackChanges)
        {
            return await FindByCondition(r => r.EmailType == emailType, trackChanges)
                .Select(r => new EmailTemplateResponseDto()
                 {
                     EmailType = r.EmailType,
                     Id = r.Id,
                     Template = r.Template,
                     Subject = r.Subject
                 })
                .FirstOrDefaultAsync();
        }
    }
}
