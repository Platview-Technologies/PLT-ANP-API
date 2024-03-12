using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Constants;

namespace Entities.Exceptions
{
    public class ExpiredToken : UnAuthorizedException
    {
        public ExpiredToken(): base(string.Format(ErrorMessage.InvalidObject, Constants.Token))
        {
                
        }
    }
}
