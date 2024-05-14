using Contract;
using Contracts;
using Entities.Models;
using Entities.SystemModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using Service.Contract;
using Utilities.Constants;
using Utilities.Enum;
using Utilities.Utilities;
//using System.Net.Mail;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly IRepositoryManager _repository;
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailTemplateService _emailTemplate;
        private readonly ILoggerManager _logger;

        public EmailService(IRepositoryManager repository, UserManager<UserModel> userManager, IEmailTemplateService emailTemplate, ILoggerManager logger)
        {
            _repository = repository;
            _userManager = userManager;
            _emailTemplate = emailTemplate;
            _logger = logger;
        }
        public async Task CreateEmail(string message, string subject, string email)
        {
            EmailModel _email = new EmailModel()
            {
                Emailaddresses = email,
                UpdatedDate = DateTime.Now,
                Message = message,
                Status = MessageStatusEnums.Pending,
                Subject = subject,
            };
            _repository.Email.CreateEmailLog(_email);
            await _repository.SaveAsync();
        }
        public async Task CreateEmail(string email, Guid? userId, string? token, EmailTypeEnums emailType)
        {
            EmailModel _email = new EmailModel()
            {
                Emailaddresses = email,
                UpdatedDate = DateTime.Now,
                Status = MessageStatusEnums.Pending,
                EmailType = emailType,
                NewUserActivationToken = token,
                UserId = userId,
                ChangeOrResetUserId = userId.ToString()
            };
            _repository.Email.CreateEmailLog(_email);
            await _repository.SaveAsync();
        }
        //public async Task<List<EmailModel>> GetPendingEmails(int page, int pageSize)
        //{
        //    var pendingEmails = await FindByCondition(x => x.Status == MessageStatusEnums.Pending, true)
        //        .OrderBy(r => r.UpdatedDate)
        //                             .Skip(page)
        //                            .Take((page + 1) * pageSize)
        //                             .ToListAsync();



        //    return pendingEmails;
        //}
        public async Task<IEnumerable<EmailModel>> GetPendingEmails(int page, int pageSize)
        {
            return await _repository.Email.GetAllPendingEmails(true, page, pageSize);

        }
        public async Task SendEmail(EmailModel pendingEmail, SMTPSettings sMTP)
        {
            try
            {
                if (sMTP == null)
                {
                    pendingEmail.Status = MessageStatusEnums.Failed;
                    pendingEmail.FailedDate = DateTime.Now;
                    pendingEmail.ResponseMessage = "SMTP not configure yet";
                    _logger.LogError("SMTP not configured yet");

                    return;
                }
                List<string> confirmedEmails = new List<string>();
                List<string> unConfirmedEmails = new List<string>();
                string[] splittedEmails = await SortEmails(pendingEmail, confirmedEmails, unConfirmedEmails);

                if (confirmedEmails.Count() == 0)
                {
                    pendingEmail.Status = MessageStatusEnums.Failed;
                    pendingEmail.FailedDate = DateTime.Now;
                    pendingEmail.ResponseMessage = "Email(s) does not exist or their respective domain(s) has expired";

                    return;
                }

                EmailContent emailContent = await GetEmailContent(pendingEmail);

                //MailMessage mail = new MailMessage()
                //{

                //    From = new MailAddress(sMTP.FromEmail),
                //    IsBodyHtml = true,
                //    Subject = emailContent.Subject,
                //    Body = WebUtility.HtmlDecode(emailContent.Message),
                //};
                //mail.To.Add(string.Join(",", confirmedEmails));
                var mail = new MimeMessage();
                mail.From.Add(MailboxAddress.Parse(sMTP.FromEmail));
                mail.To.Add(MailboxAddress.Parse(string.Join(",", confirmedEmails)));
                mail.Subject = emailContent.Subject;
                mail.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = emailContent.Message
                };

                var ccEmails = Environment.GetEnvironmentVariable(Constants.ccEmail);
                if (!string.IsNullOrEmpty(ccEmails))
                {
                    var ccEmailList = ccEmails.Split(Constants.Comma).Select(x => x.Trim()).ToList();
                    foreach (var ccEmail in ccEmailList)
                    {
                        mail.Cc.Add(MailboxAddress.Parse(ccEmail));
                    }
                }

                using (var client = new SmtpClient())
                {
                    //client.EnableSsl = sMTP.SSLStatus;
                    //client.Host = sMTP.HostServer;
                    //client.Port = sMTP.HostPort;
                    //client.Credentials = new NetworkCredential(sMTP.FromEmail, sMTP.FromEmailPassword);
                    //client.UseDefaultCredentials = true;

                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await client.ConnectAsync(sMTP.HostServer, sMTP.HostPort, SecureSocketOptions.StartTls);
                    
                    await client.AuthenticateAsync(sMTP.FromEmail, sMTP.FromEmailPassword);
                    var sentConfirmation = await client.SendAsync(mail);
                    await client.DisconnectAsync(true);

                    if (confirmedEmails.Count() == splittedEmails.Count())
                    {
                        pendingEmail.Status = MessageStatusEnums.Sent;
                        pendingEmail.Sentdate = DateTime.Now;
                        pendingEmail.ResponseMessage = sentConfirmation;
                        _logger.LogInfo("sent....");
                    }
                    else
                    {
                        pendingEmail.Status = MessageStatusEnums.SentPartially;
                        pendingEmail.ResponseMessage = "Unable to send to these emails: " + string.Join(",", unConfirmedEmails) + ". Either Email(s) does not exist or their respective domain(s) has expired";
                        pendingEmail.Sentdate = DateTime.Now;
                        pendingEmail.FailedDate = DateTime.Now;
                        _logger.LogInfo("sent....");
                    }
                }
            }
            catch (IOException ioEx)
            {
                // IOException occurred (retryable)
                HandleRetryableError(ioEx, pendingEmail);
            }
            catch (TimeoutException timeoutEx)
            {
                // TimeoutException occurred (retryable)
                HandleRetryableError(timeoutEx, pendingEmail);
            }
            catch (SmtpCommandException smtpEx)
            {
                // SmtpCommandException occurred (retryable)
                HandleRetryableError(smtpEx, pendingEmail);
            }
            catch (SmtpProtocolException protocolEx)
            {
                // SmtpProtocolException occurred (retryable)
                HandleRetryableError(protocolEx, pendingEmail);
            }
            catch (Exception ex)
            {
                // Other exceptions
                HandleNonRetryableError(ex, pendingEmail);
            }
        }
        private void HandleRetryableError(Exception ex, EmailModel pendingEmail)
        {
            // Log the error
            _logger.LogError(ex.Message + " retrying......");

            // Set email status as failed
            pendingEmail.Status = MessageStatusEnums.Pending;
            pendingEmail.FailedDate = DateTime.Now;
            pendingEmail.Sentdate = DateTime.Now;
            pendingEmail.ResponseMessage = ex.Message;
        }

        private void HandleNonRetryableError(Exception ex, EmailModel pendingEmail)
        {
            // Log the error
            _logger.LogError(ex.Message);
            // Set email status as failed
            pendingEmail.Status = MessageStatusEnums.Failed;
            pendingEmail.FailedDate = DateTime.Now;
            pendingEmail.ResponseMessage = ex.Message;

            // Additional error handling for non-retryable errors
            // (e.g., send notification to administrators, escalate the issue, etc.)
        }
        //public Task SendEmail(EmailModel pendingEmail, SMTPSettings sMTP)
        //{
        //    throw new NotImplementedException();
        //}
        private async Task<EmailContent> GetEmailContent(EmailModel pendingEmail)
        {
            EmailContent emailContent = new EmailContent();

            if (pendingEmail.EmailType.HasValue)
            {
                emailContent = await ProcessMessage(pendingEmail);
            }
            else
            {
                emailContent = new EmailContent()
                {
                    Subject = pendingEmail.Subject,
                    Message = pendingEmail.Message
                };
            }
            emailContent.Message = await AppendHeaderAndFooterToEmail(emailContent.Message);
            return emailContent;
        }

        private async Task<EmailContent> ProcessMessage(EmailModel email)
        {
            EmailContent subjectAndMessage = new EmailContent();

            if (email.EmailType.HasValue)
            {
                switch (email.EmailType.Value)
                {
                    case EmailTypeEnums.NewAccount:
                        subjectAndMessage = await GetUserSubjectAndMessage(email);
                        break;
                    case EmailTypeEnums.AccountActivation:
                        subjectAndMessage = await GetUserSubjectAndMessage(email);
                        break;
                    case EmailTypeEnums.ResetPassword:
                        if (!string.IsNullOrEmpty(email.ChangeOrResetUserId))
                        {
                            subjectAndMessage = await GetUserSubjectAndMessage(email);
                        }
                        break;
                    case EmailTypeEnums.ChangePassword:
                        if (!string.IsNullOrEmpty(email.ChangeOrResetUserId))
                        {
                            subjectAndMessage = await GetUserSubjectAndMessage(email);
                        }
                        break;
                    case EmailTypeEnums.UserRegistration:
                        subjectAndMessage = await GetUserSubjectAndMessage(email);
                        break;
                    default:
                        break;

                }
            }
            return subjectAndMessage;

        }

        private async Task<EmailContent> GetUserSubjectAndMessage(EmailModel email)
        {

            
            var Tempuser = await _repository.TempUser.GetTempUser(email.UserId, false);
            var user = Tempuser.UserModel;
            if (email.EmailType == EmailTypeEnums.UserRegistration)
            {
                user = new()
                {
                    Email = "null@null.com",
                    FirstName = "null",
                    PhoneNumber = "null",
                    LastName = "null"
                };
            }

            if (user == null)
            {
                return new EmailContent();
            }


            
            EmailTemplateModel template = await _emailTemplate.GetEmailTemplate(email.EmailType.Value, false);

            string message = template.Template
                .Replace(TagName.FirstName, user.FirstName)
                .Replace(TagName.Surname, user.LastName)
                .Replace(TagName.EmailAddress, user.Email)
                .Replace(TagName.ActivationToken, string.IsNullOrEmpty(email.NewUserActivationToken) ? "" : email.NewUserActivationToken)
                .Replace(TagName.UserId, email.UserId.ToString())
                .Replace(TagName.BaseUrl, Environment.GetEnvironmentVariable("BaseUrl"));

            string subject = template.Subject
                .Replace(TagName.FirstName, user.FirstName)
                .Replace(TagName.Surname, user.LastName)
                .Replace(TagName.EmailAddress, user.Email)
                .Replace(TagName.PhoneNumber, user.PhoneNumber)
                .Replace(TagName.UserId, email.UserId.ToString());

            return new EmailContent()
            {
                Message = message,
                Subject = subject
            };
        }

        //private async Task UserEmail(string userId, EmailTypeEnums emailType)
        //{
        //    var user = await _userManager.FindByEmailAsync(userId);
        //    var tempUser = await
        //    if (user == null)
        //    {
        //        return;
        //    }

        //    EmailModel email = new EmailModel()
        //    {
        //        Emailaddresses = user.Email,
        //        EmailType = emailType,
        //        Status = MessageStatusEnums.Pending,
        //        UserId = ((emailType == EmailTypeEnums.NewAccount ||
        //        emailType == EmailTypeEnums.AccountActivation) ? (userId) : (null)),
        //        ChangeOrResetUserId = ((emailType == EmailTypeEnums.NewAccount) ?
        //        (null) : (userId)),
        //        Id = Guid.NewGuid()
        //    };

        //    _repository.Email.CreateEmailLog(email);
        //    _repository.Save();
        //}

        private async Task<string> AppendHeaderAndFooterToEmail(string message)
        {
           
            EmailTemplateModel headerTemplate = await _emailTemplate.GetEmailTemplate(EmailTypeEnums.EmailHeader, false);
            EmailTemplateModel footerTemplate = await _emailTemplate.GetEmailTemplate(EmailTypeEnums.EmailFooter, false);
            string finaltemplate = WrapMessageWithHTMLTableROW(headerTemplate.Template) +
                WrapMessageWithHTMLTableROW(message) + WrapMessageWithHTMLTableROW(footerTemplate.Template);

            return AddHTMLTable(finaltemplate);

        }
        private string WrapMessageWithHTMLTableROW(string message)
        {
            string HtmlTable = "";

            HtmlTable += "<tr>";
            HtmlTable += "<td rowspan='1' colspan='1' style='padding:20px;" + "'>";
            HtmlTable += message;
            HtmlTable += "</td>";
            HtmlTable += "</tr>";

            return HtmlTable;
        }
        private string AddHTMLTable(string message)
        {
            string HtmlTable = "<div align='center'>";
            HtmlTable += "<table style='background-color:#eeeeee;box-shadow: 0 .5rem 1rem rgba(0,0,0,.15) !important;' bgcolor='#eeeeee' border='0' width='100%' cellspacing='0' cellpadding='0'>";
            HtmlTable += "<tbody>";
            HtmlTable += "<tr>";
            HtmlTable += "<td rowspan='1' colspan='1' align='center' style='line-height:24px;'>";

            HtmlTable += "<table width='600' style='background-color:#ffffff;'bgcolor='#ffffff'border='0'cellspacing='0' cellpadding='0'>";
            HtmlTable += "<tbody>";

            HtmlTable += message;

            HtmlTable += "</tbody>";
            HtmlTable += "</table>";

            HtmlTable += "</td>";
            HtmlTable += "</tr>";
            HtmlTable += "</tbody>";
            HtmlTable += "</table>";
            HtmlTable += "</div>";

            return HtmlTable;
        }


        private static async Task<string[]> SortEmails(EmailModel pendingEmail, List<string> confirmedEmails, List<string> unConfirmedEmails)
        {
            string[] splittedEmails = pendingEmail.Emailaddresses.Split(",");
            DNSCheck dNSCheck = new DNSCheck();
            foreach (string email in splittedEmails)
            {
                if (await dNSCheck.IsEmailValid(email))
                {
                    confirmedEmails.Add(email);
                }
                else
                {
                    unConfirmedEmails.Add(email);
                }
            }

            return splittedEmails;
        }

       
    }
}
