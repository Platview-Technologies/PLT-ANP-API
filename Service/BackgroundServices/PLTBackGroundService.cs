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
//                System.Console.WriteLine("Error Logger Initialzed");
//                System.Console.WriteLine(ex.Message);
//                System.Console.WriteLine(ex);

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
using Entities.SystemModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Repository;
using Service.Contract;
using static System.Formats.Asn1.AsnWriter;

namespace Service.BackgroundServices
{
    public class PLTBackGroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private const int pageSize = 100;
        private readonly SMTPSettings _sMTPSettings;
        private readonly ILogger<PLTBackGroundService> _logger;
        private IEmailService _emailService;

        public PLTBackGroundService(IServiceScopeFactory serviceScopeFactory, IOptions<SMTPSettings> config,
            ILogger<PLTBackGroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _sMTPSettings = config.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    using (var scope = _serviceScopeFactory.CreateScope())
            //    {
            //        var context = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
            //        _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            //        await SendEmailsInBatchesAsync(context, stoppingToken);
            //    }

            //    await Task.Delay(50000, stoppingToken);
            //}
        }

        private async Task SendEmailsInBatchesAsync(RepositoryContext context, CancellationToken cancellationToken)
        {
            try
            {
                
                var pendingEmails = await _emailService.GetPendingEmails(0, pageSize);
                await SendEmailsAsync(pendingEmails, context, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error Logger Initialzed");
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex);
                _logger.LogError(ex, "Error occurred while sending emails.");
            }
        }

        private async Task SendEmailsAsync(IEnumerable<EmailModel> pendingEmails, RepositoryContext context, CancellationToken cancellationToken)
        {
            var tasks = pendingEmails.Select(email => _emailService.SendEmail(email, _sMTPSettings));
            await Task.WhenAll(tasks);
        }
    }
}
