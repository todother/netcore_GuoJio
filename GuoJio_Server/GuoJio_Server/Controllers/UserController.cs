using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuoJio_BLL;
using GuoJio_Model;
using Microsoft.AspNetCore.Mvc;

namespace GuoJio_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> getUsers()
        {
            //List<UserModel> users = new UserHandler().getUsers();
            //return users;
            return "OK";
        }
    }
}