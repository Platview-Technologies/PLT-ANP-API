using Entities.Models;
using Entities.SystemModel;
using Shared.DTOs;
using Shared.DTOs.Response;
using Utilities.Enum;

namespace Service.Contract
{
    public interface INotificationService
    {
        Task CreateNotification(string message, string subject, string email);
        Task CreateNotification(string email, DealsModel deal, EmailTypeEnums type);
        Task<IEnumerable<NotificationModel>> GetPendingNotifications(int page, int pageSize);
        Task SendNotification(NotificationModel pendingEmail, SMTPSettings sMTP);
        Task<(IEnumerable<NotificationResponseDto> notifications, MetaData metaData)> GetNotifications(bool trackChanges, int page);
        Task<NotificationDto> GetNotification(Guid Id, bool trackChanges);
        
    }
}
