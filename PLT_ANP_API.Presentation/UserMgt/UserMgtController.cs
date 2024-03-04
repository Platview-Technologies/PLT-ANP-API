using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLT_ANP_API.Presentation.UserMgt
{
    [ApiController]
    [Route("api/usermgt")]
    public class UserMgtController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UserMgtController(IServiceManager service)
        {
            _service = service;
        }

        public static Task<IActionResult> AddNewUser([FromBody] string email)
        {

        }
    }
}
