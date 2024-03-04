using Contract;
using Entities.SystemModel;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Response;
using Utilities.Constants;
using Utilities.Enum;

namespace Repository
{
    public class EmailRepository : RepositoryBase<EmailModel, Guid>, IEmailRepository 
    {
        public EmailRepository(RepositoryContext context) : base(context)
        {

        }

        public void CreateEmailLog(EmailModel email)
        {
            Create(email);
        }

        public async Task<IEnumerable<EmailModel>> GetAllPendingEmails(bool trackChanges, int page, int pageSize)
        {
            return await FindByCondition(x => x.Status == MessageStatusEnums.Pending, trackChanges)
                .OrderBy(r => r.UpdatedDate)
                                     .Skip(page)
                                    .Take((page + 1) * pageSize)
                                     .ToListAsync();
        }
        public void GetEmailUser(EmailModel email)
        {
            var user = FindByCondition(x => x.Id == email.Id, false).Select(x => new EmailUserResponseDto()
            {
                Email = x.Owner.Email,
                FirstName = x.Owner.FirstName,
                LastName = x.Owner.LastName,
                EmailConfirmed = x.Owner.EmailConfirmed,
                Id = x.Owner.Id,
                NewUserActivationToken = x.Owner.NewUserActivationToken,
                PhoneNumber = x.Owner.PhoneNumber
            });
        }

    }
}
