

using Entities.SystemModel;
using Shared.DTOs.Response;
using Utilities.Enum;

namespace Contract
{
    public interface IEmailTemplateRepository
    {
        void CreateEmailTemplate(EmailTemplateModel emailTemplate);
        Task<EmailTemplateModel> GetEmailTemplate(EmailTypeEnums emailType, bool trackChanges);
        Task<EmailTemplateModel> GetEmailTemplate(Guid id, bool trackChanges);
        Task<EmailTemplateResponseDto> GetEmailTemplateToResponse(EmailTypeEnums emailType, bool trackChanges);
    }
}
