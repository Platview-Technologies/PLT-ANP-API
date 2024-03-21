using System.ComponentModel.DataAnnotations;
using Utilities.Constants;

namespace Utilities.Enum
{
    public enum EmailTypeEnums
    {
        [Display(Name = "Expiration Mail Template", Description = Constants.Constants.ToPersonanized +
            TagName.FirstName +
            Constants.Constants.CommaSpace +
            TagName.Solution +
            Constants.Constants.CommaSpace +
            TagName.Date +
            Constants.Constants.CommaSpace +
            TagName.Month +
            Constants.Constants.CommaSpace +
            TagName.Year)]
        Expiration,
        [Display(Name = "New Deal Template", Description = Constants.Constants.ToPersonanized +
            TagName.FirstName +
            Constants.Constants.CommaSpace +
            TagName.Solution +
            Constants.Constants.CommaSpace +
            TagName.Date +
            Constants.Constants.CommaSpace +
            TagName.Month +
            Constants.Constants.CommaSpace +
            TagName.Year)]
        NewDeal,
        [Display(Name = "Notification Template", Description = Constants.Constants.ToPersonanized +
            TagName.FirstName +
            Constants.Constants.CommaSpace +
            TagName.Solution +
            Constants.Constants.CommaSpace +
            TagName.Date +
            Constants.Constants.CommaSpace +
            TagName.Month +
            Constants.Constants.CommaSpace +
            TagName.Year)]
        Notification,
        [Display(Name = "Reminder Template", Description = Constants.Constants.ToPersonanized +
            TagName.FirstName +
            Constants.Constants.CommaSpace +
            TagName.Solution +
            Constants.Constants.CommaSpace +
            TagName.Date +
            Constants.Constants.CommaSpace +
            TagName.Month +
            Constants.Constants.CommaSpace +
            TagName.Year)]
        Reminder,
        [Display(Name = "New Account Template", Description = Constants.Constants.ToPersonanized +
            TagName.FirstName +
            Constants.Constants.CommaSpace +
            TagName.Surname +
            Constants.Constants.CommaSpace +
            TagName.NewPassword +
            Constants.Constants.CommaSpace +
            TagName.EmailAddress +
            Constants.Constants.CommaSpace +
            TagName.PhoneNumber)]
        NewAccount,
        [Display(Name = "Reset Password Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.NewPassword + Constants.Constants.CommaSpace + TagName.EmailAddress)]
        ResetPassword,
        [Display(Name = "Change Password Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.NewPassword + Constants.Constants.CommaSpace + TagName.EmailAddress)]
        ChangePassword,
        [Display(Name = "Email Header", Description = Constants.Constants.ToPersonanized + TagName.WebsiteLogo + Constants.Constants.CommaSpace + TagName.WebsiteName)]
        EmailHeader,
        [Display(Name = "Email Footer", Description = Constants.Constants.ToPersonanized + TagName.WebsiteLogo + Constants.Constants.CommaSpace + TagName.WebsiteName)]
        EmailFooter,
        [Display(Name = "Account Activation Template", Description = Constants.Constants.ToPersonanized +
           
            TagName.FirstName +
            Constants.Constants.CommaSpace +
             TagName.Surname +
            Constants.Constants.CommaSpace +
            TagName.ActivationToken +
            Constants.Constants.CommaSpace +
            TagName.UserId
            )]
        AccountActivation,
        [Display(Name = "User Registration Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.RegCode + Constants.Constants.CommaSpace + TagName.UserId)]
        UserRegistration,
        [Display(Name = "Notification Footer", Description = Constants.Constants.ToPersonanized + TagName.WebsiteLogo + Constants.Constants.CommaSpace + TagName.WebsiteName)]
        NotificationFooter,
    }
}
