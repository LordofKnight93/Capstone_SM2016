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
using iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass;
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

        public ActionResult UserInformation(string userID)
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
                var result = userDAO.Get_UserInformation(userID);
                return PartialView("_UserInformation", result);
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
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_ActivityHistory(userID);
                return PartialView("_ActivityHistory", result);
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
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_JoinedGroups(userID);
                return PartialView("_JoinedGroups", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

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
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_CurrentProjects(userID);
                return PartialView("_CurrentProjects", result);
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
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_FriendList(userID);
                return PartialView("_FriendList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult UserSearch(string name, int page)
        {
            try
            {
                List<AccountInformation> result = new List<AccountInformation>();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = userDAO.Search_Account(name, 10 * (page - 1), 10);
                else result = userDAO.Search_Account(name, Status.IS_ACTIVATE, 10 * (page - 1), 10);
                return PartialView("_UserResult", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
    }
}