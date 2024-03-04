using Entities.SystemModel;

using Utilities.Enum;
using Shared.DTOs.Response;

namespace Service.Contract
{
    public interface IEmailTemplateService
    {
        Task<EmailTemplateModel> GetEmailTemplate(EmailTypeEnums emailType, bool trackChanges);
        Task<EmailTemplateModel> GetEmailTemplate(Guid id, bool trackChanges);
        Task<EmailTemplateResponseDto> GetEmailTemplateToResponse(EmailTypeEnums emailType, bool trackChanges);
    }
}
