using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
        facebookDBEntities db = new facebookDBEntities();
        private static string accessToken = "CAACEdEose0cBACWZCmMXMZBrHR71lZBcGfYr42eHgnP07XvZBgJ7FDK4LZBh68zaoUBrFIU4j2QtAsc33j6oalDRWDeZCnyhjh8jIEgecloo3FpTcr9SSABKMWZBdZAiZCRdm3bfC8qZCnIMC1COm9HNx9srXe5ZBPsPUcxBYb6XZAkOD1B3Sap3UxCVuvsvw0oswLlZAB3RK04M9DBlCOUXmmxzYn4909WOHxzsZD";
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
                string id = (string)rss.SelectToken("likes.data[" + i + "].id");
                string picture = (string) rss.SelectToken("likes.data["+i+"].picture.data.url");
                users.Add(
                        new User()
                        {
                            Id  = id,
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

       // [HttpPost]
        //[ValidateAntiForgeryToken]
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

        public JsonResult GetData(string postId)
        {
            int id = postId.IndexOf('_');
            string postShared = postId.Substring(id + 1);
            //GetSharerId(postShared,GetNoofShares(postId));

            return Json( GetSharerId(postShared,GetNoofShares(postId)), JsonRequestBehavior.AllowGet);
        }

        public string GetPostShareId(string postId)
        {
            int id = postId.IndexOf('_');
            string postShared = postId.Substring(id + 1);
            //GetSharerId(postShared,GetNoofShares(postId));

            return postShared;
        }

        public List<Share> GetSharerId(string postShared, int noOfShares)
        {
            dynamic me = client.Get(postShared + "/sharedposts");
            string msg = me.ToString();
            JObject rss = JObject.Parse(msg);
            List<Share> sharerList = new List<Share>();
           

            for (int i = 0; i < noOfShares; i++)
            {
                Share aShare = new Share();
                aShare.SharerId = (string) rss.SelectToken("data["+i+"].from.id");
                sharerList.Add(aShare);
            }
            return sharerList;
        }

        
        public bool IsLikeThisPost(string userId, string postId)
        {
            
            List<User> user = GetLikers(postId);
            var result = user.Where(a => a.Id == userId);
            if (result.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
                
        
        }

        public bool IsSharedThisPost(string userId, string postId)
        {
            string postShared = GetPostShareId(postId);
            int noOfShares = GetNoofShares(postId);
            List<Share> shareList = GetSharerId(postShared, noOfShares);
            var user = shareList.Where(a => a.SharerId == userId);
            if (user.Any())
                return true;
            else
                return false;


        }

        public void SaveInDatabase(string postId)
        {
            List<t_post> posts = new List<t_post>();
            List<User> likers = GetLikers(postId);
            string postShared = GetPostShareId(postId);
            int noOfShares = GetNoofShares(postId);
            List<Share> sharerList = GetSharerId(postShared, noOfShares);
            //bool isShareThisPost = IsSharedThisPost();





            if (sharerList.Any())
            {
                foreach (Share sharer in sharerList)
                {
                    t_post post = new t_post();
                    post.facebook_post_id = postId;
                    if (IsLikeThisPost(sharer.SharerId, postId))
                    {
                        post.like_post = true;
                    }
                    else
                    {
                        post.like_post = false;
                    }
                    post.share_post = true;
                    post.sharer_facebook_id = sharer.SharerId;
                    posts.Add(post);
                }
            }
            else
            {
                foreach (User user in likers)
                {
                    t_post post = new t_post();
                    post.facebook_post_id = postId;
                    post.like_post = true;
                    post.sharer_facebook_id = user.Id;
                    if (IsSharedThisPost(user.Id, postId))
                    {
                        post.share_post = true;
                    }
                    else
                    {
                        post.share_post = false;
                    }
                    posts.Add(post);
                }
            }

            if (db.t_post.Any())
            {
                foreach (t_post post in posts)
                {
                    db.Entry(post).State = EntityState.Modified;
                    db.SaveChanges();
                }
               
            }
            else
            {
                foreach (t_post post in posts)
                {
                    db.t_post.AddOrUpdate(post);
                }
                db.SaveChanges();
            }
           

            // return Json(posts, JsonRequestBehavior.AllowGet);


        }

        
	}
}