using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class RenewalsModel : EntityBase<Guid>
    {
        [Required(ErrorMessage = "Deal is a required field.")]
        public DealsModel Deal { get; set; }

        [Required(ErrorMessage = "Deal ID is a required field.")]
        public Guid DealId { get; set; }

        [Required(ErrorMessage = "Value is a required field.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Value must be a positive number.")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Date renewed is a required field.")]
        [DataType(DataType.DateTime)]
        public DateTime DateRenewed { get; set; }
        [Required(ErrorMessage = "Term must be specified")]
        public int Term { get; set; }
        [Required(ErrorMessage = "Previous Commencemnt Date for Deal is Required")]
        [DataType(DataType.DateTime)]
        public DateTime PrevCommencementDate { get; set; }
        [Required(ErrorMessage = "Previous Expiry Date for Deal is Required")]
        [DataType(DataType.DateTime)]
        public DateTime PrevExpiryDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime ExpectedRenewalDate { get; set; }
        [Required(ErrorMessage = "Value Before Renewal is a required field.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Value Before Renewal must be a positive number.")]
        public decimal ValueBeforeRenewal { get; set; }
    }
}
