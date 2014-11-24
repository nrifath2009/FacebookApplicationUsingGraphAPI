using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookApplicationUsingGraphAPI.Models
{
    public class AdminPost
    {
        public List<UserPosts> postLists { get; set; }
    }

    public class UserPosts
    {
        public string id { set; get; }
        public string message { get; set; }
         
        

       
    }
}