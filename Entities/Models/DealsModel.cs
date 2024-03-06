using Entities.SystemModel;
using Utilities.Validation;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class DealsModel : EntityBase<Guid>
    {
        public DealsModel(): base()
        {   
           
        }
        [Required(ErrorMessage = "Deal name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Client name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string ClientName { get; set; }
        [Required(ErrorMessage = "First name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        [Required(ErrorMessage = "Email is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        [EmailValidation(ErrorMessage = "Invalid email address")]
        public string ContactEmail { get; set; }
        [Required(ErrorMessage = "Commencemnt Date for Deal is Required")]
        [DataType(DataType.DateTime)]
        public DateTime CommencementDate { get; set; }
        [Required(ErrorMessage = "Renewal Date for Deal is Required")]
        [DataType(DataType.DateTime)]
        public DateTime RenewalDate { get; set; }
        public ICollection<EmailModel> Emails { get; set; }
    }
}
