using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Required(ErrorMessage = "Email is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string ContactEmail { get; init; }
        [Required(ErrorMessage = "Commencemnt Date for Deal is Required")]
        [DataType(DataType.DateTime)]
        public DateTime CommencementDate { get; init; }
        [Required(ErrorMessage = "Renewal Date for Deal is Required")]
        [DataType(DataType.DateTime)]
        public DateTime RenewalDate { get; init; }
    }
}
