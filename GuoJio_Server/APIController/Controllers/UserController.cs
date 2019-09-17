using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatsPrj.Model;
using GuoJio_BLL;
using Microsoft.AspNetCore.Mvc;

namespace APIController.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult getUsers()
        {
            List<UserModel> users = new UserHandler().getUsers();
            return Json(new { result = users });
        }

        public JsonResult userLogin(string userAccount,string pwd)
        {
            return Json(new { result = "OK" });
        }
    }
}