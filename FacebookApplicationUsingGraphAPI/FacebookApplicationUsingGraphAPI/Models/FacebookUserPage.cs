using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookApplicationUsingGraphAPI.Models
{
    public class FacebookUserPage
    {
        public string id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
    }

    public class ManagePageByUser
    {
        public List<FacebookUserPage> pageList { set; get; }
    }
}