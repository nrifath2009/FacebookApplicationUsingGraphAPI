using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using FacebookApplicationUsingGraphAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FacebookApplicationUsingGraphAPI.Controllers
{
    public class PostSearchController : Controller
    {
        private static string accessToken = "CAACEdEose0cBAMuamfXfkuZBaQ1Qrkdawchn1tZBfsVHKCqfH8cF6wpMtnH5CHIGOVtnWp3Po4EAZAtOdwdtBQe6mpnty10tGFZCu7aeZAevvYhqjClU8vppGSbNN0bx26ltgx5rGSftbZAk5QzUr2ZAupCPrrwO3AdRGb8yphZAawHWRocumhkZBHqkNTwAKZCIP3UBZAHoGZC8imk5ZB8QGhdBED3vkH3T2rEUZD";
        FacebookClient client = new FacebookClient(accessToken);
        //
        // GET: /PostSearch/
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult PageInfo()
        {
            
            //var client = new FacebookClient(accessToken);
            dynamic me = client.Get("me?fields=id,email,first_name,last_name,gender,picture");
            FacebookUserModel fmodel = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserModel>(me.ToString());
            return View(fmodel);


        }

        public ActionResult FacebookPages()
        {
            ManagePageByUser aManagePageByUser = new ManagePageByUser();
            
            //var client = new FacebookClient(accessToken);

            dynamic me = client.Get("me/accounts?fields=id,name,category");
            var data = me["data"].ToString();
            aManagePageByUser.pageList = JsonConvert.DeserializeObject<List<FacebookUserPage>>(data);
            return View(aManagePageByUser);
        }


       


        //Here SelectedPage ID is Not Passing......
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FacebookPages(string pageId)
        {
            ManagePageByUser aManagePageByUser = new ManagePageByUser();

            //var client = new FacebookClient(accessToken);

            dynamic me = client.Get("me/accounts?fields=id,name,category");
            var data = me["data"].ToString();
            dynamic pageName = client.Get(pageId + "?fields=name,likes");
            string pname = pageName.ToString();
            JObject rss = JObject.Parse(pname);
            ViewBag.PageNameById = (string) rss.SelectToken("name");
            ViewBag.PageLikesById = (int)rss.SelectToken("likes");
            aManagePageByUser.pageList = JsonConvert.DeserializeObject<List<FacebookUserPage>>(data);
            try
            {
                ViewBag.PostsList = GetPage(pageId).postLists.ToList();
                return View(aManagePageByUser);
            }
            catch (NullReferenceException exception)
            {
                ViewBag.PostsList = null;
                return View(aManagePageByUser);
            }
        }


        public int GetNoOfLikesInPost(string postId)
        {
            //var client = new FacebookClient(accessToken);

            dynamic me = client.Get(postId+"?fields=likes.summary(true)");
            string msg = me.ToString();
            JObject rss = JObject.Parse(msg);
            try
            {
                int totalLikes = (int) rss.SelectToken("likes.summary.total_count");
                return totalLikes;
            }
            catch (ArgumentNullException exception)
            {
                return 0;
            }
        }

        public int GetNoofShares(string postId)
        {
            //var client = new FacebookClient(accessToken);

            dynamic me = client.Get(postId + "?fields=shares");
            string msg = me.ToString();
            JObject rss = JObject.Parse(msg);
            try
            {
                int totalShare = (int) rss.SelectToken("shares.count");
                return totalShare;
            }
            catch (ArgumentNullException exception)
            {
                return 0;
            }
        }

        public string GetPostMessage(string postId)
        {
           // var client = new FacebookClient(accessToken);

            dynamic me = client.Get(postId + "?fields=message");
            string msg = me.ToString();
            JObject rss = JObject.Parse(msg);
            string message = (string)rss.SelectToken("message");
            return message;
        }

        public List<User> GetLikers(string postId)
        {
           // var client = new FacebookClient(accessToken);
            dynamic me = client.Get(postId + "?fields=likes{id,name,picture}");
            string msg = me.ToString();
            JObject rss = JObject.Parse(msg);
            int noOfLikes = GetNoOfLikesInPost(postId);
            List<User> users = new List<User>();
            for (int i = 0; i < noOfLikes; i++)
            {
                string name = (string) rss.SelectToken("likes.data["+i+"].name");
                string picture = (string) rss.SelectToken("likes.data["+i+"].picture.data.url");
                users.Add(
                        new User()
                        {
                            Name = name,
                            Picture = picture
                        }
                    );
            }

            return users;


        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetPagePosts(string pageId)
        {
           
            dynamic me = client.Get(pageId + "?fields=posts{message,likes{id,name,picture}}");
            string msg = me.ToString();
            JObject rss = JObject.Parse(msg);
            List<Post> posted = new List<Post>();
            List<User> users = new List<User>();
           
            //Working Correctly............
           // string message = (string) rss.SelectToken("posts.data[0].likes.data[0].name");
            //-------------------------------------------
            for (int i = 0; i < rss.Count + 1; i++)
            {
                string message = (string)rss.SelectToken("posts.data[" + i + "].message");
                // string picture = (string)rss.SelectToken("posts.data[0].likes.data[0].pic_square");
                string name = (string)rss.SelectToken("posts.data[" + i + "].likes.data[" + i + "].name");

                posted.Add(
                        new Post()
                        {
                            ContainText = message,
                            Users = users
                        }
                    );
            }

            
            
            
            return Json(posted, JsonRequestBehavior.AllowGet);
        }

       public AdminPost GetPage(string pageId)
       {
           
           AdminPost adminPost = new AdminPost();
           //var client = new FacebookClient(accessToken);
           string query = pageId+"?fields=posts{message}";
           dynamic me = client.Get(query);
           try
           {
               var data = me["posts"]["data"].ToString();
               adminPost.postLists = JsonConvert.DeserializeObject<List<UserPosts>>(data);
               return adminPost;
           }
           catch (KeyNotFoundException exception)
           {
               return null;
           }

       }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPostDetails(string postId)
        {
            List<User> likersList = GetLikers(postId);
            int NoOfLikes = GetNoOfLikesInPost(postId);
            string Message = GetPostMessage(postId);
            int NoOfShares = GetNoofShares(postId);


            ViewBag.Message = Message;
            ViewBag.NoOfLikes = NoOfLikes;
            ViewBag.LikersList = likersList;
            ViewBag.NoOfShares = NoOfShares;
            return View();

        }

        
	}
}