using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using MySql.Data;
using GuoJio_DBHelper;
using Sugar.Enties;

namespace GuoJio_DAL
{
    public class PostsProvider
    {
        public List<PostsPics> getPosts(string openId, int from, int count, int orderby, DateTime refreshTime, int currSel, double lati, double longti)
        {
            try
            {
                SqlSugarClient db = SqlSugarInstance.newInstance();
                List<PostsPics> result = new List<PostsPics>();
                tbl_userconfig config = new tbl_userconfig();
                config = db.Queryable<tbl_userconfig>().Where(o => o.userId == openId).First();
                var postsList = new List<PostsPics>();
                List<string> followeds = new List<string>();
                var tempPosts = new List<string>();
                followeds = db.Queryable<tbl_userfollowed>().Where(o => o.userId == openId).Select(o => o.followedUser).ToList();
                if (currSel == 1)
                {
                    //postsList = postsList;
                    tempPosts = db.Queryable<tbl_posts>().Where(o => o.postsMakeDate <= refreshTime).OrderBy(o => o.ifOfficial, OrderByType.Desc).OrderBy(o => o.postsMakeDate, OrderByType.Desc).Select(o => o.postsID).ToPageList(from / count + 1, count);
                    postsList = db.Queryable<tbl_posts, tbl_postspics, tbl_user>((po, pp, ur) => new object[] {
                                        JoinType.Left,po.postsID==pp.postsID,
                                        JoinType.Left,po.postsMaker==ur.openid})
                                        .Where((po, pp, ur) => pp.picIndex == 0 && (po.postsStatus != 1 || po.ifOfficial == 1) && (po.ifLY != 1 || !SqlFunc.HasValue(po.ifLY)) && tempPosts.Contains(po.postsID))
                                        .OrderBy((po, pp, ur) => po.ifOfficial, OrderByType.Desc)
                                        .OrderBy((po, pp, ur) => po.postsMakeDate, OrderByType.Desc)
                                                .Select((po, pp, ur) => new PostsPics
                                                {
                                                    postsID = po.postsID,
                                                    postsContent = SqlFunc.IIF(SqlFunc.Length(po.postsContent) >= 20, SqlFunc.Substring(po.postsContent, 0, 20), po.postsContent),
                                                    postsPics = pp.picPath,
                                                    postsMaker = ur.nickName,
                                                    postsLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.loveStatus == 1).Count(),
                                                    //postsReaded = SqlFunc.Subqueryable<tbl_userviewed>().Where(tv => tv.postsID == po.postsID).Count(),
                                                    postsStatus = po.postsStatus,
                                                    postsMakeDate = po.postsMakeDate,
                                                    postsPicCount = po.postsPicCount,
                                                    postsReported = po.postsReported,
                                                    postsCollected = po.postsCollected,
                                                    picSimpPath = pp.picSimpPath,
                                                    openId = ur.openid,
                                                    picIndex = pp.picIndex,
                                                    postsType = po.postsType,
                                                    ifOfficial = po.ifOfficial,
                                                    picsRate = pp.picsRate,
                                                    makerPhoto = ur.avantarUrl,
                                                    ifLY = po.ifLY,
                                                    ifUserLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.userID == openId && tl.loveStatus == 1).Count()

                                                }).ToList();
                }
                else if (currSel == 3)
                {
                    //postsList = postsList.Where(po => (po.postsStatus != 1 || po.ifOfficial == 1)).Where(po => po.ifLY != 1 || !SqlFunc.HasValue(po.ifLY)).OrderBy(po => po.ifOfficial, OrderByType.Desc)
                    //.OrderBy(po => SqlFunc.Subqueryable<tbl_userloved>().Where(o => o.postsID == po.postsID && o.lovedTime >= DateTime.Now.AddDays(-3)).Count(), OrderByType.Desc);
                    tempPosts = db.Queryable<tbl_userloved>().Where(o => o.lovedTime >= DateTime.Now.AddDays(-3) && o.lovedTime <= refreshTime).GroupBy(o => o.postsID).Select(o => o.postsID).ToList();
                    tempPosts = db.Queryable<tbl_posts, tbl_userloved>((tp, tul) => new object[] {
                        JoinType.Left,tp.postsID==tul.postsID
                    }).Select((tp, tul) => new PostsPics
                    {
                        postsID = tp.postsID,
                        postsLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tu => tu.postsID == tp.postsID && tu.lovedTime >= DateTime.Now.AddDays(-3)).Count()
                    }).Where(tp => tempPosts.Contains(tp.postsID) && tp.postsMakeDate <= refreshTime).OrderBy("postsLoved desc").Select(tp => tp.postsID).ToPageList(from / count + 1, count);
                    postsList = db.Queryable<tbl_posts, tbl_postspics, tbl_user>((po, pp, ur) => new object[] {
                                        JoinType.Left,po.postsID==pp.postsID,
                                        JoinType.Left,po.postsMaker==ur.openid})
                                        .Where((po, pp, ur) => pp.picIndex == 0 && (po.postsStatus != 1 || po.ifOfficial == 1) && po.ifLY != 1 || !SqlFunc.HasValue(po.ifLY) && tempPosts.Contains(po.postsID))
                                        .OrderBy((po, pp, ur) => po.ifOfficial, OrderByType.Desc)
                                        .OrderBy((po, pp, ur) => SqlFunc.Subqueryable<tbl_userloved>().Where(o => o.postsID == po.postsID && o.lovedTime >= DateTime.Now.AddDays(-3)).Count(), OrderByType.Desc)
                                                .Select((po, pp, ur) => new PostsPics
                                                {
                                                    postsID = po.postsID,
                                                    postsContent = SqlFunc.IIF(SqlFunc.Length(po.postsContent) >= 20, SqlFunc.Substring(po.postsContent, 0, 20), po.postsContent),
                                                    postsPics = pp.picPath,
                                                    postsMaker = ur.nickName,
                                                    postsLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.loveStatus == 1).Count(),
                                                    //postsReaded = SqlFunc.Subqueryable<tbl_userviewed>().Where(tv => tv.postsID == po.postsID).Count(),
                                                    postsStatus = po.postsStatus,
                                                    postsMakeDate = po.postsMakeDate,
                                                    postsPicCount = po.postsPicCount,
                                                    postsReported = po.postsReported,
                                                    postsCollected = po.postsCollected,
                                                    picSimpPath = pp.picSimpPath,
                                                    openId = ur.openid,
                                                    picIndex = pp.picIndex,
                                                    postsType = po.postsType,
                                                    ifOfficial = po.ifOfficial,
                                                    picsRate = pp.picsRate,
                                                    makerPhoto = ur.avantarUrl,
                                                    ifLY = po.ifLY,
                                                    ifUserLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.userID == openId && tl.loveStatus == 1).Count()

                                                })
                                                .ToList();
                }
                else if (currSel == 2)
                {
                    //postsList = postsList.Where(po => po.postsStatus != 1).Where(po => po.ifLY != 1 || !SqlFunc.HasValue(po.ifLY)).Where(ur => followeds.Contains(ur.openId)).OrderBy(po => po.ifOfficial, OrderByType.Desc).OrderBy(po=>po.postsMakeDate,OrderByType.Desc);
                    tempPosts = db.Queryable<tbl_posts>().Where(o => followeds.Contains(o.postsMaker) && o.postsMakeDate <= refreshTime).OrderBy(po => po.ifOfficial, OrderByType.Desc).OrderBy(po => po.postsMakeDate, OrderByType.Desc).Select(po => po.postsID).ToPageList(from / count + 1, count);
                    postsList = db.Queryable<tbl_posts, tbl_postspics, tbl_user>((po, pp, ur) => new object[] {
                                        JoinType.Left,po.postsID==pp.postsID,
                                        JoinType.Left,po.postsMaker==ur.openid}).Where((po, pp, ur) => pp.picIndex == 0 && (po.postsStatus != 1 || po.ifOfficial == 1) && (po.ifLY != 1 || !SqlFunc.HasValue(po.ifLY)) && tempPosts.Contains(po.postsID))
                                        .OrderBy((po, pp, ur) => po.ifOfficial, OrderByType.Desc)
                                        .OrderBy((po, pp, ur) => po.postsMakeDate, OrderByType.Desc)
                                                .Select((po, pp, ur) => new PostsPics
                                                {
                                                    postsID = po.postsID,
                                                    postsContent = SqlFunc.IIF(SqlFunc.Length(po.postsContent) >= 20, SqlFunc.Substring(po.postsContent, 0, 20), po.postsContent),
                                                    postsPics = pp.picPath,
                                                    postsMaker = ur.nickName,
                                                    postsLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.loveStatus == 1).Count(),
                                                    //postsReaded = SqlFunc.Subqueryable<tbl_userviewed>().Where(tv => tv.postsID == po.postsID).Count(),
                                                    postsStatus = po.postsStatus,
                                                    postsMakeDate = po.postsMakeDate,
                                                    postsPicCount = po.postsPicCount,
                                                    postsReported = po.postsReported,
                                                    postsCollected = po.postsCollected,
                                                    picSimpPath = pp.picSimpPath,
                                                    openId = ur.openid,
                                                    picIndex = pp.picIndex,
                                                    postsType = po.postsType,
                                                    ifOfficial = po.ifOfficial,
                                                    picsRate = pp.picsRate,
                                                    makerPhoto = ur.avantarUrl,
                                                    ifLY = po.ifLY,
                                                    ifUserLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.userID == openId && tl.loveStatus == 1).Count()

                                                })
                                                //.OrderBy(po => SqlFunc.Subqueryable<tbl_userloved>().Where(o => o.postsID == po.postsID && o.lovedTime >= DateTime.Now.AddDays(-3)).Count(), OrderByType.Desc)
                                                .ToList();
                }
                else if (currSel == 4)
                {
                    //postsList = postsList.Where(po => po.postsStatus != 1 && po.ifLY == 1).OrderBy(po => calcDistance(po.latitude, po.longitude, lati, longti));
                    tempPosts = db.Queryable<tbl_posts>().Where(o => o.ifLY == 1).Select(o => o.postsID).ToPageList(from / count + 1, count);
                    postsList = db.Queryable<tbl_posts, tbl_postspics, tbl_user>((po, pp, ur) => new object[] {
                                        JoinType.Left,po.postsID==pp.postsID,
                                        JoinType.Left,po.postsMaker==ur.openid}).Where((po, pp, ur) => tempPosts.Contains(po.postsID) && (po.postsStatus != 1 || po.ifOfficial == 1) && po.ifLY == 1)
                                        .OrderBy((po, pp, ur) => calcDistance(po.latitude, po.longitude, lati, longti))
                                                .Select((po, pp, ur) => new PostsPics
                                                {
                                                    postsID = po.postsID,
                                                    postsContent = SqlFunc.IIF(SqlFunc.Length(po.postsContent) >= 20, SqlFunc.Substring(po.postsContent, 0, 20), po.postsContent),
                                                    postsPics = pp.picPath,
                                                    postsMaker = ur.nickName,
                                                    postsLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.loveStatus == 1).Count(),
                                                    //postsReaded = SqlFunc.Subqueryable<tbl_userviewed>().Where(tv => tv.postsID == po.postsID).Count(),
                                                    postsStatus = po.postsStatus,
                                                    postsMakeDate = po.postsMakeDate,
                                                    postsPicCount = po.postsPicCount,
                                                    postsReported = po.postsReported,
                                                    postsCollected = po.postsCollected,
                                                    picSimpPath = pp.picSimpPath,
                                                    openId = ur.openid,
                                                    picIndex = pp.picIndex,
                                                    postsType = po.postsType,
                                                    ifOfficial = po.ifOfficial,
                                                    picsRate = pp.picsRate,
                                                    makerPhoto = ur.avantarUrl,
                                                    ifLY = po.ifLY,
                                                    ifUserLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.userID == openId && tl.loveStatus == 1).Count()

                                                }).ToList();
                }

                if (config.onlyVerify == 1)
                {
                    postsList = db.Queryable<tbl_posts, tbl_postspics, tbl_user>((po, pp, ur) => new object[] {
                                        JoinType.Left,po.postsID==pp.postsID,
                                        JoinType.Left,po.postsMaker==ur.openid}).Where((po, pp, ur) => pp.picIndex == 0 && po.postsStatus == 1)
                                                .Select((po, pp, ur) => new PostsPics
                                                {
                                                    postsID = po.postsID,
                                                    postsContent = SqlFunc.IIF(SqlFunc.Length(po.postsContent) >= 20, SqlFunc.Substring(po.postsContent, 0, 20), po.postsContent),
                                                    postsPics = pp.picPath,
                                                    postsMaker = ur.nickName,
                                                    postsLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.loveStatus == 1).Count(),
                                                    //postsReaded = SqlFunc.Subqueryable<tbl_userviewed>().Where(tv => tv.postsID == po.postsID).Count(),
                                                    postsStatus = po.postsStatus,
                                                    postsMakeDate = po.postsMakeDate,
                                                    postsPicCount = po.postsPicCount,
                                                    postsReported = po.postsReported,
                                                    postsCollected = po.postsCollected,
                                                    picSimpPath = pp.picSimpPath,
                                                    openId = ur.openid,
                                                    picIndex = pp.picIndex,
                                                    postsType = po.postsType,
                                                    ifOfficial = po.ifOfficial,
                                                    picsRate = pp.picsRate,
                                                    makerPhoto = ur.avantarUrl,
                                                    ifLY = po.ifLY,
                                                    ifUserLoved = SqlFunc.Subqueryable<tbl_userloved>().Where(tl => tl.postsID == po.postsID && tl.userID == openId && tl.loveStatus == 1).Count()

                                                }).ToList();
                }

                //postsList = postsList.Where(po => po.postsMakeDate <= refreshTime);
                return postsList;
            }
            catch (Exception e)
            {
                return new List<PostsPics>();
            }
        }

        public int calcDistance(double pla, double plo, double ula, double ulo)
        {
            return Convert.ToInt32(Math.Floor(Math.Sqrt(Math.Pow(pla - ula, 2) + Math.Pow(plo - ulo, 2))));
        }
    }
}
