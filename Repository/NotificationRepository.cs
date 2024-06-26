﻿using Contract;
using Entities.Models;
using Entities.SystemModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Utilities.Enum;

namespace Repository
{
    public class NotificationRepository : RepositoryBase<NotificationModel, Guid>, INotificationRepository
    {
        public NotificationRepository(RepositoryContext context):base(context)
        {
       
        }

        public async Task<NotificationModel> GetNotification(Guid Id, bool trackChanges)
        {
            var _ = await FindByCondition(x => x.Id == Id, trackChanges).Include(x => x.Deal).SingleOrDefaultAsync();
            return _;
        }

        public async Task<IEnumerable<NotificationModel>> GetPendingNotifications(bool trackChanges, int page, int pageSize)
        {
            return await FindByCondition(x => x.Status == MessageStatusEnums.Pending, trackChanges)
                 .OrderBy(r => r.UpdatedDate)
                                      .Skip(page)
                                     .Take((page + 1) * pageSize)
                                      .ToListAsync();
        }

        public void CreateNotification(NotificationModel notification) => Create(notification);
        public async Task<NotificationModel> GetLastSuccessFuleNotificationForDeal(Guid dealId, bool trackChnages)
        {
            return await FindByCondition(x => x.DealId == dealId, trackChnages)
                .Where(x => x.Status == MessageStatusEnums.Sent)
                .OrderBy(x => x.UpdatedDate).FirstOrDefaultAsync();

        }

        public async Task<PagedList<NotificationModel>> GetNotificationsAsync(bool trackChanges, int page)
        {
            var notifications = await FindByCondition(x => !x.IsDeleted, trackChanges)
                .Include(x => x.Deal)
                .OrderByDescending(x => x.UpdatedDate)
                .ToListAsync();

            return PagedList<NotificationModel>
               .ToPagedList(notifications, page, 20);
        }
    }
}
