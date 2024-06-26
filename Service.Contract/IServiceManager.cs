﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contract
{
    public interface IServiceManager
    {
        IDealService DealService { get; }
        IAuthenticationService AuthenticationService { get; }
        IUserManagementService UserManagementService { get; }
    }
}
