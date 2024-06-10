using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Constants;

namespace Shared.DTOs.Response
{
    public record RenewalResponseDto: RenewalDto
    {
        public Guid Id { get; init; }
        [Required(ErrorMessage = "Deal is a required field.")]
        public DealResponseDto Deal { get; set; }
        [Required(ErrorMessage = "Previous Commencemnt Date for Deal is Required")]
        [DataType(DataType.DateTime)]
        public DateTime PrevCommencementDate { get; set; }
        [Required(ErrorMessage = "Previous Expiry Date for Deal is Required")]
        [DataType(DataType.DateTime)]
        public DateTime PrevExpiryDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ExpectedRenewalDate { get; set; }
    }
}
