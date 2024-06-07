using System;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models;

namespace Entities.SystemModel
{
    public class LoginSessions : EntityBase<Guid>
    {
        public LoginSessions()
        {
            DeviceId = Guid.NewGuid();  // Generate new GUID for DeviceId
        }
        // Foreign keys to reference ApplicationUserToken
        public string UserId { get; set; }
        
        public string RefreshToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceType { get; set; }
        public UserModel User { get; set; }
        public bool CheckExpiry()
        {
            return ExpirationDate <= DateTime.UtcNow;
        }
    }
}
