using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookApplicationUsingGraphAPI.Models
{
    public class FacebookUserModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
        public Picture picture { get; set; }
    }
}