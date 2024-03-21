using Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities.Enum;

namespace Entities.SystemModel
{
    public class NotificationModel: EntityBase<Guid>
    {
        public NotificationModel() : base()
        {

        }
        [Column(TypeName = "nvarchar(max)")]
        public string? Message { get; set; }
        public EmailTypeEnums? EmailType { get; set; }
        public MessageStatusEnums Status { get; set; }
        public DateTime? Sentdate { get; set; }
        public DateTime? FailedDate { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Emailaddresses { get; set; }

        public string? Subject { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? ResponseMessage { get; set; }

        public Guid DealId { get; set; }
        public DealsModel Deal { get; set; }
    }
}
