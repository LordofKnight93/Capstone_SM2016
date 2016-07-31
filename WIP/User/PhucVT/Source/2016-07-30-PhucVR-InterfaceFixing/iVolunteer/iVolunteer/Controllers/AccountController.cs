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
using System.Net.Mail;

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

                // get joined project list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Current_Projects(userID);
                // get joined project Info
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectsInformation(listID);

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

                // get joined project list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Joined_Projects(userID);
                // get joined project Info
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectsInformation(listID);

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
                        relationDAO.Delete_Follower(userID, projectID);
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
                {
                    relationDAO.Add_Request(userID, groupID);
                    //SEND join group NOTIFICATION to Group Leader(s)
                    SendJoinGroupNotify(userID, groupID);
                }

                return ActionToGroup(groupID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public bool SendJoinGroupNotify(string userID, string groupID)
        {
            try
            {
                //Get group leader(s)ID 
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                List<string> leadersID = relationDAO.Get_Leaders(groupID);

                //Get SDLink 
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink actor = userDAO.Get_SDLink(userID);
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                SDLink destination = groupDAO.Get_SDLink(groupID);
                //Add Notification item 
                //Create Notifcation Item
                Notification notif = new Notification(actor, Notify.JOIN_GROUP_REQUEST, actor, destination);

                foreach (var leader in leadersID)
                {
                    userDAO.Add_Notification(leader, notif);
                }
                //Connect to NotificationHub
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getJoinGroupRequests(leadersID);
                return true;
            }
            catch
            {
                throw;
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
                {
                    AcceptFriendRequest(otherID);
                    //Send Friend Request Accepted Notification 
                    SendFriendRequestAccepted(userID, otherID);
                }
                else
                {
                    ///check if curent user has sent request or not 
                    if (!relationDAO.Is_Requested(userID, otherID))
                    {
                        relationDAO.Add_Request(userID, otherID);
                        // Send notification
                        SendFriendRequestNotify(userID, otherID);
                    }
                }

                return ActionToOtherUser(otherID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public bool SendFriendRequestNotify(string userID, string otherID)
        {

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            hubContext.Clients.All.getFriendRequests(otherID);
            return true;
        }
        public bool SendFriendRequestAccepted(string userID, string requestID)
        {
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            try
            {
                //Send Friend REquest accepted to requested User
                //Create Notify for request user
                SDLink actor = userDAO.Get_SDLink(userID);
                SDLink target = userDAO.Get_SDLink(requestID);
                Notification notify = new Notification(actor, Notify.FRIEND_REQUEST_ACCEPTED, target);
                userDAO.Add_Notification(requestID, notify);

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getFriendRequestAccepted(requestID);

                return true;
            }
            catch
            {
                throw;
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
                SQL_Project_DAO projectDAO = new SQL_Project_DAO();

                if (!projectDAO.Is_Ongoing(projectID))
                {
                    ViewBag.Ongoing = false;
                    return PartialView("_ActionToProject");
                }
                else ViewBag.Ongoing = true;

                if (Session["UserID"] == null)
                {

                    ViewBag.IsUser = false;
                    return PartialView("_ActionToProject");
                }
                else ViewBag.IsUser = true;

                if (Session["Role"].ToString() == "Admin")
                {

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
                {
                    relationDAO.Add_Join_Request(userID, projectID);
                    //SEND join group NOTIFICATION to Project Leader(s)
                    SendJoinProjectNotify(userID, projectID);
                }

                return ActionToProject(projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public bool SendJoinProjectNotify(string userID, string projectID)
        {
            try
            {
                //Get project leader(s)ID 
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                List<string> leadersID = relationDAO.Get_Leaders(projectID);

                //Get SDLink 
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink actor = userDAO.Get_SDLink(userID);
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                SDLink destination = projectDAO.Get_SDLink(projectID);

                //Create Notifcation Item
                Notification notif = new Notification(actor, Notify.JOIN_PROJECT_REQUEST, actor, destination);

                foreach (var leader in leadersID)
                {
                    userDAO.Add_Notification(leader, notif);
                }

                //Connect to NotificationHub
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getJoinProjectRequests(leadersID);

                return true;
            }
            catch
            {
                throw;
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
                return null;
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

                return null;
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
                return null;
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
                                relationDAO.Delete_Leader(userID, projectID);
                                relationDAO.Delete_Organizer(userID, projectID);
                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                userDAO.Out_Project(userID);
                                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                                projectDAO.Members_Out(projectID, 1);
                                
                            }
                            else
                            {
                                transaction.Dispose();
                                ViewBag.Message = "Quản lý không thể rời hoạt động chỉ có 1 Quản lý.";
                                return PartialView("ErrorMessage");
                            }
                        }
                        else
                        {
                            relationDAO.Delete_Member(userID, projectID);
                            relationDAO.Delete_Organizer(userID, projectID);
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            userDAO.Out_Group(userID);
                            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                            projectDAO.Members_Out(projectID, 1);
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
                return ActionToProject(projectID);
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

                        //Send Friend Request Accepted NOTIFICATION
                        SendFriendRequestAccepted(userID, requestID);

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
        public JsonResult DeclineFriendRequestInNotif(string requestID)
        {

            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return Json(false);
                }

                string userID = Session["UserID"].ToString();

                //delete sql relation
                SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                relationDAO.Delete_Request(requestID, userID);

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
            if (Session["UserID"] == null) return Json(0);
            string userID = Session["UserID"].ToString();
            try
            {
                int count = 0;
                SQL_AcAc_Relation_DAO relation = new SQL_AcAc_Relation_DAO();
                count += relation.Count_Request(userID);
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                count += userDAO.Count_FriendAccepted(userID);

                return Json(count);
            }
            catch
            {
                throw;
            }
        }
        public ActionResult sendActivationEmail(string displayName, string email, string userID)
        {

            MailMessage message = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            message.From = new MailAddress("ivolunteer.noreply@gmail.com");
            message.To.Add(email);
            message.Subject = "Verify your email used on ivolunteer.com.vn";
            message.Body = string.Format("Hi {0}, <br /> Thank you for your registration, please click <a href = \"{1}\" title = \"Activate your account\">here</a> to complete your registration!", displayName, Url.Action("Confirm", "Account", new { userID = userID }, Request.Url.Scheme));

            message.IsBodyHtml = true;
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            client.Credentials = new System.Net.NetworkCredential("ivolunteer.noreply@gmail.com", "iv0lunt##r");
            client.Send(message);
            ViewBag.Message = "Gửi Email xác nhận tài khoản thành công. Mời bạn truy cập Email và làm theo hướng dẫn";
            return View("ErrorMessage");

        }

        public void sendForgotPasswordEmail(string userID, string userName, string email)
        {

            MailMessage message = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            message.From = new MailAddress("account-security-noreply@ivolunteervn.com");
            message.To.Add(email);
            message.Subject = "Reset your password on iVolunteerVN.com";
            message.Body = string.Format("Hi {0}, <br /> We received a request to reset the password for your account. <br /> You can click  <a href = \"{1}\" title = \"Activate your account\">here</a> to reset your password", userName, Url.Action("ForgotPassword", "Account", new { userId = userID }, Request.Url.Scheme));

            message.IsBodyHtml = true;
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            client.Credentials = new System.Net.NetworkCredential("account-security-noreply@ivolunteervn.com", "Passw0rd@123");
            client.Send(message);

        }
        /// <summary>
        /// get friend not in a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult FriendNotInGroup(string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcAc_Relation_DAO acAcDAO = new SQL_AcAc_Relation_DAO();
                var listID = acAcDAO.Get_Friend_Not_In_Group(userID, groupID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.GroupID = groupID;
                return PartialView("_FriendNotInGroup", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        /// <summary>
        /// get friend not in a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult FriendNotInProject(string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcAc_Relation_DAO acAcDAO = new SQL_AcAc_Relation_DAO();
                var listID = acAcDAO.Get_Friend_Not_In_Project(userID, projectID);

                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();
                ViewBag.IsLeader = acPrDAO.Is_Leader(userID, projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_FriendNotInProject", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult SuggestFriends(string[] friendID, string projectID)
        {
            try
            {
                if (friendID == null) return FriendNotInProject(projectID);

                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                relationDAO.Suggest_Users(friendID, projectID);

                ViewBag.Message = "Đễ xuất thành công, cám ơn bạn.";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult InvitedProjects()
        {
            try
            {
                string userID = Session["UserID"].ToString();

                // get joined project list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Invited_Projects(userID);
                // get joined project Info
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectsInformation(listID);

                return PartialView("_InvitedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult AcceptInvitation(string projectID)
        {
            try
            {
                string userID = Session["UserID"].ToString();

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                        relationDAO.Delete_Invite_User(userID, projectID);
                        relationDAO.Add_Member(userID, projectID);

                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        userDAO.Join_Project(userID);

                        Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                        projectDAO.Members_Join(projectID, 1);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }
                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult DeclineInvitation(string projectID)
        {
            try
            {
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Delete_Invite_User(userID, projectID);

                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// get lead group to action to projectID
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult LeadGroups(string projectID)
        {
            try
            {
                string userID = Session["UserID"].ToString();

                // get joined project list
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Lead_Groups(userID);
                // get joined project Info
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

                ViewBag.ProjectID = projectID;

                return PartialView("_LeadGroups", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePassword");
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            try
            {
                if(!ModelState.IsValid) return PartialView("_ChangePassword");

                if (!ValidationHelper.IsValidPassword(changePasswordModel.NewPassword))
                {
                    ViewBag.Message = Error.PASSWORD_INVALID;
                    PartialView("_ChangePassword");
                }

                string userID = Session["UserID"].ToString();

                SQL_Account_DAO accountDAO = new SQL_Account_DAO();
                if(!accountDAO.Is_Password_Match(userID, changePasswordModel.OldPassword))
                {
                    ViewBag.Message = "Mật khẩu cũ không chính xác!";
                    return PartialView("_ChangePassword");
                }

                accountDAO.Set_Password(userID, changePasswordModel.NewPassword);

                ViewBag.Message = "Đổi mật khẩu thành công";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
    }
}