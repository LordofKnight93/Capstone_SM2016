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
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult UserInformationDetail(string userID)
        {
            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");
            UserInformation userInfo = new UserInformation();
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                userInfo = userDAO.Get_UserInformation(userID);
            }
            catch
            {
                RedirectToAction("FrontPage", "Home");
            }
            return View(userInfo);
        }

        public ActionResult ImageCover(string userID)
        {
            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");
            AccountInformation userInfo = new AccountInformation();
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                userInfo = userDAO.Get_AccountInformation(userID);
            }
            catch
            {
                RedirectToAction("FrontPage", "Home");
            }
            return PartialView(userInfo);
        }
        [HttpGet]
        public ActionResult UpdateInformation(string userID)
        {
            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var userInfo = userDAO.Get_UserInformation(userID);
                return View(userInfo);
            }
            catch
            {
                return RedirectToAction("Newfeed", "Home");
            }
        }
        public ActionResult UpdateInformation(string userID, UserInformation userInfo)
        {
            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");
            //get current info
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            var curInfo = userDAO.Get_UserInformation(userID);
            //set new info to current info
            curInfo.Address = userInfo.Address;
            curInfo.Phone = userInfo.Phone;

            userDAO.Update_UserInformation(userID, curInfo);
            return RedirectToAction("UserInformationDetail","User", userID);
        }

    }
}