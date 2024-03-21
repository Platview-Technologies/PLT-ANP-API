using Entities.Models;
using Entities.SystemModel;
using Utilities.Enum;

namespace Service.Contract
{
    public interface INotificationService
    {
        Task CreateNotification(string message, string subject, string email);
        Task CreateNotification(string email, DealsModel deal, EmailTypeEnums type);
        Task<IEnumerable<NotificationModel>> GetPendingNotifications(int page, int pageSize);
        Task SendNotification(NotificationModel pendingEmail, SMTPSettings sMTP);

    }
}
