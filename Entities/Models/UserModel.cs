using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace Entities.Models
{
    public class UserModel: IdentityUser
    {
        public UserModel(): base()
        {
            IsDeleted = false;
            IsActive = true;
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }


    }
}
