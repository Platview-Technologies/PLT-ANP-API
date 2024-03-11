using Entities.Interfaces;
using Entities.SystemModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Validation;

namespace Entities.Models
{
    public class TempUserModel : EntityBase<Guid>
    {
        public TempUserModel() : base() { }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailValidation(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        public bool? IsActive { get; set; } = false;
        public string? UserId { get; set; }
        public UserModel? UserModel {get; set;}
        public ICollection<EmailModel> Emails { get; set; }
    }
}
