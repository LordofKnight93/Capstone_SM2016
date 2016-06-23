using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
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
    public class ProjectController : Controller
    {
        public ActionResult Information( string projectID)
        {
            ProjectInformation result = new ProjectInformation();
            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                result = projectDAO.Get_ProjectInformation(projectID);
            }
            catch
            {
                result = null;
            }
            return View(result);
        }
        public ActionResult AvatarCover(string projectID)
        {
            SDLink result = new SDLink();
            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                result = projectDAO.Get_SDLink(projectID);
            }
            catch
            {
                RedirectToAction("FrontPage", "Home");
            }
            return View("_AvatarCover", result);
        }

        //GET : CreateProject
        [HttpGet]
        public ActionResult CreateProject()
        {
            return View();
        }
        //POST : CreateProject
        [HttpPost]
        public ActionResult CreateProject( ProjectInformation projectInfo )
        {
            if (!ModelState.IsValid) return View();

            //set missing information for project
            projectInfo.DateCreate = DateTime.Now;
            projectInfo.MemberCount = 1;
            projectInfo.IsActivate = true;
            projectInfo.IsRecruit = true;

            //create creator
            SDLink creator = new SDLink();
            creator.ID = Session["UserID"].ToString();
            creator.DisplayName = Session["DisplayName"].ToString();
            creator.Handler = Handler.USER;

            //create mongo project
            Mongo_Project mongo_Project = new Mongo_Project(creator, projectInfo);

            //create sql project
            SQL_Project sql_Project = new SQL_Project();
            sql_Project.ProjectID = mongo_Project.ProjectInformation.ProjectID;
            sql_Project.IsActivate = true;

            //create first relation
            SQL_User_Project relation = new SQL_User_Project();
            relation.UserID = creator.ID;
            relation.ProjectID = mongo_Project.ProjectInformation.ProjectID;
            relation.RelationType = Relation.LEADER_RELATION;

            //this code will change user information, will add later

            //start transaction
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //create DAO instance 
                    Mongo_Project_DAO mongo_Project_DAO = new Mongo_Project_DAO();
                    SQL_Project_DAO sql_Project_DAO = new SQL_Project_DAO();
                    SQL_User_Project_DAO sql_User_Project_DAO = new SQL_User_Project_DAO();
                    //write to DB
                    sql_Project_DAO.Add_Project(sql_Project);
                    sql_User_Project_DAO.Add_Relation(relation);
                    mongo_Project_DAO.Add_Project(mongo_Project);

                    // copy default avatar and cover
                    FileInfo avatar = new FileInfo(Server.MapPath("~") + Default.DEFAULT_AVATAR);
                    avatar.CopyTo(Server.MapPath("~") + "/Images/Project/Avatar/" + sql_Project.ProjectID + ".jpg");
                    FileInfo cover = new FileInfo(Server.MapPath("~") + Default.DEFAULT_COVER);
                    cover.CopyTo(Server.MapPath("~") + "/Images/Project/Cover/" + sql_Project.ProjectID + ".jpg");

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = "Có lỗi xảy ra, vui lòng thử lại sau ít phút";
                    return View();
                }
            }

            ViewBag.Message = "Tạo dự án tình nguyện thành công";
            return RedirectToAction("Newfeed", "Home");
        }
        public ActionResult ProjectHome()
        {
            return View();
        }

        public PartialViewResult DisplayPublic()
        {
            return PartialView("_Public");
        }

        public PartialViewResult DisplayDiscussion()
        {
            return PartialView("_Discussion");
        }
    }
}