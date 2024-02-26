using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Constants
{
    public static class Constants
    {
        public const char Dot = '.';
        public const string Empty = "";
        public const string And = "and";
        public const string To = "to";
        public const string Comma = ",";
        public const string CommaSpace = ", ";
        public const string Space = " ";
        public const string Underscore = "_";
        public const string BackSlash = "/";
        public const string ForwardSlash = "\\";
        public const string NumberTo2DecimalPlaces = "N";
        public const string MoneyFormat = "0,0.00";
        public const string DateFormatForIdGeneration = "yyyyMMdd";
        public const string DateFormatMMMddyyyy = "MMMddyyyy";
        public const string DateFormatMMMddyyyyhhmm = "MMMddyyyyhhmm";
        public const string DisplayDateFormatMMMddyyyy = "MMM dd, yyyy";
        public const string DisplayMMMyyyy = "MMM, yyyy";
        public const string Displayyyyy = "yyyy";
        public const string NewLine = "\n";


        public const string Account = "Account";
        public const string Token = "Token";
        public const string Secret = "SECRET";
        public const string Password = "Password";
        public const string ConfirmPassword = "ConfirmPassword";
        public const string Email = "Email";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string WorkExperience = "Work Experience";
        public const string Technology = "Technology";
        public const string Project = "Project";
        public const string Name = "Name";
        public const string GithubLink = "Github Link";
        public const string User = "User";
        public const string EntrustGroup = "Torokobo";
        public const string CertName = "identityguard";
        public const string CertNameTomcat = "tomcat";
        public const string CertWin = "WIN-GEVJDLCEEM2";
        public const string ClientCertificate = "EntrustClientCert";
        public const string ServiceCertificate = "Servicecertificate";
        public const string entrustcertificate = "entrustcertificate";




        public const string OperatonNotAllowed = "This operation is not allowed. Thank you";
        public const string InvalidInformation = "Invalid {0} Information";
        public const string RecordAlreadyExist = "You have carry out this operation already. Thank you";
        public const string PasswordRequirement = "Your password must contain at least a special character, at least a capital letter, at least a number and must be at least 8 character in length. Thank you";
        public const string InvalidSubject = "Invalid {0}";
        public const string RequiredSubject = "{0} is required!";
        public const string RequestValidSubject = "Please enter valid {0}!";
        public const string SubjectDoesNotMatch = "{0} does not match!";
        public const string NotFoundSentence = "the {0} with id : {1} dosent't exist";
        public const string ResouceWithProperyExist = "{0} with specified {1} already exists";
        public const string AccountActivationErrorMessage = "Account activation unsuccessfull";
        public const string AccountActivationMessage = "Account activation successfull";
        public const string NewAccountMessage = "Activation mail sent please activate account before next login";

        public const string ToPersonanized = "To Personanized this template, Use the following \n ";
        public const string WebRootPath = "WebRootPath";
        public const string InvalidCredentials = "Invalid Credentials";
        public const string UserNotActivated = "This user is not activated please activate";
        public const string NotEntrustUser = "This user MFA hasnt been activated please activate";
        public const string EntrustGeneralError = "Something went wrong with entrust";
        public const string EntrustConnectionError = "Unable to connect to Entrust";
        public const string EntrustBlockedUser = "This User is blocked by entrust";
        public const string EntrustWrongGroup = "User does not not belong to this application";
    }
}
