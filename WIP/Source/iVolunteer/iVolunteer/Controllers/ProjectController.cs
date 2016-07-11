using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.Mvc;
using iVolunteer.Models.SQL;
using iVolunteer.Models.ViewModel;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.DAL.SQL;
using iVolunteer.DAL.MongoDB;
using iVolunteer.Common;
using System.IO;

namespace iVolunteer.Controllers
{
    public class ProjectController : Controller
    {
        [HttpGet]
        public ActionResult CreateProject()
        {
            return PartialView("_CreateProject");
        }

        [HttpPost]
        public ActionResult CreateProject(ProjectInformation projectInfo)
        {
            if (!ModelState.IsValid) return View();

            //set missing information for project
            projectInfo.DateCreate = DateTime.Now;
            projectInfo.MemberCount = 1;
            projectInfo.InProgress = Status.ONGOING;
            projectInfo.IsActivate = Status.IS_ACTIVATE;
            projectInfo.IsRecruit = Status.IS_RECRUITING;

            string userID = Session["UserID"].ToString();

            //create mongo project
            Mongo_Project mongo_Project = new Mongo_Project(projectInfo);

            //create sql project
            SQL_Project sql_Project = new SQL_Project();
            sql_Project.ProjectID = mongo_Project.ProjectInformation.ProjectID;
            sql_Project.InProgress = Status.ONGOING;
            sql_Project.IsActivate = true;

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
                    sql_User_Project_DAO.Add_Leader(userID, sql_Project.ProjectID);

                    mongo_Project_DAO.Add_Project(mongo_Project);
                    mongo_User_DAO.Join_Project(userID);

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

            return RedirectToAction("ProjectHome", "Project", new { projectID = sql_Project.ProjectID });
        }

        [HttpGet]
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

                if (Session["UserID"] != null)
                {
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    if (Session["UserID"].ToString() == "Admin")
                        ViewBag.IsAdmin = true;
                    else
                    {
                        string userID = Session["UserID"].ToString();
                        ViewBag.IsJoined = relationDAO.Is_Joined(userID, projectID);
                    }
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
            else return View("ProjectHome", result);
        }

        [ChildActionOnly]
        public ActionResult AvatarCover(string projectID)
        {
            SDLink result = null;
            try
            {
                //get data
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                result = projectDAO.Get_SDLink(projectID);
                //get ralation 
                if (Session["UserID"] == null)
                {
                    ViewBag.CanChange = "false";
                }
                else
                {
                    string userID = Session["UserID"].ToString();
                    if (relationDAO.Is_Leader(userID, projectID))
                        ViewBag.CanChange = "true";
                    else ViewBag.CanChange = "false";
                }
                return PartialView("_AvatarCover", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        [HttpGet]
        public ActionResult ChangeAvatar(string id)
        {
            //check permission here

            ViewBag.Action = "UploadAvatar";
            ViewBag.Controller = "Project";
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

        [HttpGet]
        public ActionResult ChangeCover(string id)
        {
            //check permission here

            ViewBag.Action = "UploadCover";
            ViewBag.Controller = "Project";
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

        public ActionResult ProjectInformation(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectInformation(projectID);
                if(Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                return PartialView("_ProjectInformation", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        [ChildActionOnly]
        public ActionResult ProjectLeaders(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Leaders(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                return PartialView("_ProjectLeaders", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [ChildActionOnly]
        public ActionResult ProjectMembers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Members(projectID);
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                return PartialView("_ProjectMembers", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult SearchProject(string name)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    ViewBag.Message = Error.INVALID_INFORMATION;
                    return View("ErrorMessage");
                }

                Mongo_Project_DAO pạoectDAO = new Mongo_Project_DAO();

                List<ProjectInformation> result = new List<ProjectInformation>();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = pạoectDAO.Project_Search(name, true);
                else result = pạoectDAO.Project_Search(name, false);

                return View("SearchProject", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult ProjectPublic()
        {
            return PartialView("_ProjectPublic");
        }

        public ActionResult ProjectDiscussion()
        {
            return PartialView("_ProjectDiscussion");
        }

        public ActionResult ProjectGallery()
        {
            return PartialView("_ProjectGallery");
        }

        public ActionResult ProjectPlan()
        {
            return PartialView("_ProjectPlan");
        }

        public ActionResult ProjectStructure()
        {
            return PartialView("_ProjectStructure");
        }
    }
}