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
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult UserHome(string userID)
        {
            SDLink result = null;
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_SDLink(userID);
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
            return View("UserHome", result);
        }

        [ChildActionOnly]
        public ActionResult AvatarCover(string userID)
        {
            SDLink result = null;
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_SDLink(userID);
                if (Session["UserID"] != null && userID == Session["UserID"].ToString()) ViewBag.CanChange = "true";
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

        [HttpGet]
        public ActionResult ChangeAvatar()
        {
            ViewBag.Action = "UploadAvatar";
            ViewBag.Controller = "User";
            return View("_ImageUpload");
        }

        [HttpPost]
        public ActionResult UploadAvatar()
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                string userID = Session["UserID"].ToString();
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/User/Avatar/" + userID + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("UserHome", "User", new { userID = userID });

            }
            else return View("_ImageUpload");

        }

        [HttpGet]
        public ActionResult ChangeCover()
        {
            ViewBag.Action = "UploadCover";
            ViewBag.Controller = "User";
            return View("_ImageUpload");
        }

        [HttpPost]
        public ActionResult UploadCover()
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                string userID = Session["UserID"].ToString();
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/User/Cover/" + userID + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("UserHome", "User", new { userID = userID });

            }
            else return View("_ImageUpload");
        }

        public ActionResult PersonalInformation(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (Session["UserID"] != null && Session["UserID"].ToString() == userID)
                ViewBag.IsMyHome = true;
            else ViewBag.IsMyHome = false;

            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_PersonalInformation(userID);
                return PartialView("_PersonalInformation", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult JoinedGroups(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (Session["UserID"] != null && Session["UserID"].ToString() == userID)
                ViewBag.IsMyHome = true;
            else ViewBag.IsMyHome = false;

            try
            {
                // get joined group list
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_JoinedGroups(userID);
                // get joined group Info
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

                return PartialView("_JoinedGroups", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult Friends(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                if (Session["UserID"] != null && Session["UserID"].ToString() == userID)
                    ViewBag.IsMyHome = true;
                else ViewBag.IsMyHome = false;

                ViewBag.UserID = userID;

                return PartialView("_Friends");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult FriendList(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (Session["UserID"] != null && Session["UserID"].ToString() == userID)
                ViewBag.IsMyHome = true;
            else ViewBag.IsMyHome = false;

            try
            {
                // get friend
                SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                var listID = relationDAO.Get_Friends(userID);
                // get friend
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                return PartialView("_FriendList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        [ChildActionOnly]
        public ActionResult CurrentProjects(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (Session["UserID"] != null && Session["UserID"].ToString() == userID)
                ViewBag.IsMyHome = true;
            else ViewBag.IsMyHome = false;

            try
            {
                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Current_Projects(userID);
                // get joined group Info
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectsInformation(listID);

                return PartialView("_CurrentProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [ChildActionOnly]
        public ActionResult OrganizedProjects(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Organized_Projects(userID);
                // get joined group Info
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectsInformation(listID);

                return PartialView("_OrganizedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [ChildActionOnly]
        public ActionResult SponsoredProjects(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Sponsored_Projects(userID);
                // get joined group Info
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectsInformation(listID);

                return PartialView("_SponsoredProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [ChildActionOnly]
        public ActionResult ParticipatedProjects(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Participated_Projects(userID);
                // get joined group Info
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectsInformation(listID);

                return PartialView("_ParticipatedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult ActivityHistory(string userID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(userID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                ViewBag.UserID = userID;
                return PartialView("_ActivityHistory");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult SearchUser(string name)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    ViewBag.Message = Error.INVALID_INFORMATION;
                    return View("ErrorMessage");
                }

                Mongo_User_DAO userDAO = new Mongo_User_DAO();

                List<AccountInformation> result = new List<AccountInformation>();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = userDAO.User_Search(name, true);
                else result = userDAO.User_Search(name, false);

                return View("SearchUser", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

    }
}