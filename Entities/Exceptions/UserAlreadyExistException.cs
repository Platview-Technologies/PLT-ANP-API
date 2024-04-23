using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Constants;

namespace Entities.Exceptions
{
    public class UserAlreadyExistException: BadRequestException
    {
        public UserAlreadyExistException():  base(string.Format(ErrorMessage.ObjectAlreadyExist, Constants.User))
        {
            
        }
    }
}
