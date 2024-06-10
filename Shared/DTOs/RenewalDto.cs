using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Constants;

namespace Shared.DTOs
{
    public abstract record RenewalDto
    {
        
        [Required(ErrorMessage = "Deal ID is a required field.")]
        public Guid DealId { get; set; }

        [Required(ErrorMessage = "Value is a required field.")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Value must be a positive number.")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Date renewed is a required field.")]
        [DateTimeNotDefault(ErrorMessage = "Date renewed is a required field.")]
        [DataType(DataType.DateTime)]
        public DateTime DateRenewed { get; set; }
        [Required(ErrorMessage = "Term must be specified")]
        [DateTimeNotDefault(ErrorMessage = "Term must be specified")]
        public int Term { get; set; }
    }
}
