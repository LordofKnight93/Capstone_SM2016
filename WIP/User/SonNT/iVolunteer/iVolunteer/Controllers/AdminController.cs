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
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Manage()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return View("Manage");
        }
        /// <summary>
        /// Get group list
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public ActionResult ListGroup(int page)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            //check input, use default value if not pass
            if (page <= 0) page = 1;

            ViewBag.CurrentPage = page;
            List<GroupInformation> result = null;
            try
            {
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                result = groupDAO.Get_All_GroupInformation(10 * (page - 1), 10);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return View("_ListGroup", result);
        }
        public ActionResult ListProject(int page)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            //check input, use default value if not pass
            if (page <= 0) page = 1;

            ViewBag.CurrentPage = page;
            List<ProjectInformation> result = null;
            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                result = projectDAO.Get_All_ProjectInformation(10 * (page - 1), 10);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return View("_ListProject", result);
        }
        public ActionResult ListAccount(int page)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            //check input, use default value if not pass
            if (page <= 0) page = 1;

            ViewBag.CurrentPage = page;
            List<AccountInformation> result = null;
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_All_AccountInformation(10 * (page - 1), 10);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return View("_ListAccount",result);
        }/// <summary>
        /// ban an account
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult AccountBanned(string userID, int currentPage)
        {
            if (Session["Role"]== null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (currentPage <= 0) currentPage = 1;
            ViewBag.CurrentPage = currentPage;

            try
            {
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                sqlDAO.Set_Activation_Status(userID, Status.IS_BANNED);
                mongoDAO.Set_Activation_Status(userID, Status.IS_BANNED);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return RedirectToAction("ListAccount", "Admin", new { page = currentPage });
        }
        /// <summary>
        /// reactivate an account
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult AccountActivate(string userID, int currentPage)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (currentPage <= 0) currentPage = 1;
            ViewBag.CurrentPage = currentPage;

            try
            {
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                sqlDAO.Set_Activation_Status(userID, Status.IS_ACTIVATE);
                mongoDAO.Set_Activation_Status(userID, Status.IS_ACTIVATE);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return RedirectToAction("ListAccount", "Admin", new { page = currentPage });
        }
        /// <summary>
        /// ban a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult GroupBanned(string groupID, int currentPage)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (currentPage <= 0) currentPage = 1;
            ViewBag.CurrentPage = currentPage;

            try
            {
                Mongo_Group_DAO mongoDAO = new Mongo_Group_DAO();
                SQL_Group_DAO sqlDAO = new SQL_Group_DAO();
                sqlDAO.Set_Activation_Status(groupID, Status.IS_BANNED);
                mongoDAO.Set_Activation_Status(groupID, Status.IS_BANNED);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return RedirectToAction("ListGroup", "Admin", new { page = currentPage });
        }
        /// <summary>
        /// reactivate a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult GroupActivate(string groupID, int currentPage)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (currentPage <= 0) currentPage = 1;
            ViewBag.CurrentPage = currentPage;

            try
            {
                Mongo_Group_DAO mongoDAO = new Mongo_Group_DAO();
                SQL_Group_DAO sqlDAO = new SQL_Group_DAO();
                sqlDAO.Set_Activation_Status(groupID, Status.IS_ACTIVATE);
                mongoDAO.Set_Activation_Status(groupID, Status.IS_ACTIVATE);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return RedirectToAction("ListGroup", "Admin", new { page = currentPage});
        }
        /// <summary>
        /// banned a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult ProjectBanned(string projectID, int currentPage)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (currentPage <= 0) currentPage = 1;
            ViewBag.CurrentPage = currentPage;

            try
            {
                Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                sqlDAO.Set_Activation_Status(projectID, Status.IS_BANNED);
                mongoDAO.Set_Activation_Status(projectID, Status.IS_BANNED);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return RedirectToAction("ListProject", "Admin", new { pag = currentPage });
        }
        /// <summary>
        /// reactivate project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult ProjectActivate(string projectID, int currentPage)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (currentPage <= 0) currentPage = 1;
            ViewBag.CurrentPage = currentPage;

            try
            {
                Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                sqlDAO.Set_Activation_Status(projectID, Status.IS_ACTIVATE);
                mongoDAO.Set_Activation_Status(projectID, Status.IS_ACTIVATE);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return RedirectToAction("ListProject", "Admin", new { page = currentPage });
        }
    }
}