using Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class TempUserModel : EntityBase<Guid>
    {
        public TempUserModel() : base() { }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public bool? IsActive { get; set; } = false;
        public string? UserID { get; set; }
        public UserModel? UserModel {get; set;}
    }
}
