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
    public class HomeController : Controller
    {
        /// <summary>
        /// ホームページを表示
        /// </summary>
        /// <returns></returns>
        public ActionResult FrontPage()
        {
            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.FrontPage_Project(0, 10);
                return View("FrontPage", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// チャットルーム画面を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult ChatRoom()
        {
            if (Session["UserID"] != null) return View("_ChatRoom");
            return View();
        }
        /// <summary>
        /// 過去の通知画面を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadAllNotification()
        {
            try
            {
                if (Session["UserID"] == null) return null;

                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                List<Notification> notifyList = userDAO.Get_Notifications(userID, 0, 100);
                return View("NotificationsPage", notifyList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 過去の友達申請画面を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult FriendRequestAll()
        {
            try
            {
                if (Session["UserID"] == null) return null;

                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                List<Notification> notifyList = userDAO.Get_Old_FriendAcceptedNotification(userID);
                return View("FriendRequestAll", notifyList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// ニュースフィード画面を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult Newfeed()
        {
            if (Session["UserID"] == null)
            {
                return FrontPage();
            }
            if (Session["Role"].ToString() == "Admin") return RedirectToAction("Manage", "Admin");
            return View("Newfeed");
        }
        public ActionResult LoadNewfeedPosts()
        {
            if (Session["UserID"] == null)
            {
                return FrontPage();
            }
            string userID = Session["UserID"].ToString();
            try
            {
                //Get Groups and Projects that User followed and joined
                SQL_AcGr_Relation_DAO grRelation = new SQL_AcGr_Relation_DAO();
                SQL_AcPr_Relation_DAO prRelation = new SQL_AcPr_Relation_DAO();
                List<string> groups = grRelation.Get_Joined_Groups(userID);
                List<string> projects = prRelation.Get_Current_Projects(userID);
                List<string> flGroups = grRelation.Get_Followed_Groups(userID);
                List<string> flProjects = prRelation.Get_Followed_Projects(userID);
                List<string> destinations = new List<string>();
                List<string> flDestinations = new List<string>();

                if (groups.Count == 0 && projects.Count == 0 && flGroups.Count == 0 && flProjects.Count == 0)
                {
                    return PartialView("_NewfeedPosts", null);
                }
                else
                {
                    //if (groups.Count != 0 && projects.Count != 0)
                    //{
                    //    destinations.AddRange(groups);
                    //    destinations.AddRange(projects);
                    //}
                    //else if (groups.Count != 0) destinations.AddRange(groups);
                    //else destinations.AddRange(projects);
                    destinations.AddRange(groups);
                    destinations.AddRange(projects);
                    flDestinations.AddRange(flGroups);
                    flDestinations.AddRange(flProjects);
                }
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> posts = postDAO.Get_NewFeed_Post_All(destinations, flDestinations, 0, 10);
                return PartialView("_NewfeedPosts", posts);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// 登録画面を表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Register()
        {
            if (Session["UserID"] != null) return RedirectToAction("Newfeed", "Home");
            return View();
        }
        /// <summary>
        /// 登録する
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegisterModel registerModel)
        {
            if(!ModelState.IsValid) return View("Register", registerModel);

            // if user already login then redirect ro new feed
            if (Session["UserID"] != null) return RedirectToAction("Newfeed", "Home");

            string err = "";
            bool isValid = true;

            try
            {
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
                    return View("Register", registerModel);
                }

                //encrypt password here
                
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
                        return View("Register", registerModel);
                    }
                }

                return View("/Views/Account/Confirm.cshtml", account);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("Register", registerModel);
            }
        }
        /// <summary>
        /// ロギング画面を表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            return PartialView("_Login");
        }
        /// <summary>
        /// ロギングする
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid) return PartialView("_Login", loginModel);

            SQL_Account account = null;
            try
            {
                SQL_Account_DAO accountDAO = new SQL_Account_DAO();
                account = accountDAO.Get_Account_By_Email(loginModel.Email);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("_Login", loginModel);
            }

            if (account == null)
            {
                ViewBag.Message = Error.ACCOUNT_NOT_EXIST;
                return PartialView("_Login", loginModel);
            }
            if (!account.IsActivate)
            {
                ViewBag.Message = Error.ACCOUNT_BANNED;
                return PartialView("_Login", loginModel);
            }
            if (!account.IsConfirm)
            {
                ViewBag.Message = Error.EMAIL_NOT_CONFIRM;
                return PartialView("_Login", loginModel);
            }
            if (account.Password != loginModel.Password)
            {
                ViewBag.Message = Error.WRONG_PASSWORD;
                return PartialView("_Login", loginModel);
            }
            // code save cookie wil be added here later

            //set session information
            Session["UserID"] = account.UserID;
            Session["DisplayName"] = account.DisplayName;
            Session["Role"] = account.IsAdmin ? "Admin" : "User";

            return JavaScript("location.reload(true)");
            ////redirect
            //if (account.IsAdmin)
            //    return RedirectToAction("Manage", "Admin");
            //else
            //    return RedirectToAction("Newfeed","Home");
        }
        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("FrontPage", "Home");
        }
        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="name"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public ActionResult Search(string name, string option)
        {
            switch (option)
            {
                case "User":
                    if (String.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
                    {
                        ViewBag.Message = "Rất tiếc, chúng tôi không hiểu tìm kiếm này. Vui lòng thử truy vấn theo cách khác.";
                        return View("ErrorMessage");
                    }
                    return RedirectToAction("SearchUser", "User", new { name = name, page = 1 });
                case "Group":
                    if (String.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
                    {
                        ViewBag.Message = "Rất tiếc, chúng tôi không hiểu tìm kiếm này. Vui lòng thử truy vấn theo cách khác.";
                        return View("ErrorMessage");
                    }
                    return RedirectToAction("SearchGroup", "Group", new { name = name, page = 1 });
                case "Project":
                    SearchModel searchModel = new SearchModel();
                    searchModel.Name = name;
                    TempData["SearchModel"] = searchModel;
                    return RedirectToAction("SearchProject", "Project");
                default:
                    ViewBag.Message = Error.INVALID_INFORMATION;
                    return View("ErrorMessage");
            }
        }
        /// <summary>
        /// パスワード忘れ画面を表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return PartialView("_ForgotPassword");
        }
        /// <summary>
        /// パスワード忘れメールを放送
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            try
            {
                ViewBag.Email = email;

                if (!ValidationHelper.IsValidEmail(email))
                {
                    ViewBag.Message = Error.EMAIL_INVALID;
                    return PartialView("_ForgotPassword");
                }

                SQL_Account_DAO accountDAO = new SQL_Account_DAO();
                if(!accountDAO.Is_Email_Exist(email))
                {
                    ViewBag.Message = "Email này chưa đăng ký!";
                    return PartialView("_ForgotPassword");
                }
                //get password
                string password = accountDAO.Get_Account_By_Email(email).Password;
                //encrypt here

                //sent mail here

                ViewBag.Message = "Mật khẩu đã được gửi vào hòm thư!";
                return PartialView("_ForgotPassword");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult LoadMorePostNewfeed(int times)
        {
            if(Session["UserID"] == null)
            {
                return FrontPage();
            }
            string userID = Session["UserID"].ToString();
            try
            {
                int skip = times * 10;
                SQL_AcGr_Relation_DAO grRelation = new SQL_AcGr_Relation_DAO();
                SQL_AcPr_Relation_DAO prRelation = new SQL_AcPr_Relation_DAO();
                List<string> groups = grRelation.Get_Joined_Groups(userID);
                List<string> projects = prRelation.Get_Current_Projects(userID);
                List<string> flGroups = grRelation.Get_Followed_Groups(userID);
                List<string> flProjects = prRelation.Get_Followed_Projects(userID);
                List<string> destinations = new List<string>();
                List<string> flDestinations = new List<string>();
                destinations.AddRange(groups);
                destinations.AddRange(projects);
                flDestinations.AddRange(flGroups);
                flDestinations.AddRange(flProjects);
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                //List<Mongo_Post> posts = postDAO.Get_NewFeed_Post(destinations, skip, 10);
                List<Mongo_Post> posts = postDAO.Get_NewFeed_Post_All(destinations, flDestinations, skip, 10);
                return PartialView("_NewfeedPosts", posts);
            }
            catch
            {
                throw;
            }
        }
    }
}