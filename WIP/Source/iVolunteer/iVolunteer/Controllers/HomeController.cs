using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.Mvc;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Models.Data_Access_Object.MongoDB;
using iVolunteer.Models.Data_Access_Object.SQL;
using iVolunteer.Common;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.ViewModel;

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
            if (Session["UserID"] == null) return RedirectToAction("Login");
            return View("Newfeed");
        }

        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
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
                ViewBag.Message = "Tài khoản không tồn tại";
                return View("Login", loginModel);
            }
            if (!account.IsActivate)
            {
                ViewBag.Message = "Tài khoản đã bị khóa";
                return View("Login", loginModel);
            }
            if (!account.IsConfirm)
            {
                ViewBag.Message = "Tài khoản chưa được xác nhận";
                return View("Login", loginModel);
            }
            if (account.Password != loginModel.Password)
            {
                ViewBag.Message = "Mật khẩu không chính xác";
                return View("Login", loginModel);
            }
            // code save cookie wil be added here later


            Session["UserID"] = account.UserID;
            Session["DisplayName"] = account.DisplayName;
            Session["Avatar"] = account.AvtImgLink;
            Session["IsAdmin"] = account.IsAdmin;
            
            return RedirectToAction("Newfeed");
        }
        //GET : Register
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel registerModel)
        {
            if(!ValidationHelper.IsValidEmail(registerModel.Email) 
                    || !ValidationHelper.IsValidIdentifyID(registerModel.IdentifyID))
            {
                ViewBag.Message = "Thông tin không hợp lệ";
                return View("Register");
            }
            //create account in SQL
            ObjectId _id = ObjectId.GenerateNewId();
            SQL_Account account = new SQL_Account();
            account.UserID = _id.ToString();
            account.Email = registerModel.Email;
            account.Password = registerModel.Password;
            account.IndentifyID = registerModel.IdentifyID;
            account.DisplayName = registerModel.RealName;
            account.AvtImgLink = Constant.DEFAULT_AVATAR;
            account.IsAdmin = Constant.IS_USER;
            account.IsActivate = Constant.IS_ACTIVATE;
            account.IsConfirm = Constant.IS_NOT_CONFIRMED;

            //create account in MongoDB
            Mongo_User user = new Mongo_User(registerModel);
            //write to DB
            
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    //create DAO instance
                    Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
                    SQL_Account_DAO sql_Account_DAO = new SQL_Account_DAO();
                    //write to DB
                    mongo_User_DAO.Add_User(user);
                    sql_Account_DAO.Add_Account(account);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = "Có lỗi xảy ra, vui lòng thử lại sau ít phút";
                    return View();
                }
            }

            return RedirectToAction("Login");
        }
    }
}