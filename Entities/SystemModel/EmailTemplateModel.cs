using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities.Enum;

namespace Entities.SystemModel
{
    public class EmailTemplateModel : EntityBase<Guid>
    {
        public EmailTemplateModel() : base()
        {
                    
        }
        [Column(TypeName = "nvarchar(max)")]
        public string Template { get; set; }
        public string Subject { get; set; }
        public EmailTypeEnums EmailType { get; set; }

    }
}
