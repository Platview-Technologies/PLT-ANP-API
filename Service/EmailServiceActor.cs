//using Entities.SystemModel;
//using System.Net;
//using Utilities.Constants;
//using Utilities.Enum;
//using Utilities.Utilities;
//using Repository;
//using Microsoft.EntityFrameworkCore;
//using MailKit.Net.Smtp;
//using MailKit.Security;
//using MimeKit;
//using Contract;

//namespace Service
//{
//    public class EmailServiceActor : IRepositoryBase<EmailModel, Guid>, IEmailService
//    {
        

//        public EmailServiceActor(RepositoryContext context) : base(context)
//        {
            
//        }

//        public void CreateEmail(string message, string subject, string email)
//        {
//            Create(new EmailModel()
//            {
//                Emailaddresses = email,
//                UpdatedDate = DateTime.Now,
//                Message = message,
//                Status = MessageStatusEnums.Pending,
//                Subject = subject,
//            });
//        }

//        //public void CreateEmail(string message, string subject, List<string> emails)
//        //{
//        //    foreach (string email in emails)
//        //    {
//        //        CreateEmail(message, subject, email);
//        //    }
//        //}

//        public async Task<EmailContent> GetEmailContent(EmailModel pendingEmail)
//        {
//            EmailContent emailContent = new EmailContent();

//            if (pendingEmail.EmailType.HasValue)
//            {
//                emailContent = await ProcessMessage(pendingEmail);
//            }
//            else
//            {
//                emailContent = new EmailContent()
//                {
//                    Subject = pendingEmail.Subject,
//                    Message = pendingEmail.Message
//                };
//            }
//            emailContent.Message = await AppendHeaderAndFooterToEmail(emailContent.Message);
//            return emailContent;
//        }

//        public async Task<EmailContent> ProcessMessage(EmailModel email)
//        {
//            EmailContent subjectAndMessage = new EmailContent();

//            if (email.EmailType.HasValue)
//            {
//                switch (email.EmailType.Value)
//                {
//                    case EmailTypeEnums.NewAccount:
//                        subjectAndMessage = await GetUserSubjectAndMessage(email);
//                        break;
//                    case EmailTypeEnums.AccountActivation:
//                        subjectAndMessage = await GetUserSubjectAndMessage(email);
//                        break;
//                    case EmailTypeEnums.ResetPassword:
//                        if (!string.IsNullOrEmpty(email.ChangeOrResetUserId))
//                        {
//                            subjectAndMessage = await GetUserSubjectAndMessage(email);
//                        }
//                        break;
//                    case EmailTypeEnums.ChangePassword:
//                        if (!string.IsNullOrEmpty(email.ChangeOrResetUserId))
//                        {
//                            subjectAndMessage = await GetUserSubjectAndMessage(email);
//                        }
//                        break;
//                    default:
//                        break;

//                }
//            }
//            return subjectAndMessage;

//        }

//        public async Task<EmailContent> GetUserSubjectAndMessage(EmailModel email)
//        {
//            var user = await Context.Users
//                .Where(r => r.Id == email.UserId)
//                .Select(r => new
//                {
//                    r.FirstName,
//                    r.LastName,
//                    r.EmailConfirmed,
//                    r.Email,
//                    r.PhoneNumber,
//                    email.NewUserActivationToken,
//                    r.Id
//                })
//                .FirstOrDefaultAsync();

//            if (user == null)
//            {
//                return new EmailContent();
//            }


//            EmailTemplateService emailTemplateService = new EmailTemplateService(Context);
//            EmailTemplateModel template = await emailTemplateService.GetEmailTemplate(email.EmailType.Value, false);

//            string message = template.Template
//                .Replace(TagName.FirstName, user.FirstName)
//                .Replace(TagName.Surname, user.LastName)
//                .Replace(TagName.EmailAddress, user.Email)
//                .Replace(TagName.ActivationToken, string.IsNullOrEmpty(email.NewUserActivationToken) ? "" : email.NewUserActivationToken)
//                .Replace(TagName.UserId, string.IsNullOrEmpty(email.UserId) ? "" : email.UserId);

//            string subject = template.Subject
//                .Replace(TagName.FirstName, user.FirstName)
//                .Replace(TagName.Surname, user.LastName)
//                .Replace(TagName.EmailAddress, user.Email)
//                .Replace(TagName.PhoneNumber, user.PhoneNumber)
//                .Replace(TagName.UserId, user.Id);

//            return new EmailContent()
//            {
//                Message = message,
//                Subject = subject
//            };
//        }

//        public async Task UserEmail(string userId, EmailTypeEnums emailType)
//        {
//            var user = await Context.Users
//               .Where(r => r.Id == userId)
//               .Select(r => new
//               {
//                   r.Email,
//               })
//               .FirstOrDefaultAsync();

//            if (user == null)
//            {
//                return;
//            }


            

//            Create(new EmailModel()
//            {
//                Emailaddresses = user.Email,
//                EmailType = emailType,
//                Status = MessageStatusEnums.Pending,
//                UserId = ((emailType == EmailTypeEnums.NewAccount ||
//                emailType == EmailTypeEnums.AccountActivation) ? (userId) : (null)),
//                ChangeOrResetUserId = ((emailType == EmailTypeEnums.NewAccount) ?
//                (null) : (userId)),
//                Id = Guid.NewGuid()
//            });
//        }

//        private async Task<string> AppendHeaderAndFooterToEmail(string message)
//        {
//            EmailTemplateServiceActor emailTemplateService = new EmailTemplateServiceActor(Context);
//            EmailTemplateModel headerTemplate = await emailTemplateService.GetEmailTemplate(EmailTypeEnums.EmailHeader, false);
//            EmailTemplateModel footerTemplate = await emailTemplateService.GetEmailTemplate(EmailTypeEnums.EmailFooter, false);
//            string finaltemplate = WrapMessageWithHTMLTableROW(headerTemplate.Template) + WrapMessageWithHTMLTableROW(message) + WrapMessageWithHTMLTableROW(footerTemplate.Template);

//            return AddHTMLTable(finaltemplate);

//        }
//        private string WrapMessageWithHTMLTableROW(string message)
//        {
//            string HtmlTable = "";

//            HtmlTable += "<tr>";
//            HtmlTable += "<td rowspan='1' colspan='1' style='padding:20px;" + "'>";
//            HtmlTable += message;
//            HtmlTable += "</td>";
//            HtmlTable += "</tr>";

//            return HtmlTable;
//        }
//        private string AddHTMLTable(string message)
//        {
//            string HtmlTable = "<div align='center'>";
//            HtmlTable += "<table style='background-color:#eeeeee;box-shadow: 0 .5rem 1rem rgba(0,0,0,.15) !important;' bgcolor='#eeeeee' border='0' width='100%' cellspacing='0' cellpadding='0'>";
//            HtmlTable += "<tbody>";
//            HtmlTable += "<tr>";
//            HtmlTable += "<td rowspan='1' colspan='1' align='center' style='line-height:24px;'>";

//            HtmlTable += "<table width='600' style='background-color:#ffffff;'bgcolor='#ffffff'border='0'cellspacing='0' cellpadding='0'>";
//            HtmlTable += "<tbody>";

//            HtmlTable += message;

//            HtmlTable += "</tbody>";
//            HtmlTable += "</table>";

//            HtmlTable += "</td>";
//            HtmlTable += "</tr>";
//            HtmlTable += "</tbody>";
//            HtmlTable += "</table>";
//            HtmlTable += "</div>";

//            return HtmlTable;
//        }

//        public async Task<List<EmailModel>> GetPendingEmails(int page, int pageSize)
//        {
//            var pendingEmails = await FindByCondition(x => x.Status == MessageStatusEnums.Pending, true)
//                .OrderBy(r => r.UpdatedDate)
//                                     .Skip(page)
//                                    .Take((page + 1) * pageSize)
//                                     .ToListAsync();

           

//            return pendingEmails;
//        }

//        public async Task SendEmail(EmailModel pendingEmail, SMTPSettings sMTP)
//        {
//            try
//            {
//                if (sMTP == null)
//                {
//                    pendingEmail.Status = MessageStatusEnums.Failed;
//                    pendingEmail.FailedDate = DateTime.Now;
//                    pendingEmail.ResponseMessage = "SMTP not configure yet";
                   
//                    return;
//                }
//                List<string> confirmedEmails = new List<string>();
//                List<string> unConfirmedEmails = new List<string>();
//                string[] splittedEmails = await SortEmails(pendingEmail, confirmedEmails, unConfirmedEmails);

//                if (confirmedEmails.Count() == 0)
//                {
//                    pendingEmail.Status = MessageStatusEnums.Failed;
//                    pendingEmail.FailedDate = DateTime.Now;
//                    pendingEmail.ResponseMessage = "Email(s) does not exist or their respective domain(s) has expired";
                   
//                    return;
//                }

//                EmailContent emailContent = await GetEmailContent(pendingEmail);

//                //MailMessage mail = new MailMessage()
//                //{

//                //    From = new MailAddress(sMTP.FromEmail),
//                //    IsBodyHtml = true,
//                //    Subject = emailContent.Subject,
//                //    Body = WebUtility.HtmlDecode(emailContent.Message),
//                //};
//                //mail.To.Add(string.Join(",", confirmedEmails));
//                var mail = new MimeMessage();
//                mail.From.Add(MailboxAddress.Parse(sMTP.FromEmail));
//                mail.To.Add(MailboxAddress.Parse(string.Join(",", confirmedEmails)));
//                mail.Subject = emailContent.Subject;
//                mail.Body = new TextPart(MimeKit.Text.TextFormat.Html)
//                {
//                    Text = emailContent.Message
//                };


//                using (var client = new MailKit.Net.Smtp.SmtpClient())
//                {
//                    //client.EnableSsl = sMTP.SSLStatus;
//                    //client.Host = sMTP.HostServer;
//                    //client.Port = sMTP.HostPort;
//                    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
//                    //client.UseDefaultCredentials = true;
//                    //client.Credentials = new NetworkCredential(sMTP.FromEmail, sMTP.FromEmailPassword);
//                    client.Connect(sMTP.HostServer, sMTP.HostPort, SecureSocketOptions.StartTls);
//                    client.Authenticate(sMTP.FromEmail, sMTP.FromEmailPassword);
//                    var sentConfirmation = await client.SendAsync(mail);
                    
//                    if (confirmedEmails.Count() == splittedEmails.Count())
//                    {
//                        pendingEmail.Status = MessageStatusEnums.Sent;
//                        pendingEmail.Sentdate = DateTime.Now;
//                        pendingEmail.ResponseMessage = sentConfirmation;
//                    }
//                    else
//                    {
//                        pendingEmail.Status = MessageStatusEnums.SentPartially;
//                        pendingEmail.ResponseMessage = "Unable to send to these emails: " + string.Join(",", unConfirmedEmails) + ". Either Email(s) does not exist or their respective domain(s) has expired";
//                        pendingEmail.Sentdate = DateTime.Now;
//                        pendingEmail.FailedDate = DateTime.Now;
//                    }
//                }



//            }
//            catch (Exception ex)
//            {

//                pendingEmail.Status = MessageStatusEnums.Failed;
//                pendingEmail.FailedDate = DateTime.Now;
//                pendingEmail.ResponseMessage = ex.Message;
                
//            }
//        }
//        private static async Task<string[]> SortEmails(EmailModel pendingEmail, List<string> confirmedEmails, List<string> unConfirmedEmails)
//        {
//            string[] splittedEmails = pendingEmail.Emailaddresses.Split(",");
//            DNSCheck dNSCheck = new DNSCheck();
//            foreach (string email in splittedEmails)
//            {
//                if (await dNSCheck.IsEmailValid(email))
//                {
//                    confirmedEmails.Add(email);
//                }
//                else
//                {
//                    unConfirmedEmails.Add(email);
//                }
//            }

//            return splittedEmails;
//        }
//    }
//}
