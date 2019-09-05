using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuoJio_BLL;
using GuoJio_Model;
using Microsoft.AspNetCore.Mvc;

namespace APIController.Controllers
{
    public class PostsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult getPosts(string openId, int dataFrom, int count, DateTime refreshTime, int currentSel, double ulo, double ula)
        {
            PostsHandler handler = new PostsHandler();
            List<PostsModel> list = new List<PostsModel>();
            list = handler.getPosts(openId, dataFrom, count, refreshTime, currentSel, ula, ulo);
            return Json(new { result = list });
        }

    }
}