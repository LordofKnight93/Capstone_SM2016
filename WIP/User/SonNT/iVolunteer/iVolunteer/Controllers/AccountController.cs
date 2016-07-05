using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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
    public class AccountController : Controller
    {
        public ActionResult Confirm(string userID)
        {
            try
            {
                SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                sqlDAO.Set_Confirmation_Status(userID, Status.IS_CONFIRMED);
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                mongoDAO.Set_Comfirmation_Status(userID, Status.IS_CONFIRMED);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return RedirectToAction("Login", "Home");
        }
        [HttpGet]
        public ActionResult ChangeDisplayName()
        {
            if (Session["UserID"] == null) return RedirectToAction("FrontPage", "Home");
            string userID = Session["UserID"].ToString();
            try
            {
                SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                DateTime? date = sqlDAO.Get_DateOfChange(userID);

                if (date == null) return View("_ChangeDisplayName");

                DateTime now = DateTime.Now.Date;
                TimeSpan? span = now - date;
                if(span.Value.Days <= 90 && span != null)
                {
                    ViewBag.Message = Error.DISPLAYNAME_FAIL;
                    return View("ErrorMessage");
                }
                else return View("_ChangeDisplayName");
            }
            catch 
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        [HttpPost]
        public ActionResult ChangeDisplayName(string name)
        {
            if (Session["UserID"] == null) return RedirectToAction("FrontPage", "Home");
            string userID = Session["UserID"].ToString();
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                    Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                    sqlDAO.Set_DisplayName(userID, name);
                    sqlDAO.Set_DateOfChange(userID);
                    mongoDAO.Set_DisplayName(userID, name);
                    //perform more change name here

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("UserProfile", "User", new { userID = userID });
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string password)
        {
            //get userID
            string userID = Session["UserID"].ToString();
            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    //create dao instance
                    SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                    Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                    //update
                    sqlDAO.Set_Password(userID, password);
                    mongoDAO.Set_Password(userID, password);

                    transaction.Complete();
                 }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("UserProfile", "User", new { userID = userID });
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}