using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookApplicationUsingGraphAPI.Models
{
    public class Post
    {
        public string PostId { set; get; }
        public DateTime CreatedDateTime { get; set; }
        public string ContainText { get; set; }
        public List<User> Users { set; get; }

    }

    public class User
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string Picture { set; get; }
    }
}