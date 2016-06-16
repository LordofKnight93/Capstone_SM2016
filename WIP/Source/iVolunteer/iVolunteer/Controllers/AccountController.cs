using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
using System.IO;

namespace iVolunteer.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Confirm(string userID)
        {
            SQL_Account_DAO accountDAO = new SQL_Account_DAO();
            accountDAO.Set_Confirmation_Status(userID, Constant.IS_CONFIRMED);
            return RedirectToAction("Login","Home");
        }

        public ActionResult ChangeAvatar()
        {
            return View("ImageUpload");
        }
        [HttpPost]
        public ActionResult UploadAvatar()
        {
            HttpPostedFileBase file = Request.Files["fileuploadImage"];
            if (file != null)
            {
                string userID = Session["UserID"].ToString();
                // write your code to save image
                string uploadPath = Server.MapPath("/Image/Avatar/" + userID + ".jpg");
                file.SaveAs(uploadPath);
                try
                {
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    userDAO.Set_AvtImgLink(userID, "/Image/Avatar/" + userID + ".jpg");
                    return RedirectToAction("UserInformationDetail", "User", new { userID = userID });
                }
                catch
                {
                    return View("ImageUpload");
                }

               ;

            }
            else return View("ImageUpload");
            
        }
    }
}