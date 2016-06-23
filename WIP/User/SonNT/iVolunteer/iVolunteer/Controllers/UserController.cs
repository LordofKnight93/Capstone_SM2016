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
    public class UserController : Controller
    {
        public ActionResult Information(string userID)
        {
            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");

            UserInformation userInfo = null;
            try
            {
                SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                if (sqlDAO.IsActivate(userID))
                {
                    Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                    userInfo = mongoDAO.Get_UserInformation(userID);
                }
            }
            catch
            {
                RedirectToAction("FrontPage", "Home");
            }
            return View("_Information",userInfo);
        }
        public ActionResult AvatarCover(string userID )
        {
            SDLink result = new SDLink();
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_SDLink(userID);
            }
            catch
            {
                RedirectToAction("FrontPage", "Home");
            }
            return View("_AvatarCover", result);
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
            if (!ModelState.IsValid) return View();

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