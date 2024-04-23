using Entities.SystemModel;
using Shared.DTOs;

namespace Contract
{
    public interface INotificationRepository
    {
        void CreateNotification(NotificationModel notification);
        Task<NotificationModel> GetNotification(Guid Id, bool trackChanges);
        Task<IEnumerable<NotificationModel>> GetPendingNotifications(bool trackChanges, int page, int pageSize);
        Task<NotificationModel> GetLastSuccessFuleNotificationForDeal(Guid dealId, bool trackChnages);
        Task<PagedList<NotificationModel>> GetNotificationsAsync(bool trackChanges, int page);
    }
}
