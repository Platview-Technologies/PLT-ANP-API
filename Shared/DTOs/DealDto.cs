using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Validation;

namespace Shared.DTOs
{
    public abstract record DealDto
    {
        [Required(ErrorMessage = "Deal name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string Name { get; init; }
        [Required(ErrorMessage = "Client name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string ClientName { get; init; }
        [Required(ErrorMessage = "First name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string ContactFirstName { get; init; }
        public string ContactLastName { get; init; }
        [Required(ErrorMessage = "Emails is a required field.")]
        public ICollection<string> ContactEmail { get; init; }
        public ICollection<string>? CCEmails { get; init; }
        [Required(ErrorMessage = "Commencemnt Date for Deal is Required")]
        [DateTimeNotDefault(ErrorMessage = "Commencemnt Date must be specified.")]
        [DataType(DataType.DateTime)]
        public DateTime CommencementDate { get; init; }
        [Required(ErrorMessage = "Expiry Date for Deal is Required")]
        [DateTimeNotDefault(ErrorMessage = "Expiry Date must be specified.")]
        [DataType(DataType.DateTime)]
        public DateTime ExpiryDate { get; init; }
        [Required(ErrorMessage = "Renewal Date for Deal is Required")]
        [DateTimeNotDefault(ErrorMessage = "Renewal Date must be specified.")]
        [DataType(DataType.DateTime)]
        public DateTime RenewalDate { get; init; }
        public bool IsActive { get; init; }
        public bool Status { get; init; }
    }
}
