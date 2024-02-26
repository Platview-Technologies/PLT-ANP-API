using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Validation
{
    public class EmailValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                // Allow null or empty values (use [Required] attribute if needed)
                return true;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(value.ToString());
                return addr.Address == value.ToString();
            }
            catch
            {
                return false;
            }
        }
    }
}
