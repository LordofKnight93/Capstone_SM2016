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
        /// <summary>
        /// get user information
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult Information(string userID)
        {
            UserInformation result = null;
            try
            {
                SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                if (sqlDAO.IsActivate(userID))
                {
                    Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                    result = mongoDAO.Get_UserInformation(userID);
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return PartialView("_Information",result);
        }
        /// <summary>
        /// get user SDLink for avatarcover view
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult AvatarCover(string userID)
        {
            SDLink result = null;
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_SDLink(userID);
                if (Session["UserID"] != null && userID == Session["UserID"].ToString()) ViewBag.CanChange = "true";
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return PartialView("_AvatarCover", result);
        }
        /// <summary>
        /// get user activity history in system
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult ActivityHistory(string userID)
        {
            ActivityInformation result = null;
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_ActivityHistory(userID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return PartialView("_ActivityHistory", result);
        }
        /// <summary>
        /// get user joined project
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult JoinedProjects(string userID)
        {
            ActivityInformation result = null;
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_ActivityHistory(userID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return PartialView("_JoinedProjects", result);
        }
        /// <summary>
        /// get user current project
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult CurrentProjects(string userID)
        {
            List<SDLink> result = null;
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_CurrentProjects(userID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return PartialView("_CurrentProjects", result);
        }
        /// <summary>
        /// get user joined groups
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult JoinedGroups(string userID)
        {
            List<SDLink> result = null;
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_JoinedGroups(userID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return PartialView("_JoinedGroups", result);
        }

        [HttpGet]
        public ActionResult UpdateInformation(string userID)
        {
            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");
            UserInformation userInfo = new UserInformation();
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                userInfo = userDAO.Get_UserInformation(userID);
                return RedirectToAction("UserProfile", "User", new { userID = userID });
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [HttpPost]
        public ActionResult UpdateInformation(string userID, UserInformation userInfo)
        {
            if (!ModelState.IsValid) return View();

            if (String.IsNullOrEmpty(userID)) return RedirectToAction("FrontPage", "Home");
            //get current info
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var curInfo = userDAO.Get_UserInformation(userID);
                //set new info to current info
                curInfo.Address = userInfo.Address;
                curInfo.Phone = userInfo.Phone;

                userDAO.Update_UserInformation(userID, curInfo);
                return RedirectToAction("UserProfile", "User", new { userID = userID });
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// user profile page
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult UserProfile(string userID)
        {
            SDLink result = null; 
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                result = userDAO.Get_SDLink(userID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return View("UserProfile", result);
        }

        public ActionResult ChangeAvatar()
        {
            ViewBag.Option = "UserAvatar";
            return View("_ImageUpload");
        }
        [HttpPost]
        public ActionResult UploadAvatar()
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                string userID = Session["UserID"].ToString();
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/User/Avatar/" + userID + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("UserProfile", "User", new { userID = userID });

            }
            else return View("_ImageUpload");

        }
        public ActionResult ChangeCover()
        {
            ViewBag.Option = "UserCover";
            return View("_ImageUpload");
        }
        [HttpPost]
        public ActionResult UploadCover()
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                string userID = Session["UserID"].ToString();
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/User/Cover/" + userID + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("UserProfile", "User", new { userID = userID });

            }
            else return View("_ImageUpload");
        }
        /// <summary>
        /// send join request to group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult GroupJoinRequest(string groupID)
        {
            //craete creator
            SDLink creator = new SDLink();
            creator.ID = Session["UserID"].ToString();
            creator.DisplayName = Session["DisplayName"].ToString();
            creator.Handler = Handler.USER;

            try
            {
                Mongo_Group_DAO  groupDAO = new Mongo_Group_DAO();
                SDLink group = groupDAO.Get_SDLink(groupID);

                RequestItem request = new RequestItem(creator, RequestType.JOIN_REQUEST, group);
                if(groupDAO.Is_Request_Exist(groupID, request))
                {
                    ViewBag.Message = " Bạn đã gửi yêu cầu rồi.";
                }
                else
                {
                    groupDAO.Add_Request(groupID, request);
                    ViewBag.Message = " Gửi yêu cầu thành công.";
                }

                return View("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// send idividual join request to project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ProjectJoinRequest(string projectID)
        {
            //craete creator
            SDLink creator = new SDLink();
            creator.ID = Session["UserID"].ToString();
            creator.DisplayName = Session["DisplayName"].ToString();
            creator.Handler = Handler.USER;

            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                SDLink project = projectDAO.Get_SDLink(projectID);

                RequestItem request = new RequestItem(creator, RequestType.JOIN_REQUEST, project);

                if (projectDAO.Is_JoinRequest_Exist(projectID, request))
                {
                    ViewBag.Message = " Bạn đã gửi yêu cầu rồi.";
                }
                else
                {
                    projectDAO.Add_JoinRequest(projectID, request);
                    ViewBag.Message = " Gửi yêu cầu thành công.";
                }

                return View("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// send group join request to project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult ProjectJoinRequest(string projectID, string groupID)
        {
            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                SDLink project = projectDAO.Get_SDLink(projectID);
                SDLink group = groupDAO.Get_SDLink(groupID);

                RequestItem request = new RequestItem(group, RequestType.JOIN_REQUEST, project);

                if (projectDAO.Is_JoinRequest_Exist(projectID, request))
                {
                    ViewBag.Message = " Bạn đã gửi yêu cầu rồi.";
                }
                else
                {
                    projectDAO.Add_JoinRequest(projectID, request);
                    ViewBag.Message = " Gửi yêu cầu thành công.";
                }

                return View("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        [ChildActionOnly]
        public ActionResult ActionToGroup(string groupID)
        {
            try
            {
                //echk session
                if (Session["UserID"] == null)
                {
                    ViewBag.IsJoined = "true";
                    ViewBag.IsFollowing = "true";
                    ViewBag.GroupID = groupID;
                    return PartialView("_ActionToGroup");
                }

                string userID = Session["UserID"].ToString();
                SQL_AcGr_Relation_DAO sqlDAO = new SQL_AcGr_Relation_DAO();

                int relation = sqlDAO.Get_Specific_Relation(userID, groupID);
                switch (relation)
                {
                    case 0:
                        ViewBag.IsJoined = "false";
                        ViewBag.IsFollowing = "false";
                        ViewBag.GroupID = groupID;
                        break;
                    case Relation.FOLLOW_RELATION:
                        ViewBag.IsJoined = "false";
                        ViewBag.IsFollowing = "true";
                        ViewBag.GroupID = groupID;
                        break;
                    default:
                        ViewBag.IsJoined = "true";
                        ViewBag.IsFollowing = "true";
                        ViewBag.GroupID = groupID;
                        break;
                }

                return PartialView("_ActionToGroup");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        //[ChildActionOnly]
        //public ActionResult ActionToOtherUser( string otherID)
        //{
        //    try
        //    {
        //        // chekc if session null
        //        if (Session["UserID"] == null)
        //        {
        //            ViewBag.IsFriend = "true";
        //            ViewBag.OtherID = otherID;
        //            return PartialView("_ActionToGroup");
        //        }

        //        //get current user ID
        //        string userID = Session["UserID"].ToString();
        //        SQL_Friendship_DAO sqlDAO = new SQL_Friendship_DAO();
        //        //get relation
        //        bool relation = sqlDAO.Is_Friend(userID, otherID);
        //        if (relation)
        //            ViewBag.IsFriend = "true";
        //        else ViewBag.IsFriend = "false";

        //        ViewBag.OtherID = otherID;
        //        return PartialView("_ActionToOtherUser");
        //    }
        //    catch
        //    {
        //        ViewBag.Message = Error.UNEXPECT_ERROR;
        //        return PartialView("ErrorMessage");
        //    }
        //}

        public ActionResult AccountSearch(string name, int page)
        {
            List<AccountInformation> result = new List<AccountInformation>();
            if (page <= 0)
            {
                page = 1;
            }

            try
            {
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                //search depend on role
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = mongoDAO.Search_Account(name, 10 * (page - 1), 10);
                else
                    result = mongoDAO.Search_Account(name, Status.IS_ACTIVATE, 10 * (page - 1), 10);

                    ViewBag.CurrentPage = page;
                    return PartialView("_AccountResult", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
    }
}