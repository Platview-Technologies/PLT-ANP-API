using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class InvalidLicenseException: UnAuthorizedException
    {
        public InvalidLicenseException(string message): base(message)
        {
        }
    }
}
