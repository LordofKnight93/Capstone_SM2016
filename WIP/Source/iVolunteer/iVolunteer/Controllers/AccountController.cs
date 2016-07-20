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
using Microsoft.AspNet.SignalR;
using iVolunteer.Hubs;

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
                sqlDAO.Confirmed(userID);
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                mongoDAO.Confirmed(userID);
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

                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Current_Projects(userID);
                // get joined group Info
                Mongo_Project_DAO groupDAO = new Mongo_Project_DAO();
                var result = groupDAO.Get_ProjectsInformation(listID);

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

                // get joined group list
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Joined_Groups(userID);
                // get joined group Info
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

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

                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Joined_Projects(userID);
                // get joined group Info
                Mongo_Project_DAO groupDAO = new Mongo_Project_DAO();
                var result = groupDAO.Get_ProjectsInformation(listID);

                return PartialView("_JoinedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult FollowGroup(string groupID)
        {
            try
            {
                if (Session["UserID"] == null) return RedirectToAction("Login", "Home");
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                relationDAO.Add_Follower(userID, groupID);

                return ActionToGroup(groupID);
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
                relationDAO.Delete_Follower(userID, groupID);

                return ActionToGroup(groupID);
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
                string userID = Session["UserID"].ToString();

                
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                        relationDAO.Add_Follower(userID, projectID);
                        Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                        projectDAO.User_Follow(projectID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                return ActionToProject(projectID);
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
                        relationDAO.Add_Follower(userID, projectID);
                        Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                        projectDAO.User_Unfollow(projectID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                return ActionToProject(projectID);
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
                ViewBag.GroupID = groupID;
                if (Session["UserID"] == null) 
                {

                    ViewBag.IsUser = false;
                    return PartialView("_ActionToGroup");
                }
                else ViewBag.IsUser = true;

                if (Session["Role"].ToString() == "Admin")
                {
                    SQL_Group_DAO groupDAO = new SQL_Group_DAO();

                    ViewBag.IsAdmin = true;
                    ViewBag.IsActivate = groupDAO.IsActivate(groupID);
                    return PartialView("_ActionToGroup");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                if (relationDAO.Is_Joined(userID, groupID) || relationDAO.Is_Leader(userID, groupID))
                    ViewBag.IsJoined = true;

                if (relationDAO.Is_Requested(userID, groupID))
                    ViewBag.IsRequested = true;

                if (relationDAO.Is_Follower(userID, groupID))
                    ViewBag.IsFollowing = true;

                if (relationDAO.Is_Reported(userID, groupID))
                    ViewBag.IsReported = true;

                return PartialView("_ActionToGroup");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult JoinGroupRequest(string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();

                if(!relationDAO.Is_Requested(userID, groupID))
                    relationDAO.Add_Request(userID, groupID);

                return ActionToGroup(groupID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult CancelJoinGroupRequest(string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                //delete sql relation
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                relationDAO.Delelte_Request(userID, groupID);

                return ActionToGroup(groupID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        [ChildActionOnly]
        public ActionResult ActionToOtherUser(string otherID)
        {
            try
            {
                ViewBag.OtherID = otherID;

                //check if is user is login
                if (Session["UserID"] == null)
                {

                    ViewBag.IsUser = false;
                    return PartialView("_ActionToOtherUser");
                }
                else ViewBag.IsUser = true;

                if (Session["Role"].ToString() == "Admin")
                {
                    SQL_Account_DAO accountDAO = new SQL_Account_DAO();

                    ViewBag.IsAdmin = true;
                    ViewBag.IsActivate = accountDAO.IsActivate(otherID);
                    return PartialView("_ActionToOtherUser");
                }

                SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();

                string userID = Session["UserID"].ToString();
                //if same as currnet user
                if (userID == otherID)
                {
                    ViewBag.IsUser = false;
                    return PartialView("_ActionToOtherUser");
                }

                if (Session["Role"].ToString() == "Admin")
                {
                    ViewBag.IsAdmin = true;
                    return PartialView("_ActionToOtherUser");
                }


                //check if is friend
                if (relationDAO.Is_Friend(userID, otherID))
                {
                    ViewBag.IsFriend = true;
                }
                //check if user send request
                if (relationDAO.Is_Requested(userID, otherID))
                {
                    ViewBag.IsRequested = true;
                }
                if (relationDAO.Is_Reported(userID, otherID))
                    ViewBag.IsReported = true;

                return PartialView("_ActionToOtherUser");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult MutalFriends(string otherID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return null;
                }

                string userID = Session["UserID"].ToString();
                if (userID == otherID) return null;

                SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                var listID =  relationDAO.Get_MutalFriend(userID, otherID);
                
                ViewBag.Message = " - " + listID.Count + " bạn chung ";
                //Mongo_User_DAO userDAO = new Mongo_User_DAO();
                //var result = userDAO.Get_AccountsInformation(listID);

                return PartialView("_MutalFriends");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult FriendRequest(string otherID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                //check if target send request or not
                if (relationDAO.Is_Requested(otherID, userID))
                    AcceptFriendRequest(otherID);
                else
                {
                    ///check if curent user has sent request or not 
                    if (!relationDAO.Is_Requested(userID, otherID))
                        relationDAO.Add_Request(userID, otherID);

                    // Send notification
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    hubContext.Clients.All.getFriendRequests(otherID);
                }

                return ActionToOtherUser(otherID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult CancelFriendRequest(string otherID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                relationDAO.Delete_Request(userID, otherID);

                return ActionToOtherUser(otherID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        //child acton return action with project
        [ChildActionOnly]
        public ActionResult ActionToProject(string projectID)
        {
            try
            {
                ViewBag.ProjectID = projectID;
                if (Session["UserID"] == null)
                {

                    ViewBag.IsUser = false;
                    return PartialView("_ActionToProject");
                }
                else ViewBag.IsUser = true;

                if (Session["Role"].ToString() == "Admin")
                {
                    SQL_Project_DAO projectDAO = new SQL_Project_DAO();

                    ViewBag.IsAdmin = true;
                    ViewBag.IsActivate = projectDAO.IsActivate(projectID);
                    return PartialView("_ActionToProject");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if (relationDAO.Is_Joined(userID, projectID))
                    ViewBag.IsJoined = true;

                if (relationDAO.Is_Join_Requested(userID, projectID))
                    ViewBag.IsJoinRequested = true;

                if (relationDAO.Is_Sponsor(userID, projectID))
                    ViewBag.IsSponsored = true;

                if (relationDAO.Is_Sponsor_Requested(userID, projectID))
                    ViewBag.IsSponsorRequested = true;

                if (relationDAO.Is_Follower(userID, projectID))
                    ViewBag.IsFollowing = true;

                if (relationDAO.Is_Reported(userID, projectID))
                    ViewBag.IsReported = true;

                return PartialView("_ActionToProject");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult JoinProjectRequest(string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (!relationDAO.Is_Join_Requested(userID, projectID))
                    relationDAO.Add_Join_Request(userID, projectID);

                return ActionToProject(projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult CancelJoinProjectRequest(string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                //delete sql relation
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                relationDAO.Delete_Join_Request(userID, projectID);

                return ActionToProject(projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult SponsorProjectRequest(string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (!relationDAO.Is_Sponsor_Requested(userID, projectID))
                    relationDAO.Add_Sponsor_Request(userID, projectID);

                return ActionToProject(projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult CancelSponsorProjectRequest(string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                //delete sql relation
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                relationDAO.Delete_Sponsor_Request(userID, projectID);

                return ActionToProject(projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult AcceptFriendRequest(string requestID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //delete sql relation
                        SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                        relationDAO.Accept_Request(userID, requestID);

                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        SDLink first = userDAO.Get_SDLink(userID);
                        SDLink second = userDAO.Get_SDLink(requestID);
                        //userDAO.Add_Friend(userID);
                        //userDAO.Add_Friend(requestID);
                        userDAO.Add_Friend_To_List(userID, second);
                        userDAO.Add_Friend_To_List(requestID, first);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }

                }
                return FriendRequestList();
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult DeclineFriendRequest(string requestID)
        {

            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                //delete sql relation
                SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                relationDAO.Delete_Request(requestID, userID);

                return FriendRequestList();
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult FriendRequestList()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                //delete sql relation
                SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                var listID =  relationDAO.Get_Requests(userID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                return PartialView("_FriendRequestList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult DeleteFriend(string friendID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //delete sql relation
                        SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                        relationDAO.Delete_Friend(friendID, userID);
                        relationDAO.Delete_Friend(userID, friendID);

                        //Delete Friend in FriendList
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        userDAO.Delete_Friend(userID, friendID);
                        userDAO.Delete_Friend(friendID, userID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }

                }
                return FriendRequestList();
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult GroupResign(string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //delete sql relation
                        SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                        if (relationDAO.Is_Leader(userID, groupID))
                        {
                            if (relationDAO.Is_More_Than_One_Leader(groupID))
                            {
                                if (relationDAO.Delete_Leader(userID, groupID))
                                {
                                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                    userDAO.Out_Group(userID);
                                    Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                                    groupDAO.Member_Out(groupID);
                                }
                            }
                            else
                            {
                                transaction.Dispose();
                                ViewBag.Message = "Nhóm trưởng không thể rời nhóm chỉ có 1 nhóm trưởng.";
                                return PartialView("ErrorMessage");
                            }
                        }
                        else
                        {
                            if (relationDAO.Delete_Member(userID, groupID))
                            {
                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                userDAO.Out_Group(userID);
                                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                                groupDAO.Member_Out(groupID);
                            }
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
                return ActionToGroup(groupID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult ProjectResign(string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //delete sql relation
                        SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                        if (relationDAO.Is_Leader(userID, projectID))
                        {
                            if (relationDAO.Is_More_Than_One_Leader(projectID))
                            {
                                if (relationDAO.Delete_Leader(userID, projectID))
                                {
                                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                    userDAO.Out_Project(userID);
                                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                                    projectDAO.Member_Out(projectID);
                                }
                            }
                            else
                            {
                                transaction.Dispose();
                                ViewBag.Message = "Nhóm trưởng không thể rời nhóm chỉ có 1 nhóm trưởng.";
                                return PartialView("ErrorMessage");
                            }
                        }
                        else
                        {
                            if (relationDAO.Delete_Member(userID, projectID))
                            {
                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                userDAO.Out_Group(userID);
                                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                                projectDAO.Member_Out(projectID);
                            }
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
                return ActionToGroup(projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public JsonResult AcceptFriendRequestInNotif(string requestID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return Json(false);

                }
                string userID = Session["UserID"].ToString();
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                        relationDAO.Accept_Request(userID, requestID);
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        SDLink first = userDAO.Get_SDLink(userID);
                        SDLink second = userDAO.Get_SDLink(requestID);
                        userDAO.Add_Friend_To_List(userID, second);
                        userDAO.Add_Friend_To_List(requestID, first);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return Json(false);
                    }
                }
                return Json(true);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return Json(false);
            }
        }
        public JsonResult GetNumberOfFriendRequest()
        {
            string userID = Session["UserID"].ToString();
            try
            {
                SQL_AcAc_Relation_DAO relation = new SQL_AcAc_Relation_DAO();
                int count = relation.Count_Request(userID);

                return Json(count);
            }
            catch
            {
                throw;
            }
        }
    }
}