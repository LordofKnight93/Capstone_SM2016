using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.SQL;
using iVolunteer.DAL.MongoDB;
using iVolunteer.DAL.SQL;
using iVolunteer.Common;
using MongoDB.Bson;
using iVolunteer.Models.ViewModel;
using System.IO;

namespace iVolunteer.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string postID)
        {
            //get userID
            string userID = Session["UserID"].ToString();
            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");

            try
            {
                SQL_Post_DAO sqlDAO = new SQL_Post_DAO();
                if (sqlDAO.IsAccessable(userID, postID) == false) return RedirectToAction("FrontPage", "Home");
                Mongo_Post_DAO mongoDAO = new Mongo_Post_DAO();
                var result = mongoDAO.Get_Post_By_ID(postID);
                return View("Post", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("Post");
            }
        }
    }
}