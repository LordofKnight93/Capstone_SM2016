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
using iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.DAL.SQL;
using iVolunteer.DAL.MongoDB;
using iVolunteer.Common;
using System.IO;

namespace iVolunteer.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// confirmation for user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// child action, craete left navigation panel
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult NavigationPanel()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                var result = mongoDAO.Get_AccountInformation(userID);
                return PartialView("_NavigationPanel", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// child action, get current projects for navigation panel
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult CurrentProjects()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_CurrentProjects(userID);
                return PartialView("_CurrentProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// child action, get joined groups for navigation panel
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult JoinedGroups()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_JoinedGroups(userID);
                return PartialView("_JoinedGroups", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// child action, get joined projects for navigation panel
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult JoinedProjects()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_JoinedProjects(userID);
                return PartialView("_JoinedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult RequestList()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_JoinedProjects(userID);
                return PartialView("_JoinedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult DeniedRequest(string requestID)
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                //get request
                var request = userDAO.Get_Request(userID, requestID);
                //transaction
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        if (request.Type == RequestType.FRIEND_REQUEST)
                        {
                            SQL_Friendship_DAO relationDAO = new SQL_Friendship_DAO();
                            //delete relation
                            relationDAO.Delete_Specific_Relation(request.Actor.ID, userID);
                        }

                        if (request.Type == RequestType.INVITE_TO)
                        {
                            SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                            //delete relation
                            relationDAO.Delete_Specific_Relation(userID, request.Destination.ID, Relation.MEMBER_RELATION);
                        }
                        //delete request
                        userDAO.Delete_Request(userID, requestID);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                return RedirectToAction("RequestList", "Account");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult AcceptRequest(string requestID)
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                //get request
                var request = userDAO.Get_Request(userID, requestID);
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        if (request.Type == RequestType.FRIEND_REQUEST)
                        {
                            SQL_Friendship_DAO relationDAO = new SQL_Friendship_DAO();
                             //update relation
                             relationDAO.Update_Relation(request.Actor.ID, userID, Status.ACCEPTED);
                            //add new relation
                            SQL_Friendship relation = new SQL_Friendship();
                            relation.UserID = userID;
                            relation.FriendID = request.Actor.ID;
                            relation.Status = Status.ACCEPTED;

                            relationDAO.Add_Relation(relation);

                             //add friend to list
                            userDAO.Add_Friend(request.Actor.ID, request.Destination);
                            userDAO.Add_Friend(request.Destination.ID, request.Actor);
                        }

                        if (request.Type == RequestType.INVITE_TO)
                        {
                             SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                             //update relation
                             relationDAO.Update_Relation(userID, request.Destination.ID, Relation.MEMBER_RELATION, Status.ACCEPTED);
                             //add to mongBD
                             userDAO.Add_CurrentProject(userID, request.Destination);
                             userDAO.Add_JoinedProject_ActivityHistory(userID, request.Destination);
                        }

                        //delete request
                        userDAO.Delete_Request(userID, requestID);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                return RedirectToAction("RequestList", "Account");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult FriendRequest(string otherID)
        {
            try
            {
                if (Session["UserID"] == null) return RedirectToAction("Login", "Home");
                //create actor
                SDLink actor = new SDLink();
                actor.ID = Session["UserID"].ToString();
                actor.DisplayName = Session["DisplayName"].ToString();
                actor.Handler = Handler.USER;


                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                //create destination
                SDLink destination = userDAO.Get_SDLink(otherID);
                //create request
                RequestItem request = new RequestItem(actor, RequestType.FRIEND_REQUEST, destination);
                //write to DB
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_Friendship_DAO relationDAO = new SQL_Friendship_DAO();

                        if(relationDAO.Is_Be_Requested(actor.ID, otherID))
                        {
                            // create sql relation
                            SQL_Friendship relation1 = new SQL_Friendship();
                            relation1.UserID = actor.ID;
                            relation1.FriendID = otherID;
                            relation1.Status = Status.ACCEPTED;

                            SQL_Friendship relation2 = new SQL_Friendship();
                            relation2.UserID = otherID;
                            relation2.FriendID = actor.ID;
                            relation2.Status = Status.ACCEPTED;

                            relationDAO.Add_Relation(relation1);
                            relationDAO.Add_Relation(relation2);

                            userDAO.Add_Friend(actor.ID, destination);
                            userDAO.Add_Friend(destination.ID, actor);
                            //delete request
                            userDAO.Cancel_Request(otherID, actor.ID);

                            ViewBag.Message = " Kết bạn thành công.";
                        }
                        else
                        {
                            //create pending 1 way relation
                            SQL_Friendship relation = new SQL_Friendship();
                            relation.UserID = actor.ID;
                            relation.FriendID = otherID;
                            relation.Status = Status.PENDING;

                            relationDAO.Add_Relation(relation);
                            userDAO.Add_Request(otherID, request);
                            ViewBag.Message = " Gửi lời mời kết bạn thành công.";
                        }


                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult CancelFriendRequest(string otherID)
        {
            try
            {
                if (Session["UserID"] == null) return RedirectToAction("Login", "Home");
                string userID = Session["UserID"].ToString();

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_Friendship_DAO relationDAO = new SQL_Friendship_DAO();
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();

                        relationDAO.Delete_Specific_Relation(userID, otherID);

                        userDAO.Cancel_Request(userID, otherID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }
                ViewBag.Message = "Hủy yêu cầu kết bạn thành công.";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult FollowGroup(string groupID)
        {
            try
            {
                if (Session["UserID"] == null) return RedirectToAction("Login", "Home");
                ///create relation
                SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                relation.UserID = Session["UserID"].ToString();
                relation.GroupID = groupID;
                relation.Relation = Relation.FOLLOW_RELATION;
                relation.Status = Status.ACCEPTED;

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                relationDAO.Add_Relation(relation);

                ViewBag.Message = " Bạn đã theo dõi hoạt động công khai của nhóm này. ";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult UnfollowGroup(string groupID)
        {
            try
            {
                if (Session["UserID"] == null) return RedirectToAction("Login", "Home");
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                relationDAO.Delete_Specific_Relation(userID, groupID, Relation.FOLLOW_RELATION);

                ViewBag.Message = " Bạn đã ngừng theo dõi hoạt động công khai của nhóm này. ";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult FollowProject(string projectID)
        {
            try
            {
                if (Session["UserID"] == null) return RedirectToAction("Login", "Home");
                //create actor
                SDLink actor = new SDLink();
                actor.ID = Session["UserID"].ToString();
                actor.DisplayName = Session["DisplayName"].ToString();
                actor.Handler = Handler.USER;

                ///create relation
                SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                relation.UserID = Session["UserID"].ToString();
                relation.ProjectID = projectID;
                relation.Relation = Relation.FOLLOW_RELATION;
                relation.Status = Status.ACCEPTED;
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                        relationDAO.Add_Relation(relation);
                        Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                        projectDAO.Add_Follower(projectID, actor);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                ViewBag.Message = " Bạn đã theo dõi hoạt động công khai của sự kiện này. ";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult UnfollowProject(string projectID)
        {
            try
            {
                if (Session["UserID"] == null) return RedirectToAction("Login", "Home");
                string userID = Session["UserID"].ToString();

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                        relationDAO.Delete_Specific_Relation(userID, projectID, Relation.FOLLOW_RELATION);

                        Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                        projectDAO.Delete_Follower(projectID, userID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                ViewBag.Message = " Bạn đã ngừng theo dõi hoạt động công khai của sự kiện này. ";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        //child acton return action with group
        [ChildActionOnly]
        public ActionResult ActionToGroup(string groupID)
        {
            try
            {
                if (Session["UserID"] == null) 
                {

                    ViewBag.IsUser = false;
                    return PartialView("_ActionToGroup");
                }
                else ViewBag.IsUser = true;

                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                if (relationDAO.Is_Member(userID, groupID) || relationDAO.Is_Leader(userID, groupID))
                    ViewBag.IsJoined = true;

                if (relationDAO.Is_Requested(userID, groupID))
                    ViewBag.IsRequested = true;

                if (relationDAO.Is_Following(userID, groupID))
                    ViewBag.IsFollowing = true;
                ViewBag.GroupID = groupID;

                //Check if report has been sent
                SQL_AcGr_Relation_DAO reportDAO = new SQL_AcGr_Relation_DAO();
                if (reportDAO.IsSentReport(userID, groupID))
                    ViewBag.IsSentReport = true;

                return PartialView("_ActionToGroup");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult GroupJoinRequest(string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }
                //create actor
                SDLink actor = new SDLink();
                actor.ID = Session["UserID"].ToString();
                actor.DisplayName = Session["DisplayName"].ToString();
                actor.Handler = Handler.USER;

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();

                if(relationDAO.Is_Requested(actor.ID, groupID))
                {
                    ViewBag.Message = "Bạn đã gửi yêu cầu gia nhập nhóm này rồi.";
                    return PartialView("ErrorMessage");
                }

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                //get group SDLink
                SDLink group = groupDAO.Get_SDLink(groupID);

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //create sql relation
                        SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                        relation.UserID = actor.ID;
                        relation.GroupID = groupID;
                        relation.Relation = Relation.MEMBER_RELATION;
                        relation.Status = Status.PENDING;

                        relationDAO.Add_Relation(relation);

                        //create mông request
                        RequestItem request = new RequestItem(actor, RequestType.JOIN_REQUEST, group);

                        groupDAO.Add_Request(groupID, request);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                ViewBag.Message = "Gửi yêu cầu thành công.";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult CancelGroupJoinRequest(string groupID)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            string userID = Session["UserID"].ToString();

            using (var transaction = new TransactionScope())
            {
                try
                {
                    //delete sql relation
                    SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                    relationDAO.Delete_Specific_Relation(userID, groupID, Relation.MEMBER_RELATION);

                    //delete mongo request
                    Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                    groupDAO.Cancel_Request(userID, groupID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }
            }

            ViewBag.Message = "Hủy yêu cầu thành công.";
            return PartialView("ErrorMessage");
        }
        [ChildActionOnly]
        public ActionResult ActionToOtherUser(string otherID)
        {
            try
            {
                //check if is user is login
                if (Session["UserID"] == null)
                {

                    ViewBag.IsUser = false;
                    return PartialView("_ActionToOtherUser");
                }
                else ViewBag.IsUser = true;

                string userID = Session["UserID"].ToString();
                //if same as currnet user
                if (userID == otherID)
                {

                    ViewBag.IsUser = false;
                    return PartialView("_ActionToOtherUser");
                }
                //Check if report has been sent
                SQL_AcAc_Relation_DAO reportDAO = new SQL_AcAc_Relation_DAO();
                if (reportDAO.IsSentReport(userID, otherID))
                    ViewBag.IsSentReport = true;

                SQL_Friendship_DAO relationDAO = new SQL_Friendship_DAO();
                //check if is friend
                if (relationDAO.Is_Friend(userID, otherID))
                {
                    ViewBag.IsFriend = true;
                    return PartialView("_ActionToOtherUser");
                }
                else ViewBag.IsFriend = false;
                //check if user send request
                if (relationDAO.Is_Requested(userID, otherID))
                {
                    ViewBag.IsRequested = true;
                    return PartialView("_ActionToOtherUser");
                }
                else ViewBag.IsRequested = false;

                ViewBag.OtherID = otherID;
                return PartialView("_ActionToOtherUser");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
    }
}