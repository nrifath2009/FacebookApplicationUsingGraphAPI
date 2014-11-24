using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookApplicationUsingGraphAPI.Models
{
    public class PostLikes
    {
        public List<Like> likesList { set; get; }
    }
    public class Like
    {
        public string id { set; get; }
        public string name { set; get; }
        public Picture picture { set; get; }

    }
}