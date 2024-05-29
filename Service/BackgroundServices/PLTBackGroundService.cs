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
    /// <summary>
    /// Background service for processing and sending emails and notifications.
    /// </summary>
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

        /// <summary>
        /// Constructor for PLTBackGroundService.
        /// </summary>
        public PLTBackGroundService(IServiceScopeFactory serviceScopeFactory, IOptions<SMTPSettings> config,
            ILogger<PLTBackGroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _sMTPSettings = config.Value;
            _logger = logger;
            _lastKickStartTime = DateTime.Now;
        }

        //// <summary>
        /// Main execution method of the background service.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    //var context = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
                    _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    _service = scope.ServiceProvider.GetRequiredService<IServiceManager>();

                    // Process pending emails
                    await SendEmailsInBatchesAsync(stoppingToken);

                    // Create notifications
                    await CreateNotification();

                    // Process pending notifications
                    await SendNotificationsInBatchesAsync(stoppingToken);
                }

                // Delay before next iteration
                await Task.Delay(500000, stoppingToken);
            }
        }

        /// <summary>
        /// Sends emails in batches asynchronously.
        /// </summary>
        private async Task SendEmailsInBatchesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var pendingEmails = await _emailService.GetPendingEmails(0, pageSize);
                await SendEmailsAsync(pendingEmails);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Logger Initialized");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
                _logger.LogError(ex, "Error occurred while sending emails.");
            }
        }

        /// <summary>
        /// Sends notifications in batches asynchronously.
        /// </summary>
        private async Task SendNotificationsInBatchesAsync( CancellationToken cancellationToken)
        {
            try
            {
                var pendingNotifications = await _notificationService.GetPendingNotifications(0, pageSize);
                await SendNotificationsAsync(pendingNotifications, cancellationToken);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Logger Initialized");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
                _logger.LogError(ex, "Error occurred while sending notifications.");
            }
        }

        /// <summary>
        /// Sends emails asynchronously.
        /// </summary>
        private async Task SendEmailsAsync(IEnumerable<EmailModel> pendingEmails)
        {
            // Send each email asynchronously
            var tasks = pendingEmails.Select(async email => await _emailService.SendEmail(email, _sMTPSettings));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends notifications asynchronously.
        /// </summary>
        private async Task SendNotificationsAsync(IEnumerable<NotificationModel> pendingNotification, CancellationToken cancellationToken)
        {
            // Send each notification asynchronously
            var tasks = pendingNotification.Select(async notification => await _notificationService.SendNotification(notification, _sMTPSettings));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Creates notifications based on certain conditions.
        /// </summary>
        private async Task CreateNotification()
        {
            try
            {
                // Retrieve active deals
                var deals = await _service.DealService.GetActiveDeals(false);

                // Create notifications for each deal
                var tasks = deals.Select(async deals => await CheckDealsAndCreateNotificationAsync(deals));
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong while creating notifications: {ex.Message}");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Checks deals and creates notifications asynchronously.
        /// </summary>
        private async Task CheckDealsAndCreateNotificationAsync(DealsModel deal)
        {
            TimeSpan timeDifference = deal.ExpiryDate - DateTime.Today;
            NotificationModel lastNotification = deal.Notifications.Where(x => x.Status == MessageStatusEnums.Sent || x.Status == MessageStatusEnums.Pending)
                .OrderByDescending(x => x.UpdatedDate)
                .FirstOrDefault();

            // Check conditions and create appropriate notifications
            if (timeDifference.TotalDays <= 90 && timeDifference.TotalDays > 30)
            {
                if (lastNotification == null)
                {
                    await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Notification);
                    return;
                }
                else
                {
                    TimeSpan diff = DateTime.Today - lastNotification.UpdatedDate;
                    if (diff.TotalDays >= 14)
                    {
                        // Create notification for bi-weekly
                        await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Notification);
                        return;
                    }
                }
            }
            else if (timeDifference.TotalDays <= 30 && timeDifference.TotalDays > 1)
            {
                if (lastNotification == null)
                {
                    // Create notification
                    await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Reminder);
                    return;
                }
                else
                {
                    TimeSpan diff = DateTime.Today - lastNotification.UpdatedDate;
                    if (diff.TotalDays >= 7)
                    {
                        // Create notification for bi-weekly
                        await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Reminder);
                        return;
                    }
                }
            }
            else if (timeDifference.TotalDays <= 0 && timeDifference.TotalDays >= -1)
            {
                if (lastNotification != null)
                {
                    TimeSpan timeD = lastNotification.UpdatedDate - DateTime.Today;
                    if (timeD.TotalDays >= 1 )
                    {
                        await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Expiration);
                        return;
                    }
                }
                else
                {
                    await _notificationService.CreateNotification(deal.ContactEmail, deal, EmailTypeEnums.Expiration);
                }
            }
        }

    }
}
