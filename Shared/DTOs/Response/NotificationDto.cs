using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public class NotificationDto: NotificationResponseDto
    {
        public string Message { get; set; }
        public string Subject { get; set; }
    }
}
