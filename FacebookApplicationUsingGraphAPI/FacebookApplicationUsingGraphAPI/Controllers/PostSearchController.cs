using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;

namespace FacebookApplicationUsingGraphAPI.Controllers
{
    public class PostSearchController : Controller
    {
        //
        // GET: /PostSearch/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult PageInfo()
        {
            var accessToken = "CAACEdEose0cBAHlmZCe7S3v4bTvyk7ghAGmS0hwLSIlAsU2ZBbUgjPzpdhb9qNZBSLqHWFn3HZBlcgYVqeiOpkzfhB5pOj1V7Wdi2ZAjeylWTUyCSUctmqluvCgbaMeR8wxY1Bk9rExbFqXcBkBmQHdgZB8c8t0nOYwDsq62Xh6NwTVnEFmSVUMDOOWnxb8DY5Y3HZBSzoJGmxojwOJpJ2z4Trl7dFVSFcZD";
            var client = new FacebookClient(accessToken);
            dynamic me = client.Get("me");
            return Json(me, JsonRequestBehavior.AllowGet);
        }

        
	}
}