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
            ProjectInformation result = null; ;
            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                result = projectDAO.Get_ProjectInformation(projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return PartialView(result);
        }
        public ActionResult AvatarCover(string projectID)
        {
            SDLink result = null;
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                result = projectDAO.Get_SDLink(projectID);
                if (relationDAO.Get_Specific_Relation(userID, projectID) == Relation.LEADER_RELATION) ViewBag.CanChange = "true";
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return PartialView("_AvatarCover", result);
        }
        public ActionResult ChangeAvatar(string id)
        {
            ViewBag.Option = "ProjectAvatar";
            ViewBag.ID = id;
            return View("_ImageUpload");
        }

        [HttpPost]
        public ActionResult UploadAvatar(string id)
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/Project/Avatar/" + id + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("ProjectHome", "Project", new { projectID = id });

            }
            else return View("_ImageUpload");

        }
        public ActionResult ChangeCover(string id)
        {
            ViewBag.Option = "ProjectCover";
            ViewBag.ID = id;
            return View("_ImageUpload");
        }
        [HttpPost]
        public ActionResult UploadCover(string id)
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/Project/Cover/" + id + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("ProjectHome", "Project", new { projectID = id });

            }
            else return View("_ImageUpload");

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
            //create project DSlink
            SDLink project = new SDLink(mongo_Project.ProjectInformation);

            //create sql project
            SQL_Project sql_Project = new SQL_Project();
            sql_Project.ProjectID = mongo_Project.ProjectInformation.ProjectID;
            sql_Project.IsActivate = true;

            //create first relation
            SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
            relation.UserID = creator.ID;
            relation.ProjectID = mongo_Project.ProjectInformation.ProjectID;
            relation.Relation = Relation.LEADER_RELATION;

            //this code will change user information, will add later

            //start transaction
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //create DAO instance 
                    Mongo_Project_DAO mongo_Project_DAO = new Mongo_Project_DAO();
                    Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
                    SQL_Project_DAO sql_Project_DAO = new SQL_Project_DAO();
                    SQL_AcPr_Relation_DAO sql_User_Project_DAO = new SQL_AcPr_Relation_DAO();

                    //write to DB
                    sql_Project_DAO.Add_Project(sql_Project);
                    sql_User_Project_DAO.Add_Relation(relation);
                    mongo_Project_DAO.Add_Project(mongo_Project);
                    mongo_User_DAO.Add_CurrentProject(creator.ID, project);
                    mongo_User_DAO.Add_OrganizedProject(creator.ID, project);

                    // copy default avatar and cover
                    FileInfo avatar = new FileInfo(Server.MapPath(Default.DEFAULT_AVATAR));
                    avatar.CopyTo(Server.MapPath("/Images/Project/Avatar/" + sql_Project.ProjectID + ".jpg"));
                    FileInfo cover = new FileInfo(Server.MapPath(Default.DEFAULT_COVER));
                    cover.CopyTo(Server.MapPath("/Images/Project/Cover/" + sql_Project.ProjectID + ".jpg"));

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }

            ViewBag.Message = "Tạo dự án tình nguyện thành công";
            return RedirectToAction("Newfeed", "Home");
        }
        public ActionResult ProjectHome(string projectID)
        {
            SDLink result = null;
            try
            {
                SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                if (sqlDAO.IsActivate(projectID))
                {
                    Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                    result = mongoDAO.Get_SDLink(projectID);
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }
            return View("ProjectHome", result);
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