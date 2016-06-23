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
        [HttpPost]
        public ActionResult Confirm(string userID)
        {
            SQL_Account_DAO accountDAO = new SQL_Account_DAO();
            accountDAO.Set_Confirmation_Status(userID, Status.IS_CONFIRMED);
            return RedirectToAction("Login","Home");
        }
        
        public ActionResult ChangeAvatar()
        {
            ViewBag.Option = "UserAvatar";
            return View("_ImageUpload");
        }
        [HttpPost]
        public ActionResult UploadAvatar()
        {
            HttpPostedFileBase file = Request.Files["fileuploadImage"];
            if (file != null)
            {
                string userID = Session["UserID"].ToString();
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/User/Avatar/" + userID + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("InformationDetail", "User", new { userID = userID });

            }
            else return View("_ImageUpload");
            
        }
        [HttpPost]
        public ActionResult UploadCover()
        {
            HttpPostedFileBase file = Request.Files["fileuploadImage"];
            if (file != null)
            {
                string userID = Session["UserID"].ToString();
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/User/Cover/" + userID + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("InformationDetail", "User", new { userID = userID });

            }
            else return View("_ImageUpload");

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
                    return View();
                }
            }
            return RedirectToAction("InformationDetail", "User", new { userID = userID });
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}