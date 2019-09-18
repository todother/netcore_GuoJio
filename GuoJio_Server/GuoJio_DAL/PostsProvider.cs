﻿using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
using MySql.Data;
using GuoJio_DBHelper;
using System.Data;
using CatsDataEntity;
using Cats.DataEntiry;
using CatsProj.DataEntiry;

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
                tbl_userConfig config = new tbl_userConfig();
                config = db.Queryable<tbl_userConfig>().Where(o => o.userId == openId).First();
                var postsList = new List<PostsPics>();
                List<string> followeds = new List<string>();
                var tempPosts = new List<string>();
                followeds = db.Queryable<tbl_userFollowed>().Where(o => o.userId == openId).Select(o => o.followedUser).ToList();
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

        public tbl_posts getPost(string postId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_posts result = db.Queryable<tbl_posts>().Where(o => o.postsID == postId).First();
            return result;
        }

        /*public IList<tbl_posts> getPosts(string userId,int from,int count,int orderby=0)//0 means order by readed times,1 means order by createdate
		{
			SqlSugarClient db = SqlSugarInstance.newInstance();
			List<tbl_posts> result = new List<tbl_posts>();
			if (orderby == 0)
			{
				result = db.Queryable<tbl_posts>().Where(o => o.postsMaker == userId)
										   .OrderBy(o=>o.postsReaded,OrderByType.Desc).Take(from + count).Skip(from)
										   .ToList();
			}
			else
			{
				result = db.Queryable<tbl_posts>().Where(o => o.postsMaker == userId)
                                           .OrderBy(o => o.postsMakeDate, OrderByType.Desc).Take(from + count).Skip(from)
                                           .ToList();
			}
			return result;
		}*/

       

        

        public IList<PostsPics> getPostsDetail(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            List<PostsPics> result = new List<PostsPics>();
            result = db.Queryable<tbl_posts, tbl_postspics, tbl_user>((po, pp, ur) => new object[] {
                                        JoinType.Left,po.postsID==pp.postsID,
                                        JoinType.Left,po.postsMaker==ur.openid}).Where((po, pp, ur) => pp.postsID == SqlFunc.Subqueryable<tbl_posts>().Where(o => o.postsID == postsId).Select(o => o.postsID))
                                        .OrderBy((po, pp, ur) => pp.picIndex)
                                            .Select((po, pp, ur) => new PostsPics
                                            {
                                                postsID = po.postsID,
                                                postsContent = po.postsContent,
                                                postsPics = pp.picPath,
                                                postsMaker = ur.nickName,
                                                makerPhoto = ur.avantarUrl,
                                                postsLoved = po.postsLoved,
                                                postsReaded = po.postsReaded,
                                                postsStatus = po.postsStatus,
                                                postsMakeDate = po.postsMakeDate,
                                                postsPicCount = po.postsPicCount,
                                                postsReported = po.postsReported,
                                                postsCollected = po.postsCollected,
                                                picSimpPath = pp.picSimpPath,
                                                openId = ur.openid,
                                                picIndex = pp.picIndex,
                                                latitude = po.latitude,
                                                longitude = po.longitude,
                                                postsLocation = po.postsLocation,
                                                postsType = po.postsType

                                            }).ToList();
            return result;
        }

        public void savePosts(tbl_posts posts)
        {
            try
            {
                SqlSugarClient db = SqlSugarInstance.newInstance();
                db.Insertable<tbl_posts>(posts).ExecuteCommand();
            }
            catch (Exception e)
            {
                addLog(e.Message.ToString());
            }
        }


        public void lovePosts(tbl_userloved entity)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_userloved userloved = db.Queryable<tbl_userloved>().Where(o => o.postsID == entity.postsID && o.userID == entity.userID).First();
            if (userloved == null)
            {

                db.Insertable<tbl_userloved>(entity).ExecuteCommand();
            }
            else if (userloved.loveStatus == 1)
            {
                db.Updateable<tbl_userloved>().UpdateColumns(o => new tbl_userloved { loveStatus = 0 }).Where(o => o.postsID == entity.postsID && o.userID == entity.userID).ExecuteCommand();
            }
            else
            {
                db.Updateable<tbl_userloved>().UpdateColumns(o => new tbl_userloved { loveStatus = 1 }).Where(o => o.postsID == entity.postsID && o.userID == entity.userID).ExecuteCommand();
            }
        }

        public Boolean ifUserLoved(string postsId, string userId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            Boolean result = false;
            tbl_userloved entity = db.Queryable<tbl_userloved>().Where(o => o.postsID == postsId && o.userID == userId).First();
            if (entity != null && entity.loveStatus == 1)
            {
                result = true;

            }
            return result;
        }

        public void viewPosts(tbl_userviewed entity)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Insertable<tbl_userloved>(entity).ExecuteCommand();

        }

        public long getUserViewed(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            long total = db.Queryable<tbl_userviewed>().Where(o => o.postsID == postsId).Count();
            return total;
        }

        public long getUserLoved(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            long total = db.Queryable<tbl_userloved>().Where(o => o.postsID == postsId && o.loveStatus == 1).Count();
            return total;
        }
        public void userReply(tbl_reply entity)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Insertable<tbl_reply>(entity).ExecuteCommand();
        }

        public UserReply getReplyDetail(string replyId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            UserReply userReplies = db.Queryable<tbl_user, tbl_reply, tbl_posts>((tu, tr, tp) => new object[]{
                JoinType.Left,tu.openid==tr.replyMaker,
                JoinType.Left,tr.postsID==tp.postsID
            }).Where((tu, tr, tp) => tr.replyID == replyId)
            .Select((tu, tr, tp) => new UserReply
            {
                replyMaker = tr.replyMaker,
                avantarUrl = tu.avantarUrl,
                nickName = tu.nickName,
                replyId = tr.replyID,
                replyContent = tr.replyContent,
                postsId = tp.postsID,
                replyDate = tr.replyDate,
                lovedCount = SqlFunc.Subqueryable<tbl_userReplyLoved>().Where(o => o.replyId == tr.replyID).Count()
            }).First();
            return userReplies;
        }

        public List<UserReply> getUserReplies(int from, int count, string postsId, DateTime refreshTime, string openId)
        {
            try
            {
                SqlSugarClient db = SqlSugarInstance.newInstance();
                List<UserReply> userReplies = db.Queryable<tbl_user, tbl_reply, tbl_posts>((tu, tr, tp) => new object[]{
                JoinType.Left,tu.openid==tr.replyMaker,
                JoinType.Left,tr.postsID==tp.postsID
            }).Where((tu, tr, tp) => tp.postsID == SqlFunc.Subqueryable<tbl_posts>().Where(o => o.postsID == postsId).Select(o => o.postsID) && tr.replyDate <= refreshTime && tr.replyMaker != openId)
            .OrderBy((tu, tr, tp) => tr.replyDate, OrderByType.Desc)
            .Select((tu, tr, tp) => new UserReply
            {
                replyMaker = tr.replyMaker,
                avantarUrl = tu.avantarUrl,
                nickName = tu.nickName,
                replyId = tr.replyID,
                replyContent = tr.replyContent,
                postsId = tp.postsID,
                replyDate = tr.replyDate,
                lovedCount = SqlFunc.Subqueryable<tbl_userReplyLoved>().Where(o => o.replyId == tr.replyID).Count()
            }).ToPageList(from / count + 1, count);

                var userLovedL = db.Queryable<tbl_posts, tbl_reply, tbl_userReplyLoved>((tp, tr, tl) => new object[]{
                JoinType.Left,tp.postsID==tr.postsID,
                JoinType.Left,tr.replyID==tl.replyId
            }).Where((tp, tr, tl) => tp.postsID == postsId && tl.openId == openId)
            .Select((tp, tr, tl) => new UserLovedReplybyPID
            {
                replyId = tr.replyID,
            }).ToList();
                List<string> userLoved = new List<string>();
                foreach (var item in userLovedL)
                {
                    userLoved.Add(item.replyId);
                }

                List<UserReply> replyList = new List<UserReply>();
                foreach (var item in userReplies)
                {
                    if (userLoved.Contains(item.replyId))
                    {
                        item.replyLoved = true;
                    }
                    else
                    {
                        item.replyLoved = false;
                    }
                    replyList.Add(item);
                }


                return replyList;
            }
            catch (Exception e)
            {
                return new List<UserReply>();
            }
        }

        public List<UserReply> getMyReplies(int from, int count, string postsId, DateTime refreshTime, string openId)
        {
            try
            {
                SqlSugarClient db = SqlSugarInstance.newInstance();
                List<string> admins = db.Queryable<tbl_admin>().Select(o => o.openId).ToList();
                List<UserReply> userReplies = db.Queryable<tbl_user, tbl_reply, tbl_posts>((tu, tr, tp) => new object[]{
                JoinType.Left,tu.openid==tr.replyMaker,
                JoinType.Left,tr.postsID==tp.postsID
            }).Where((tu, tr, tp) => tp.postsID == SqlFunc.Subqueryable<tbl_posts>().Where(o => o.postsID == postsId).Select(o => o.postsID) && tr.replyDate <= refreshTime)
            .OrderBy((tu, tr, tp) => SqlFunc.IIF(admins.Contains(tr.replyMaker), 2, SqlFunc.IIF(tr.replyMaker == openId, 1, 0)), OrderByType.Desc)
            .OrderBy((tu, tr, tp) => tr.replyDate, OrderByType.Asc)
            .Select((tu, tr, tp) => new UserReply
            {
                replyMaker = tr.replyMaker,
                avantarUrl = tu.avantarUrl,
                nickName = tu.nickName,
                replyId = tr.replyID,
                replyContent = tr.replyContent,
                postsId = tp.postsID,
                replyType = SqlFunc.IIF(tr.replyMaker == openId, 1, 0),
                lovedCount = SqlFunc.Subqueryable<tbl_userReplyLoved>().Where(o => o.replyId == tr.replyID).Count(),
                isAdmin = SqlFunc.IIF(admins.Contains(tr.replyMaker), true, false)
            }).ToPageList(from / count + 1, count);

                var userLovedL = db.Queryable<tbl_posts, tbl_reply, tbl_userReplyLoved>((tp, tr, tl) => new object[]{
                JoinType.Left,tp.postsID==tr.postsID,
                JoinType.Left,tr.replyID==tl.replyId
            }).Where((tp, tr, tl) => tp.postsID == postsId && tl.openId == openId)
            .Select((tp, tr, tl) => new UserLovedReplybyPID
            {
                replyId = tr.replyID,
                postsId = tp.postsID,
                openId = tl.openId
            }).ToList();
                List<string> userLoved = new List<string>();
                foreach (var item in userLovedL)
                {
                    userLoved.Add(item.replyId);
                }
                /*string repliesID = string.Empty;
				userReplies.ForEach(o => repliesID=repliesID+"'"+o.replyId+"',");
				repliesID = repliesID.Substring(0, repliesID.Length - 1);

				var lovedCount = db.Ado.GetDataTable(@"select tr.replyId,count(tl.replyId) as lovedCount from tbl_reply tr
                left join tbl_userreplyloved tl on tr.trplyId=tl.replyId
                where tr.replyId in (@condition) group by tl.replyId", new { condition = repliesID });*/

                List<UserReply> replyList = new List<UserReply>();
                foreach (var item in userReplies)
                {
                    if (userLoved.Contains(item.replyId))
                    {
                        item.replyLoved = true;
                    }
                    else
                    {
                        item.replyLoved = false;
                    }
                    replyList.Add(item);
                }


                return replyList;
            }
            catch (Exception e)
            {
                return new List<UserReply>();
            }
        }

        public List<PostsPics> getPostsByMaker(string openId, int from, int count)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            List<PostsPics> result = new List<PostsPics>();
            result = db.Queryable<tbl_posts, tbl_postspics, tbl_user>((po, pp, ur) => new object[] {
                                        JoinType.Left,po.postsID==pp.postsID,
                                        JoinType.Left,po.postsMaker==ur.openid}).Where((po, pp, ur) => ur.openid == SqlFunc.Subqueryable<tbl_user>().Where(o => o.openid == openId).Select(o => o.openid))
                                        .OrderBy((po, pp, ur) => po.postsMakeDate, OrderByType.Desc)
                                        .OrderBy((po, pp, ur) => pp.picIndex, OrderByType.Asc)
                                            .Select((po, pp, ur) => new PostsPics
                                            {
                                                postsID = po.postsID,
                                                postsContent = po.postsContent,
                                                postsPics = pp.picPath,
                                                postsMaker = ur.nickName,
                                                postsLoved = po.postsLoved,
                                                postsReaded = po.postsReaded,
                                                postsStatus = po.postsStatus,
                                                postsMakeDate = po.postsMakeDate,
                                                postsPicCount = po.postsPicCount,
                                                postsReported = po.postsReported,
                                                postsCollected = po.postsCollected,
                                                picIndex = pp.picIndex,
                                                picSimpPath = pp.picSimpPath,
                                                openId = ur.openid,
                                                postsType = po.postsType

                                            }).ToPageList(from / count + 1, count);

            return result;
        }

        public List<tbl_postspics> delPosts(string postsId, int delType, string openId)
        {
            try
            {
                SqlSugarClient db = SqlSugarInstance.newInstance();
                List<tbl_postspics> postspics = new List<tbl_postspics>();
                tbl_posts posts = new tbl_posts();
                posts = db.Queryable<tbl_posts>().Where(o => o.postsID == postsId).First();

                db.Insertable<tbl_delReason>(new tbl_delReason
                {
                    delId = Guid.NewGuid().ToString(),
                    delContent = posts.postsContent,
                    delTime = DateTime.Now,
                    delType = delType == 1 ? DelType.ReplyDel : DelType.SeriousDel,
                    delUser = openId,
                    delOpenId = posts.postsMaker
                }).ExecuteCommand();

                if (delType == 2)
                {
                    tbl_user user = new tbl_user();
                    user = db.Queryable<tbl_user>().Where(o => o.openid == posts.postsMaker).First();
                    db.Updateable<tbl_user>().UpdateColumns(o => new tbl_user { userStatus = 1 }).Where(o => o.openid == user.openid).ExecuteCommand();
                    List<string> postsList = db.Queryable<tbl_posts>().Where(o => o.postsMaker == posts.postsMaker).Select(o => o.postsID).ToList();
                    db.Deleteable<tbl_postspics>().Where(o => postsList.Contains(o.postsID)).ExecuteCommand();
                    db.Deleteable<tbl_reply>().Where(o => o.replyMaker == posts.postsMaker).ExecuteCommand();
                    db.Deleteable<tbl_reply>().Where(o => postsList.Contains(o.postsID)).ExecuteCommand();
                    db.Deleteable<tbl_replyAfterReply>().Where(o => o.replyMaker == posts.postsMaker).ExecuteCommand();
                }

                postspics = db.Queryable<tbl_postspics>().Where(o => o.postsID == postsId).ToList();
                db.Deleteable<tbl_posts>().Where(o => o.postsID == postsId).ExecuteCommand();
                db.Deleteable<tbl_postspics>().Where(o => o.postsID == postsId).ExecuteCommand();
                db.Deleteable<tbl_userloved>().Where(o => o.postsID == postsId).ExecuteCommand();
                return postspics;
            }
            catch (Exception e)
            {
                return new List<tbl_postspics>();
            }
        }

        public void userViewPosts(string openId, string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Insertable<tbl_userviewed>(new tbl_userviewed { viewId = Guid.NewGuid().ToString(), postsID = postsId, userID = openId, viewTime = DateTime.Now }).ExecuteCommand();

        }

        public long getReadCount(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            long readCount = db.Queryable<tbl_userviewed>().Where(o => o.postsID == postsId).GroupBy(o => new { o.userID, o.postsID }).Select(o => o.userID).Count();
            return readCount;
        }

        public void userLoveReply(string openId, string replyId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_userReplyLoved replyLoved = db.Queryable<tbl_userReplyLoved>().Where(o => o.replyId == replyId && o.openId == openId).First();
            if (replyLoved == null)
            {
                db.Insertable<tbl_userReplyLoved>(new tbl_userReplyLoved { lovedId = Guid.NewGuid().ToString(), openId = openId, replyId = replyId, lovedTime = DateTime.Now }).ExecuteCommand();
            }
            else
            {
                db.Deleteable<tbl_userReplyLoved>().Where(o => o.replyId == replyId && o.openId == openId).ExecuteCommand();
            }
        }

        public void delReply(string replyId, string openId, int delType)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_reply reply = new tbl_reply();
            reply = db.Queryable<tbl_reply>().Where(o => o.replyID == replyId).First();
            db.Insertable<tbl_delReason>(new tbl_delReason
            {
                delId = Guid.NewGuid().ToString(),
                delContent = reply.replyContent,
                delTime = DateTime.Now,
                delType = delType == 0 ? DelType.ReplyDel : DelType.SeriousDel,
                delUser = openId,
                delOpenId = reply.replyMaker
            }).ExecuteCommand();

            if (delType == 2)
            {
                tbl_user user = new tbl_user();
                user = db.Queryable<tbl_user>().Where(o => o.openid == reply.replyMaker).First();
                db.Updateable<tbl_user>().UpdateColumns(o => new tbl_user { userStatus = 1 }).Where(o => o.openid == user.openid).ExecuteCommand();
                List<string> postsList = db.Queryable<tbl_posts>().Where(o => o.postsMaker == reply.replyMaker).Select(o => o.postsID).ToList();
                db.Deleteable<tbl_postspics>().Where(o => postsList.Contains(o.postsID)).ExecuteCommand();
                db.Deleteable<tbl_reply>().Where(o => o.replyMaker == reply.replyMaker).ExecuteCommand();
                db.Deleteable<tbl_reply>().Where(o => postsList.Contains(o.postsID)).ExecuteCommand();
                db.Deleteable<tbl_replyAfterReply>().Where(o => o.replyMaker == reply.replyMaker).ExecuteCommand();
            }

            db.Deleteable<tbl_reply>().Where(o => o.replyID == replyId).ExecuteCommand();
            db.Deleteable<tbl_replyAfterReply>().Where(o => o.replyId == replyId).ExecuteCommand();
        }

        public List<List<ReplyNLoveCount>> getReplyNLoveCount(string openId)
        {
            DateTime lastRefresh = new UserProvider().getLastRefreshDate(openId);
            SqlSugarClient db = SqlSugarInstance.newInstance();
            DataTable replyCount = db.Ado.UseStoredProcedure().GetDataTable("proc_getReplyCount", new { openId = openId, fromwhen = lastRefresh });
            var i = 0;
            List<ReplyNLoveCount> replyCountList = new List<ReplyNLoveCount>();
            //replyCountList = db.Queryable<tbl_posts, tbl_user, tbl_reply, tbl_postspics>((tp, tu, tr, tpp) => new object[]
            //{
            //    JoinType.Left,tp.postsMaker==tu.openid,
            //    JoinType.Left,tp.postsID==tr.postsID,
            //    JoinType.Left,tp.postsID==tpp.postsID
            //}).Select((tp, tu, tr, tpp) => new ReplyNLoveCount
            //{
            //    postsId = tp.postsID,
            //    replyCount = SqlFunc.AggregateCount(tr.replyID),
            //    picsSimpPath = tpp.picSimpPath,
            //    postsLoveCount = 0,
            //    replydate=tr.replyDate.Value,
            //    picindex=tpp.picIndex,
            //    replyMaker=tr.replyMaker,
            //    postsmaker=tp.postsMaker
            //}).MergeTable().Where(tr=>tr.replyMaker!=openId && tr.replydate>=lastRefresh).Where(tpp=>tpp.picindex==0)
            //.Where(tp=>tp.postsmaker==openId)
            //.GroupBy(tp => tp.postsId).ToList();
            if (replyCount.Rows.Count > 0)
            {
                for (i = 0; i < replyCount.Rows.Count; i++)
                {
                    replyCountList.Add(new ReplyNLoveCount
                    {
                        postsId = replyCount.Rows[i][0].ToString(),
                        replyCount = Convert.ToInt64(replyCount.Rows[i][1]),
                        picsSimpPath = replyCount.Rows[i][2].ToString(),
                        postsLoveCount = 0
                    });
                }
            }
            DataTable lovedCount = db.Ado.UseStoredProcedure().GetDataTable("proc_getPostsLoved", new { openId = openId, fromwhen = lastRefresh });
            i = 0;
            List<ReplyNLoveCount> lovedCountList = new List<ReplyNLoveCount>();
            //lovedCountList = db.Queryable<tbl_posts, tbl_user, tbl_userloved, tbl_postspics>((tp, tu, tul, tpp) => new object[]
            //{
            //    JoinType.Left,tp.postsMaker==tu.openid,
            //    JoinType.Left,tp.postsID==tul.postsID,
            //    JoinType.Left,tp.postsID==tpp.postsID
            //}).Select((tp, tu, tul, tpp) => new ReplyNLoveCount
            //{
            //    postsId = tp.postsID,
            //    replyCount = 0,
            //    picsSimpPath = tpp.picSimpPath,
            //    postsLoveCount = SqlFunc.AggregateCount(tul.lovedID),
            //    lovedTime = tul.lovedTime.Value,
            //    picindex = tpp.picIndex,
            //    userId = tul.userID,
            //    postsmaker = tp.postsMaker
            //}).MergeTable().Where(tul => tul.userId != openId && tul.lovedTime >= lastRefresh).Where(tpp => tpp.picindex == 0)
            //.Where(tp => tp.postsmaker == openId)
            //.GroupBy(tp => tp.postsId).ToList();
            if (lovedCount.Rows.Count > 0)
            {
                for (i = 0; i < lovedCount.Rows.Count; i++)
                {
                    lovedCountList.Add(new ReplyNLoveCount
                    {
                        postsId = lovedCount.Rows[i][0].ToString(),
                        postsLoveCount = Convert.ToInt64(lovedCount.Rows[i][1]),
                        picsSimpPath = lovedCount.Rows[i][2].ToString(),
                        replyCount = 0
                    });
                }
            }
            List<List<ReplyNLoveCount>> result = new List<List<ReplyNLoveCount>>();
            result.Add(replyCountList);
            result.Add(lovedCountList);
            return result;
        }

        public List<List<ReplyNLoveCount>> getAfterReplyNLoveCount(string openId)
        {
            DateTime lastRefresh = new UserProvider().getLastRefreshDate(openId);
            SqlSugarClient db = SqlSugarInstance.newInstance();
            DataTable replyCount = db.Ado.UseStoredProcedure().GetDataTable("proc_getAfterReplyCount", new { openId = openId, fromwhen = lastRefresh });
            var i = 0;
            List<ReplyNLoveCount> replyCountList = new List<ReplyNLoveCount>();
            //replyCountList = db.Queryable<tbl_posts, tbl_reply, tbl_replyAfterReply, tbl_postspics>((tp, tr, tra, tpp) => new object[]
            //{
            //    JoinType.Left,tr.replyID==tra.replyId,
            //    JoinType.Left,tp.postsID==tr.postsID,
            //    JoinType.Left,tp.postsID==tpp.postsID
            //})
            //.Where((tp, tr, tra, tpp)=>(tr.replyMaker==openId || tra.replyToUser==openId) && tra.replyMaker!=openId && tra.replyDate>=lastRefresh &&tpp.picIndex==0)
            //.Select((tp, tr, tra, tpp) => new ReplyNLoveCount
            //{
            //    postsId = tp.postsID,

            //    replyCount = SqlFunc.AggregateCount(tra.replyId),
            //    picsSimpPath = tpp.picSimpPath,
            //    postsLoveCount = 0,
            //    replydate = tra.replyDate,
            //    picindex = tpp.picIndex,
            //    replyMaker = tr.replyMaker,
            //    postsmaker = tp.postsMaker,
            //    replyToUser=tra.replyToUser
            //}).MergeTable()
            //.GroupBy(tp => tp.postsId).ToList();
            if (replyCount.Rows.Count > 0)
            {
                for (i = 0; i < replyCount.Rows.Count; i++)
                {
                    replyCountList.Add(new ReplyNLoveCount
                    {
                        postsId = replyCount.Rows[i][0].ToString(),
                        replyCount = Convert.ToInt64(replyCount.Rows[i][2]),
                        picsSimpPath = replyCount.Rows[i][1].ToString(),
                        postsLoveCount = 0
                    });
                }
            }
            DataTable lovedCount = db.Ado.UseStoredProcedure().GetDataTable("proc_getAfterReplyLoved", new { openId = openId, fromwhen = lastRefresh });
            i = 0;
            List<ReplyNLoveCount> lovedCountList = new List<ReplyNLoveCount>();
            //lovedCountList = db.Queryable<tbl_posts, tbl_reply, tbl_userReplyLoved, tbl_postspics>((tp, tr, tra, tpp) => new object[]
            // {
            //    JoinType.Left,tr.replyID==tra.replyId,
            //    JoinType.Left,tp.postsID==tr.postsID,
            //    JoinType.Left,tp.postsID==tpp.postsID
            // })
            //.Where((tp, tr, tra, tpp) => tr.replyMaker == openId  && tra.openId != openId && tra.lovedTime >= lastRefresh && tpp.picIndex == 0)
            //.Select((tp, tr, tra, tpp) => new ReplyNLoveCount
            //{
            //    postsId = tp.postsID,
            //    replyCount = 0,
            //    picsSimpPath = tpp.picSimpPath,
            //    postsLoveCount = SqlFunc.AggregateCount(tra.lovedId),
            //    replydate = tra.lovedTime,
            //    picindex = tpp.picIndex,
            //    replyMaker = tr.replyMaker,
            //    postsmaker = tp.postsMaker
            //    //replyToUser = tra.replyToUser
            //}).MergeTable()
            //.GroupBy(tp => tp.postsId).ToList();
            if (lovedCount.Rows.Count > 0)
            {
                for (i = 0; i < lovedCount.Rows.Count; i++)
                {
                    lovedCountList.Add(new ReplyNLoveCount
                    {
                        postsId = lovedCount.Rows[i][0].ToString(),
                        postsLoveCount = Convert.ToInt64(lovedCount.Rows[i][2]),
                        picsSimpPath = lovedCount.Rows[i][1].ToString(),
                        replyCount = 0
                    });
                }
            }
            List<List<ReplyNLoveCount>> result = new List<List<ReplyNLoveCount>>();
            result.Add(replyCountList);
            result.Add(lovedCountList);
            return result;
        }

        public void newReport(tbl_report report)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            if (db.Queryable<tbl_report>().Where(o => o.postsId == report.postsId && o.userId == report.userId).Count() == 0)
            {
                db.Insertable<tbl_report>(report).ExecuteCommand();
            }
        }

        public long getReportTimes(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            long reportTimes = db.Queryable<tbl_report>().Where(o => o.postsId == postsId).Count();
            return reportTimes;
        }

        public long getViewedTimes(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            long viewedTimes = db.Queryable<tbl_userviewed>().Where(o => o.postsID == postsId).Count();
            return viewedTimes;
        }

        public bool ifCanRead(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            bool ifValid = db.Queryable<tbl_posts>().Where(o => o.postsID == postsId).First().postsStatus != 1;
            return ifValid;
        }

        public void updatePostsStatus(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Updateable<tbl_posts>().UpdateColumns(o => new tbl_posts { postsStatus = 1 }).Where(o => o.postsID == postsId).ExecuteCommand();
        }

        public long getWaitingList()
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            long count = db.Queryable<tbl_posts>().Where(o => o.postsStatus == 1).Count();
            return count;
        }

        public void verifyPosts(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Updateable<tbl_posts>().UpdateColumns(o => new tbl_posts { postsStatus = 2 }).Where(o => o.postsID == postsId).ExecuteCommand();
            db.Deleteable<tbl_report>().Where(o => o.postsId == postsId).ExecuteCommand();//将已存在的举报记录删除
        }

        public bool ifCanReport(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            return db.Queryable<tbl_posts>().Where(o => o.postsID == postsId).First().postsStatus <= 1;//2为已审核通过的，不可再次举报
        }

        public string addReplyAfterReply(string replyId, string openId, string replyContent, string replyToUser)
        {
            tbl_replyAfterReply reply = new tbl_replyAfterReply();
            reply.afterReplyId = Guid.NewGuid().ToString();
            reply.replyContent = replyContent;
            reply.replyId = replyId;
            reply.replyDate = DateTime.Now;
            reply.replyMaker = openId;
            reply.replyToUser = replyToUser;
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Insertable<tbl_replyAfterReply>(reply).ExecuteCommand();
            return reply.afterReplyId;
        }

        public List<UserReply> getAfterReplyList(string replyId, int from, int count, DateTime refreshTime)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            List<UserReply> replyList = db.Queryable<tbl_replyAfterReply, tbl_user, tbl_user>((tr, tu, tu2) => new object[] {
                JoinType.Left,tr.replyMaker==tu.openid,
                JoinType.Left,tr.replyToUser==tu2.openid
            }).Where((tr, tu, tu2) => tr.replyId == replyId && tr.replyDate <= refreshTime)
            .OrderBy((tr, tu, tu2) => tr.replyDate)
            .Select((tr, tu, tu2) => new UserReply
            {
                replyId = tr.afterReplyId,
                replyContent = tr.replyContent,
                replyDate = tr.replyDate,
                replyMaker = tr.replyMaker,
                nickName = tu.nickName,
                avantarUrl = tu.avantarUrl,
                replyToUser = tu2.nickName
            }).ToList();
            return replyList;
        }

        public void delReplyAfterReply(string afterReplyId, string openId, int delType)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();

            tbl_replyAfterReply reply = db.Queryable<tbl_replyAfterReply>().Where(o => o.afterReplyId == afterReplyId).First();
            db.Insertable<tbl_delReason>(new tbl_delReason
            {
                delId = Guid.NewGuid().ToString(),
                delContent = reply.replyContent,
                delTime = DateTime.Now,
                delType = delType == 0 ? DelType.ReplyDel : DelType.SeriousDel,
                delUser = openId,
                delOpenId = reply.replyMaker
            }).ExecuteCommand();

            if (delType == 2)
            {
                tbl_user user = new tbl_user();
                user = db.Queryable<tbl_user>().Where(o => o.openid == reply.replyMaker).First();
                db.Updateable<tbl_user>().UpdateColumns(o => new tbl_user { userStatus = 1 }).Where(o => o.openid == user.openid).ExecuteCommand();
                List<string> postsList = db.Queryable<tbl_posts>().Where(o => o.postsMaker == reply.replyMaker).Select(o => o.postsID).ToList();
                db.Deleteable<tbl_postspics>().Where(o => postsList.Contains(o.postsID)).ExecuteCommand();
                db.Deleteable<tbl_reply>().Where(o => o.replyMaker == reply.replyMaker).ExecuteCommand();
                db.Deleteable<tbl_reply>().Where(o => postsList.Contains(o.postsID)).ExecuteCommand();
                db.Deleteable<tbl_replyAfterReply>().Where(o => o.replyMaker == reply.replyMaker).ExecuteCommand();
            }

            db.Deleteable<tbl_replyAfterReply>().Where(o => o.afterReplyId == afterReplyId).ExecuteCommand();
            //db.Deleteable<tbl_userReplyLoved>().Where(o => o.replyId == replyId).ExecuteCommand();
        }

        public bool ifUserLovedReply(string openId, string replyId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_userReplyLoved loveReply = new tbl_userReplyLoved();
            loveReply = db.Queryable<tbl_userReplyLoved>().Where(o => o.replyId == replyId && o.openId == openId).First();
            if (loveReply != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void sharePosts(string openId, string postsId)
        {
            string shareId = Guid.NewGuid().ToString();
            tbl_userShare userShare = new tbl_userShare();
            userShare.userId = openId;
            userShare.postsId = postsId;
            userShare.shareId = shareId;
            userShare.shareTime = DateTime.Now;
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Insertable<tbl_userShare>(userShare).ExecuteCommand();
        }

        public long getRepliesCount(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            long repliesCount = db.Queryable<tbl_reply>().Where(o => o.postsID == postsId).Count();
            return repliesCount;
        }

        public long saveShareCode(string postsId, string openId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_sharecode code = new tbl_sharecode();
            code.openId = openId;
            code.postsId = postsId;
            code.shareTime = DateTime.Now;
            long shareId = db.Insertable<tbl_sharecode>(code).ExecuteReturnBigIdentity();
            return shareId;
        }

        public tbl_sharecode getShareCodeRecord(string shareId)
        {
            long shareIdLong = Convert.ToInt64(shareId);
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_sharecode result = db.Queryable<tbl_sharecode>().Where(o => o.shareId == shareIdLong).First();
            return result;
        }

        public List<tbl_event> getEventList()
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            List<tbl_event> eventList = new List<tbl_event>();
            eventList = db.Queryable<tbl_event>().OrderBy(o => o.eventIndex).ToList();
            return eventList;
        }

        public long getShareCount(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            long result = db.Queryable<tbl_userShare>().Where(o => o.postsId == postsId).Count() + db.Queryable<tbl_sharecode>().Where(o => o.postsId == postsId).Count();
            return result;
        }

        public void addRobotContent(tbl_robotContent content)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Insertable<tbl_robotContent>(content).ExecuteCommand();
        }

        public int getRand()
        {
            return 0;
        }

        public tbl_robotContent getRobotContent()
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_robotContent content = db.Queryable<tbl_robotContent>().Where(o => o.ifUsed == 0).OrderBy(o => getRand()).First();
            string contentId = content.contentId;
            content.ifUsed = 1;
            db.Updateable<tbl_robotContent>(content).Where(o => o.content == contentId).ExecuteCommand();
            return content;
        }

        public void addLog(string logContent)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_log log = new tbl_log();
            log.logId = Guid.NewGuid().ToString();
            log.logContent = logContent;
            log.logTime = DateTime.Now;
            db.Insertable<tbl_log>(log).ExecuteCommand();
        }

        public void addRobotReply(tbl_robotReply reply)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            db.Insertable<tbl_robotReply>(reply).ExecuteCommand();
        }

        public tbl_robotReply getRobotReply(string type)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_robotReply reply = new tbl_robotReply();
            if (type == "")
            {
                reply = db.Queryable<tbl_robotReply>().OrderBy(o => getRand()).First();
            }
            else
            {
                reply = db.Queryable<tbl_robotReply>().Where(o => o.replyType == type).OrderBy(o => getRand()).First();
            }
            return reply;

        }

        public List<string> getPostsInLast36Hr()
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            List<string> postsList = db.Queryable<tbl_posts>().Where(o => SqlFunc.DateAdd(o.postsMakeDate.Value, 36, DateType.Hour) > DateTime.Now).Select(o => o.postsID).ToList();
            return postsList;
        }

        public bool ifPostedByRobot(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            tbl_posts posts = db.Queryable<tbl_posts>().Where(o => o.postsID == postsId).First();
            string poster = posts.postsMaker;
            tbl_user user = db.Queryable<tbl_user>().Where(o => o.openid == poster).First();
            return user.ifRobot == 1;
        }

        public List<tbl_postspics> getAllPicsByPostsId(string postsId)
        {
            SqlSugarClient db = SqlSugarInstance.newInstance();
            List<tbl_postspics> pics = db.Queryable<tbl_postspics>().Where(o => o.postsID == postsId).OrderBy(o => o.picIndex).ToList();
            return pics;
        }
    }
}
