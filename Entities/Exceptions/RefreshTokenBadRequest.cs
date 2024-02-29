using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Constants;

namespace Entities.Exceptions
{
    public class RefreshTokenBadRequest : UnAuthorizedException
    {
        public RefreshTokenBadRequest() : base(string.Format(Constants.InvalidInformation, Constants.Token ))
        {

        }
    }
}
