using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enum;

namespace Shared.DTOs.Response
{
    public class NotificationResponseDto
    {
        public Guid Id { get; init; }
        public EmailTypeEnums? EmailType { get; init; }
        public MessageStatusEnums Status { get; init; }
        public DateTime? Sentdate { get; init; }
        public DateTime? FailedDate { get; init; }
        public string Emailaddresses { get; init; }
        public string? ResponseMessage { get; init; }
        public Guid DealId { get; init; }
    }
}
