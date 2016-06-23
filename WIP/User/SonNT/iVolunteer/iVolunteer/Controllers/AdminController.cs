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
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
            return View("_Manage");
        }
        /// <summary>
        /// Get group list
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public ActionResult ListGroup(int skip, int number)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
            //check input, use default value if not pass
            if (skip < 0||number <=0)
            {
                skip = 0;
                number = 10;
            }
            ViewBag.Next = skip + number;
            ViewBag.Previous = skip - number;
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            var result = groupDAO.Get_All_GroupInformation(skip, number);
            return View("_ListGroup", result);
        }

        public ActionResult ListProject(int skip, int number)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
            //check input, use default value if not pass
            if (skip < 0 || number <= 0)
            {
                skip = 0;
                number = 10;
            }
            ViewBag.Next = skip + number;
            ViewBag.Previous = skip - number;
            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
            var result = projectDAO.Get_All_ProjectInformation(skip, number);
            return View("_ListProject", result);
        }
        public ActionResult ListAccount(int skip, int number)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
            //check input, use default value if not pass
            if (skip < 0 || number <= 0)
            {
                skip = 0;
                number = 10;
            }
            ViewBag.Next = skip + number;
            ViewBag.Previous = skip - number;
            Mongo_User_DAO projectDAO = new Mongo_User_DAO();
            var result = projectDAO.Get_All_AccountInformation(skip, number);
            return View("_ListAccount",result);
        }

        public ActionResult AccountBanned(string userID)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
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
            }
            return RedirectToAction("ListAccount", "Admin", new { skip = 0, number = 10 });
        }
        public ActionResult AccountActivate(string userID)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
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
            }
            return RedirectToAction("ListAccount", "Admin", new { skip = 0, number = 10 });
        }

        public ActionResult GroupBanned(string groupID)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
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
            }
            return RedirectToAction("ListGroup", "Admin", new { skip = 0, number = 10 });
        }
        public ActionResult GroupActivate(string groupID)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
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
            }
            return RedirectToAction("ListGroup", "Admin", new { skip = 0, number = 10 });
        }

        public ActionResult ProjectBanned(string groupID)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
            try
            {
                Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                sqlDAO.Set_Activation_Status(groupID, Status.IS_BANNED);
                mongoDAO.Set_Activation_Status(groupID, Status.IS_BANNED);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
            }
            return RedirectToAction("ListProject", "Admin", new { skip = 0, number = 10 });
        }
        public ActionResult ProjectActivate(string groupID)
        {
            if (Session["Role"].ToString() == null || Session["Role"].ToString() != "Admin") return RedirectToAction("FrontPage", "Home");
            try
            {
                Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                sqlDAO.Set_Activation_Status(groupID, Status.IS_ACTIVATE);
                mongoDAO.Set_Activation_Status(groupID, Status.IS_ACTIVATE);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
            }
            return RedirectToAction("ListProject", "Admin", new { skip = 0, number = 10 });
        }
    }
}