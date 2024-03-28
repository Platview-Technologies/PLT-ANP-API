﻿using Contract;
using Entities.Models;
using Entities.SystemModel;
using MailKit.Security;
using MimeKit;
using Service.Contract;
using Utilities.Constants;
using Utilities.Enum;
using Utilities.Utilities;

namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly IRepositoryManager _repository;
        private IEmailTemplateService _emailTemplate;
        public NotificationService(IRepositoryManager repository, IEmailTemplateService emailTemplate)
        {
            _repository = repository;
            _emailTemplate = emailTemplate;

        }
        public Task CreateNotification(string message, string subject, string email)
        {
            throw new NotImplementedException();
        }

        public async Task CreateNotification(string email, DealsModel deal, EmailTypeEnums type)
        {
            NotificationModel _ = new NotificationModel()
            {
                Emailaddresses = deal.ContactEmail,
                UpdatedDate = DateTime.Now,
                Status = MessageStatusEnums.Pending,
                EmailType = type,
                DealId = deal.Id
            };

            _repository.Notification.CreateNotification(_);
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<NotificationModel>> GetPendingNotifications(int page, int pageSize)
        {
            return await _repository.Notification.GetPendingNotifications(true, page, pageSize);
        }

        public async Task SendNotification(NotificationModel pendingNotifications, SMTPSettings sMTP)
        {
            try
            {
                if (sMTP == null)
                {
                    pendingNotifications.Status = MessageStatusEnums.Failed;
                    pendingNotifications.FailedDate = DateTime.Now;
                    pendingNotifications.ResponseMessage = "SMTP not configured yet";
                    return;
                }

                List<string> confirmedEmails = new List<string>();
                List<string> unConfirmedEmails = new List<string>();
                string[] splittedEmails = await SortEmails(pendingNotifications, confirmedEmails, unConfirmedEmails);
                if (confirmedEmails.Count() == 0)
                {
                    pendingNotifications.Status = MessageStatusEnums.Failed;
                    pendingNotifications.FailedDate = DateTime.Now;
                    pendingNotifications.ResponseMessage = "Email(s) does not exist or their respective domain(s) has expired";

                    return;
                }

                EmailContent content = await GetNotificationContent(pendingNotifications);
                var mail = new MimeMessage();
                mail.From.Add(MailboxAddress.Parse(sMTP.FromEmail));
                mail.To.Add(MailboxAddress.Parse(string.Join(",", confirmedEmails)));
                mail.Subject = content.Subject;
                mail.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = content.Message
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
                

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    //client.EnableSsl = sMTP.SSLStatus;
                    //client.Host = sMTP.HostServer;
                    //client.Port = sMTP.HostPort;
                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //client.UseDefaultCredentials = true;
                    //client.Credentials = new NetworkCredential(sMTP.FromEmail, sMTP.FromEmailPassword);
                    client.Connect(sMTP.HostServer, sMTP.HostPort, SecureSocketOptions.StartTls);
                    client.Authenticate(sMTP.FromEmail, sMTP.FromEmailPassword);
                    var sentConfirmation = await client.SendAsync(mail);

                    if (confirmedEmails.Count() == splittedEmails.Count())
                    {
                        pendingNotifications.Status = MessageStatusEnums.Sent;
                        pendingNotifications.Sentdate = DateTime.Now;
                        pendingNotifications.ResponseMessage = sentConfirmation;
                    }
                    else
                    {
                        pendingNotifications.Status = MessageStatusEnums.SentPartially;
                        pendingNotifications.ResponseMessage = "Unable to send to these emails: " + string.Join(",", unConfirmedEmails) + ". Either Email(s) does not exist or their respective domain(s) has expired";
                        pendingNotifications.Sentdate = DateTime.Now;
                        pendingNotifications.FailedDate = DateTime.Now;
                    }
                }



            }
            catch (Exception ex)
            {

                pendingNotifications.Status = MessageStatusEnums.Failed;
                pendingNotifications.FailedDate = DateTime.Now;
                pendingNotifications.ResponseMessage = ex.Message;

            }
      
        }
        private static async Task<string[]> SortEmails(NotificationModel pendingNotification, List<string> confirmedEmails, List<string> unConfirmedEmails)
        {
            string[] splittedEmails = pendingNotification.Emailaddresses.Split(",");
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

        private async Task<EmailContent> GetNotificationContent(NotificationModel pendingNotification)
        {
            EmailContent content = new EmailContent();

            if (pendingNotification.EmailType.HasValue)
            {
                content = await ProcessMessage(pendingNotification);
            }
            else
            {
                content = new EmailContent()
                {
                    Subject = pendingNotification.Subject,
                    Message = pendingNotification.Message
                };
            }
            content.Message = await AppendHeaderAndFooterToEmail(content.Message);
            return content;
        }

        private async Task<EmailContent> ProcessMessage(NotificationModel notification)
        {
            EmailContent subjectAndMessage = new EmailContent();

            if (notification.EmailType.HasValue)
            {
                switch (notification.EmailType.Value)
                {
                    case EmailTypeEnums.NewDeal:
                        subjectAndMessage = await GetUserSubjectAndMessage(notification);
                        break;
                    case EmailTypeEnums.Notification:
                        subjectAndMessage = await GetUserSubjectAndMessage(notification);
                        break;
                    case EmailTypeEnums.Reminder:
                        subjectAndMessage = await GetUserSubjectAndMessage(notification);
                        break;
                    default:
                        break;
                }
            }
            return subjectAndMessage;
        }
        private async Task<EmailContent> GetUserSubjectAndMessage(NotificationModel notification)
        {
            var deals = await _repository.Deal.GetDeal(notification.DealId, false);

            if (deals == null)
            {
                return new EmailContent();
            }

            EmailTemplateModel template = await _emailTemplate.GetEmailTemplate(notification.EmailType.Value, false);

            string message = template.Template
                .Replace(TagName.FirstName, deals.ClientName)
                .Replace(TagName.Solution, deals.Name)
                .Replace(TagName.Date, deals.RenewalDate.Day.ToString() + Helper.GetDaySuffix(deals.RenewalDate))
                .Replace(TagName.Month, deals.RenewalDate.ToString("MMMM"))
                .Replace(TagName.Year, deals.RenewalDate.Year.ToString());

            string subject = template.Subject
                .Replace(TagName.Solution, deals.Name);

            return new EmailContent()
            {
                Message = message,
                Subject = subject
            };
                
        }
        private async Task<string> AppendHeaderAndFooterToEmail(string message)
        {
            EmailTemplateModel headerTemplate = await _emailTemplate.GetEmailTemplate(EmailTypeEnums.EmailHeader, false);
            EmailTemplateModel footerTemplate = await _emailTemplate.GetEmailTemplate(EmailTypeEnums.NotificationFooter, false);

         

            string finalTemplate = WrapMessageWithHTMLTableROW(headerTemplate.Template) +
                WrapMessageWithHTMLTableROW(message) + WrapMessageWithHTMLTableROW(footerTemplate.Template.Replace(TagName.Signature, Environment.GetEnvironmentVariable(Constants.Signature)));

            return AddHTMLTable(finalTemplate);
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
    }
}