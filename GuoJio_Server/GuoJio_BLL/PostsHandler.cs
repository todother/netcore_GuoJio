using GuoJio_Converter;
using GuoJio_DAL;
using GuoJio_Model;
using Sugar.Enties;
using System;
using System.Collections.Generic;
using System.Text;

namespace GuoJio_BLL
{
    public class PostsHandler
    {
        public List<PostsModel> getPosts(string openId, int from, int count, DateTime refreshTime, int currSel, double lati, double longti)
        {
            PostsProvider provider = new PostsProvider();
            IList<PostsPics> pics = new List<PostsPics>();
            pics = provider.getPosts(openId, from, count, 1, refreshTime, currSel, lati, longti);
            List<PostsModel> result = new List<PostsModel>();

            foreach (var item in pics)
            {
                result.Add(PostsConverter.postsEntityToModel(item));
            }
            return result;
        }
    }
}
