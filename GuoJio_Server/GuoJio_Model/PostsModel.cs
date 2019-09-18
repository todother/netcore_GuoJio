using System;
namespace CatsPrj.Model
{
    public class PostsModel
    {
        public string postsID { get; set; }

        public string postsContent { get; set; }

        public string postsMaker { get; set; }


        public DateTime? postsMakeDate { get; set; }




        public int? postsPicCount { get; set; }



        public long? postsReaded { get; set; }


        public long? postsLoved { get; set; }

        public string postsPics { get; set; }

        public string makerName { get; set; }
        public string makerID { get; set; }

        public string picsSimpPath { get; set; }

        public string picsPath { get; set; }

        public string whenPosts { get; set; }

        public string makerPhoto { get; set; }

        public double latitude { get; set; }
        public double longitude { get; set; }
        public string postsLocation { get; set; }
        public string postsType { get; set; }
        public int postsStatus { get; set; }
        public int ifOfficial { get; set; }
        public int ifLY { get; set; }

        public decimal picsRate { get; set; }

        public bool ifUserLoved { get; set; }
    }
}
