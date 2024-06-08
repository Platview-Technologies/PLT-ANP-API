using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class RenewalsModel: EntityBase<Guid>
    {
        public DealsModel Deal { get; set; }
        public Guid DealId { get; set; }
        public decimal Value { get; set; }
        

    }
}
