using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enum;

namespace Shared.DTOs
{
    public class NotificationResponseDto
    {
        public Guid Id { get; init; }
        public EmailTypeEnums? EmailType { get; init; }
        public MessageStatusEnums Status { get; init; }
        public DateTime? Sentdate { get; init; }
        public DateTime? FailedDate { get; init; }
        public ICollection<string> EmailAddresses { get; init; }
        public ICollection<string>? CCEmails { get; set; } 
        public string? ResponseMessage { get; init; }
        public Guid DealId { get; init; }
    }
}
