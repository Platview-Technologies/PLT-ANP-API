using Contract;
using Entities.SystemModel;
using Service.Contract;
using Shared.DTOs.Response;
using Utilities.Enum;
using Utilities.Utilities;

namespace Service
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IRepositoryManager _repository;
        public EmailTemplateService(IRepositoryManager repository)
        {
             _repository = repository;
        }
        public async Task<EmailTemplateModel> GetEmailTemplate(EmailTypeEnums emailType, bool trackChanges)
        {
            EmailTemplateModel emailTemplate = await _repository.EmailTemplate.GetEmailTemplate(emailType, trackChanges);

            if (emailTemplate == null)
            {
                return new EmailTemplateModel()
                {
                    Template = DefaultTemplates.GetEmailTemplate(emailType),
                    Subject = DefaultTemplates.GetEmailSubject(emailType),
                };
            }

            return emailTemplate;
        }

        public async Task<EmailTemplateModel> GetEmailTemplate(Guid id, bool trackChanges)
        {
            return await _repository.EmailTemplate.GetEmailTemplate(id, trackChanges);
        }


        public async Task<EmailTemplateResponseDto> GetEmailTemplateToResponse(EmailTypeEnums emailType, bool trackChanges)
        {
            EmailTemplateResponseDto emailTemplate = await _repository.EmailTemplate.GetEmailTemplateToResponse(emailType, trackChanges);
            return ((emailTemplate == null) ? (new EmailTemplateResponseDto()
            {
                EmailType = emailType,
                Template = DefaultTemplates.GetEmailTemplate(emailType),
                Subject = DefaultTemplates.GetEmailSubject(emailType),
            }) : (emailTemplate));
        }
    }
}
