﻿using Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities.Enum;

namespace Entities.SytstemModel
{
    public class EmailModel : EntityBase<Guid>
    {
        public EmailModel() : base()
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
        public string? ChangeOrResetUserId { get; set; }
        public string? NewUserActivationToken { get; set; }
        public string? UserId { get; set; }
        public UserModel? Owner { get; set; }
        public DealsModel? Deals { get; set; }
    }
}