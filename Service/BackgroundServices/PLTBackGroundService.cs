//using Entities.SystemModel;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Repository;
//using Service.Contract;

//namespace Service.BackgroundServices
//{
//    public class PLTBackGroundService : BackgroundService
//    {
//        public IServiceScopeFactory _serviceScopeFactory;
//        private const int page = 0;
//        private const int pageSize = 100;
//        private readonly SMTPSettings sMTPSettings;
//        private readonly ILogger _logger;
//        private readonly EmailService _emailService;

//        public PLTBackGroundService(IServiceScopeFactory serviceScopeFactory, IOptions<SMTPSettings> _config,
//            ILogger<PLTBackGroundService> logger, EmailService emailService)
//        {
//            _serviceScopeFactory = serviceScopeFactory;
//            _emailService = emailService;
//            sMTPSettings = _config.Value;
//        }
//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            try
//            {
//                while (!stoppingToken.IsCancellationRequested)
//                {
//                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
//                    IServiceProvider servicesProvider = scope.ServiceProvider;
//                    RepositoryContext context = servicesProvider.GetService<RepositoryContext>();

//                    await SendEmailsInBatches(context);
//                    await Task.Delay(50000, stoppingToken);
//                }
//            }
//            catch (Exception ex)
//            {

//                _logger.LogError(ex.Message);
//            }
//        }

//        private async Task SendEmailsInBatches(RepositoryContext context)
//        {
//            try
//            {
//                var pendingEmails = await _emailService.GetPendingEmails(page, pageSize);
//                await SendEmails(pendingEmails);
//                await context.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error Logger Initialzed");
//                Console.WriteLine(ex.Message);
//                Console.WriteLine(ex);

//                _logger.LogError(ex.Message);
//            }
//        }

//        private async Task SendEmails(IEnumerable<EmailModel> pendingEmails)
//        {
//            foreach (EmailModel pendingEmail in pendingEmails)
//            {
//                await _emailService.SendEmail(pendingEmail, sMTPSettings);

//            }
//        }
//    }
//}
using Entities.Models;
using Entities.SystemModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Repository;
using Service.Contract;
using Utilities.Enum;

namespace Service.BackgroundServices
{
    public class PLTBackGroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private const int pageSize = 100;
        private readonly SMTPSettings _sMTPSettings;
        private readonly ILogger<PLTBackGroundService> _logger;
        private IEmailService _emailService;
        private INotificationService _notificationService;
        private IServiceManager _service;
        private static DateTime _lastKickStartTime;

        public PLTBackGroundService(IServiceScopeFactory serviceScopeFactory, IOptions<SMTPSettings> config,
            ILogger<PLTBackGroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _sMTPSettings = config.Value;
            _logger = logger;
            _lastKickStartTime = DateTime.Now;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
                    _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    _service = scope.ServiceProvider.GetRequiredService<IServiceManager>();
                    await SendEmailsInBatchesAsync(context, stoppingToken);
                    await CreateNotification();
                    await SendNotificationsInBatchesAsync(context, stoppingToken);
                    
                }

                await Task.Delay(50000, stoppingToken);
            }
        }

        private async Task SendEmailsInBatchesAsync(RepositoryContext context, CancellationToken cancellationToken)
        {
            try
            {
                
                var pendingEmails = await _emailService.GetPendingEmails(0, pageSize);
                await SendEmailsAsync(pendingEmails);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Logger Initialzed");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
                _logger.LogError(ex, "Error occurred while sending emails.");
            }
        }
        private async Task SendNotificationsInBatchesAsync(RepositoryContext context, CancellationToken cancellationToken)
        {
            try
            {

                var pendingNotifications = await _notificationService.GetPendingNotifications(0, pageSize);
                await SendNotificationsAsync(pendingNotifications, context, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Logger Initialzed");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
                _logger.LogError(ex, "Error occurred while sending notifications.");
            }
        }


        private async Task SendEmailsAsync(IEnumerable<EmailModel> pendingEmails)
        {
            var tasks = pendingEmails.Select(email => _emailService.SendEmail(email, _sMTPSettings));
            await Task.WhenAll(tasks);
        }
        private async Task SendNotificationsAsync(IEnumerable<NotificationModel> pendingNotification, RepositoryContext context, CancellationToken cancellationToken)
        {
            var tasks = pendingNotification.Select(notification => _notificationService.SendNotification(notification, _sMTPSettings));
            await Task.WhenAll(tasks);
        }
        private async Task CreateNotification()
        {
            //TimeSpan timeDifference = DateTime.Now - _lastKickStartTime;

            //if (timeDifference.Hours >= 5) {
                    
            //}
            try
            {

                var deals = await _service.DealService.GetActiveDeals(false);
                var tasks = deals.Select(deals => CheckDealsAndCreateNotificationAsync(deals));
                await Task.WhenAll(tasks);

            } catch(Exception ex)
            {
                _logger.LogError($"Somethinng went wrong here while create ");
                Console.WriteLine(ex.Message);
            }
        }
        private async Task CheckDealsAndCreateNotificationAsync(DealsModel deal)
        {
            TimeSpan timeDifference = deal.RenewalDate - DateTime.Today;
            NotificationModel lastNotification = deal.Notifications.Where(x => x.Status == MessageStatusEnums.Sent || x.Status == MessageStatusEnums.Pending)
                .OrderByDescending(x => x.UpdatedDate)
                .FirstOrDefault();

            if (timeDifference.TotalDays <= 90 && timeDifference.TotalDays > 30)
            {
                if (lastNotification == null)
                {
                    await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Notification);
                    return;
                } else
                {
                    TimeSpan diff = DateTime.Today - lastNotification.UpdatedDate;
                    if (diff.TotalDays >= 14)
                    {
                        // create notification for by weekly
                        await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Notification);
                        return;
                    }
                }
            } else if (timeDifference.TotalDays <= 30 && timeDifference.TotalDays > 1 )
            {
                if (lastNotification == null)
                {
                    // create notification
                    await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Reminder);
                    return;

                }
                else
                {
                    TimeSpan diff = DateTime.Today - lastNotification.UpdatedDate;
                    if (diff.TotalDays >= 7)
                    {
                        // create notification for by weekly
                        await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Reminder);
                        return;
                    }
                }

            }
            else if (timeDifference.TotalDays <= 0)
            {
                await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Expiration);
                return;
            }
        }
    }
}