using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iVolunteer.Models.SQL;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;
using iVolunteer.DAL.MongoDB;
using iVolunteer.DAL.SQL;
using System.Transactions;

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
                //ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("Post");
            }
        }
        [HttpGet]
        public ActionResult AddPost()
        {
            if (Session["UserID"] != null) return PartialView("AddPost");

            return View();
        }
        [HttpPost]
        public ActionResult AddPost(PostInformation postInfor)
        {
            postInfor.DateCreate = DateTime.Now;
            postInfor.FollowCount = 1;
            postInfor.LikeCount = 0;
            //create creator
            SDLink creator = new SDLink();
            creator.ID = Session["UserID"].ToString();
            creator.DisplayName = Session["DisplayName"].ToString();
            creator.Handler = Handler.POST;

            //create mongo Post
            Mongo_Post mongo_Post = new Mongo_Post(creator, postInfor);

            //create post SDLink
            SDLink post = new SDLink(mongo_Post.PostInfomation);

            //create sql Post
            SQL_Post sql_Post = new SQL_Post();
            sql_Post.PostID = mongo_Post.PostInfomation.PostID;
            //create relation
            SQL_AcPo_Relation relation_Po = new SQL_AcPo_Relation();
            relation_Po.UserID = creator.ID;
            relation_Po.PostID = mongo_Post.PostInfomation.PostID;
            relation_Po.Relation = Relation.FOLLOW_RELATION;

            //start transaction
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //create DAO instance
                    Mongo_Post_DAO mongo_Post_DAO = new Mongo_Post_DAO();
                    Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();

                    SQL_Post_DAO sql_Post_DAO = new SQL_Post_DAO();
                    SQL_AcPo_Relation_DAO sql_User_Post_DAO = new SQL_AcPo_Relation_DAO();

                    //write to DB
                    sql_Post_DAO.Add_Post(sql_Post);
                    sql_User_Post_DAO.Add_Relation_Po(relation_Po);
                    mongo_Post_DAO.Add_Post(mongo_Post);

                    transaction.Complete();
                }
                catch (Exception)
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("Error Message");
                }
            }
            return View();
        }
    }
}