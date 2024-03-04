using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Enum;

namespace Shared.DTOs.Response
{
    public record EmailTemplateResponseDto
    {
        public Guid Id { get; set; }
        public EmailTypeEnums EmailType { get; set; }
        public string Template { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;

    }
}
