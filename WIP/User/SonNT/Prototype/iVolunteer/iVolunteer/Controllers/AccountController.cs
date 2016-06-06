using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iVolunteer.Models.ViewModel;
using iVolunteer.Models.SQL.Data_Access_Object;
using iVolunteer.Models.SQL.DBContext;
using iVolunteer.Models.MongoDB.Data_Definition_Class.Embedded_CLass;
using iVolunteer.Models.MongoDB.Data_Access_Object;
using iVolunteer.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace iVolunteer.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        //POST : Login
        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            Account account = AccountDAO.GetAccountByEmail(loginModel.Email);
            if(account == null)
            {
                ViewBag.Message = "Account doesn't exist";
                return View("Login",loginModel);
            }
            if(!account.IsActivate)
            {
                ViewBag.Message = "Account is banned";
                return View("Login",loginModel);
            }
            if (!account.IsConfirm)
            {
                ViewBag.Message = "Account isn't confirmed yet";
                return View("Login", loginModel);
            }
            if (account.Password != loginModel.Password)
            {
                ViewBag.Message = "Wrong Password";
                return View("Login", loginModel);
            }
            Session["UserID"] = account.UserID;
            Session["IsAdmin"] = account.IsAdmin;

            return RedirectToAction("Newfeed", "Home");
        }

        // GET: Account/Details/5
        public ActionResult Details(string id)
        {
            UserInformation userInfo = UserInformationDAO.GetUserInformationByID(id);
            return View("Details",userInfo);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public ActionResult Register(RegisterModel registerModel)
        {
            try
            {
                bool isValid = true;
                string error = "";
                //check email existed
                if(AccountDAO.IsEmailExisted(registerModel.Email))
                {
                    error += "Email đã tồn tại \r\n";
                    isValid = false;
                }
                //check email valid
                if (!ValidationHelper.IsEmail(registerModel.Email))
                {
                    error += "Email không hợp lệ \r\n";
                    isValid = false;
                }
                // check identifyID existed
                if (AccountDAO.IsIdentifyIDExisted(registerModel.IndentifyID))
                {
                    error += "Số chứng minh thư đã được đăng ký \r\n";
                    isValid = false;
                }
                //check password valid
                if (!ValidationHelper.IsValidPassword(registerModel.Password))
                {
                    error += "Mật khẩu khoog hợp lệ \r\n";
                    isValid = false;
                }

                if (isValid)
                {
                    //split infomation to usernfomation
                    UserInformation userInfo = new UserInformation();
                    userInfo._id = ObjectId.GenerateNewId();
                    userInfo.UserName = registerModel.UserName;
                    userInfo.IndentifyID = registerModel.IndentifyID;
                    userInfo.Address = registerModel.Address;
                    userInfo.Phone = registerModel.Phone;

                    //split infomation to account
                    Account account = new Account();
                    account.UserID = userInfo._id.ToString();
                    account.Email = registerModel.Email;
                    account.Password = registerModel.Password;
                    account.IndentifyID = registerModel.IndentifyID;
                    account.IsActivate = Constants.IS_ACTIVATE;
                    account.IsAdmin = Constants.IS_USER;
                    account.IsConfirm = Constants.NOT_CONFIRMED;


                    //add to db
                    AccountDAO.AddAccount(account);
                    UserInformationDAO.AddUserInformation(userInfo);

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ViewBag.Message = error;
                    return View("Register", registerModel);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Account/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Account/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
