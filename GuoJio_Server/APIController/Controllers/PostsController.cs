using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatsPrj.Model;
using GuoJio_BLL;
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

        public JsonResult getPostsDetail(string postsId, string userId, int from, int count, DateTime refreshTime, string openId)
        {
            PostsHandler handler = new PostsHandler();
            handler.viewPosts(openId, postsId);
            PostsModel result = new PostsModel();
            bool ifLoved = false;
            List<RepliesModel> replies = new List<RepliesModel>();
            bool ifFollowed = false;
            string lovedTimes = "0";
            string readCount = string.Empty;
            bool ifLegal = handler.ifLegalPosts(postsId);
            List<RepliesModel> myReplies = new List<RepliesModel>();
            string repliesCount = string.Empty;
            string shareCount = string.Empty;

            result = handler.getPostsDetail(postsId);
            ifLoved = handler.ifUserLoved(postsId, userId);
            repliesCount = handler.getRepliesCount(postsId);
            lovedTimes = handler.postsLoved(postsId);
            myReplies = handler.getMyReplies(postsId, from, count, refreshTime, openId);
            //replies = handler.getReplies(postsId, from, count, refreshTime, openId);
            ifFollowed = new UserHandler().ifFollowed(userId, postsId);
            readCount = handler.getReadCount(postsId);
            ifLegal = ifLegal && result.postsStatus != 1;
            shareCount = handler.getShareCount(postsId);
            bool ifMuted = new UserHandler().getConfigModel(openId).videoMuted == 1 ? true : false;
            return Json(new { result = result, ifLoved = ifLoved, lovedTimes = lovedTimes, repliesCount = repliesCount, shareCount = shareCount, replies = replies, ifFollowed = ifFollowed, readCount = readCount, ifLegal = ifLegal, myReply = myReplies, ifMuted = ifMuted });
        }

    }
}