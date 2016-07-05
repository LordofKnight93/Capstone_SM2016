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
    public class HomeController : Controller
    {
        public ActionResult FrontPage()
        {
            return View();
        }
        public ActionResult Newfeed()
        {
            if (Session["Role"].ToString() == "Admin") return RedirectToAction("Manage", "Admin");
            return View("Newfeed");
        }

        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid) return View();
            SQL_Account_DAO accountDAO = new SQL_Account_DAO();
            SQL_Account account = new SQL_Account();
            try
            {
                account = accountDAO.Get_Account_By_Email(loginModel.Email);
            }
            catch
            {
                account = null;
            }

            if (account == null)
            {
                ViewBag.Message = Error.ACCOUNT_NOT_EXIST;
                return PartialView("Login", loginModel);
            }
            if (!account.IsActivate)
            {
                ViewBag.Message = Error.ACCOUNT_BANNED;
                return PartialView("Login", loginModel);
            }
            if (!account.IsConfirm)
            {
                ViewBag.Message = Error.EMAIL_NOT_CONFIRM;
                return PartialView("Login", loginModel);
            }
            if (account.Password != loginModel.Password)
            {
                ViewBag.Message = Error.WRONG_PASSWORD;
                return PartialView("Login", loginModel);
            }
            // code save cookie wil be added here later

            //set session information
            Session["UserID"] = account.UserID;
            Session["DisplayName"] = account.DisplayName;
            Session["Role"] = account.IsAdmin ? "Admin" : "User";
            //redirect
            if (account.IsAdmin)
                return RedirectToAction("Manage", "Admin");
            else
                return RedirectToAction("Newfeed");
        }
        //GET : Register
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("FrontPage", "Home");
        }
        public ActionResult Register()
        {
            if (Session["UserID"] != null) return RedirectToAction("Newfeed", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel registerModel)
        {
            if (Session["UserID"] != null) return RedirectToAction("Newfeed", "Home");

            string err = "";
            bool isValid = true;
            // validate information
            if (!ValidationHelper.IsValidEmail(registerModel.Email) 
                    || !ValidationHelper.IsValidIdentifyID(registerModel.IdentifyID)
                    || !ValidationHelper.IsValidPassword(registerModel.Password)
                    || !ValidationHelper.IsValidPhone(registerModel.Phone))
            {
                err = Error.INVALID_INFORMATION + Environment.NewLine;
                isValid = false;
            }

            //create DAO instance
            Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
            SQL_Account_DAO sql_Account_DAO = new SQL_Account_DAO();

            // check if email existed
            if (sql_Account_DAO.Is_Email_Exist(registerModel.Email))
            {
                err = err + Error.ACCOUNT_EXIST + Environment.NewLine;
                isValid = false;
            }

            // check if identifyID exist
            if (sql_Account_DAO.Is_IdentifyID_Exist(registerModel.IdentifyID))
            {
                err = err + Error.IDENTIFYID_EXIST + Environment.NewLine;
                isValid = false;
            }
            if (!isValid)
            {
                ViewBag.Message = err;
                return View("Register",registerModel);
            }
            //create account in MongoDB
            Mongo_User user = new Mongo_User(registerModel);

            //create account in SQL
            SQL_Account account = new SQL_Account();
            account.UserID = user.AccountInformation.UserID;
            account.Email = registerModel.Email;
            account.Password = registerModel.Password;
            account.IndentifyID = registerModel.IdentifyID;
            account.DisplayName = registerModel.RealName;
            account.IsAdmin = Role.IS_USER;
            account.IsActivate = Status.IS_ACTIVATE;
            account.IsConfirm = Status.IS_NOT_CONFIRMED;

            //write to DB
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    //write to DB
                    sql_Account_DAO.Add_Account(account);
                    mongo_User_DAO.Add_User(user);

                    // copy default avatar and cover
                    FileInfo avatar = new FileInfo(Server.MapPath(Default.DEFAULT_AVATAR));
                    avatar.CopyTo(Server.MapPath("/Images/User/Avatar/" + account.UserID + ".jpg"));
                    FileInfo cover = new FileInfo(Server.MapPath(Default.DEFAULT_COVER));
                    cover.CopyTo(Server.MapPath("/Images/User/Cover/" + account.UserID + ".jpg"));

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }

            return View("Activate",account);
        }
        [ChildActionOnly]
        public ActionResult NavigationPanel()
        {
            AccountInformation result = null;
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                result = mongoDAO.Get_AccountInformation(userID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return PartialView("_NavigationPanel", result);
        }

        public ActionResult Search(string name)
        {
            SearchModel searchModel = new SearchModel();
            searchModel.Name = name;
            return View("Search", searchModel);
        }
    }
}