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
using System.Threading.Tasks;
using System.Net;
using System.Web.Helpers;

namespace iVolunteer.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// confirmation for user
        /// ユーザーのアカウントを確認
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
            //return RedirectToAction("Login", "Home");
            ViewBag.Message = "Tải khoản của bạn đã được kích hoạt thành công. Hãy nhấn vào 'Đăng nhập hoặc đăng kí' để đăng nhập!";
            ViewBag.IsActivated = true;
            return View("_NotifyMessage");
            //return RedirectToAction("Newfeed", "Home");
        }
        /// <summary>
        /// child action, create left navigation panel
        /// 左のナヴィゲーションパーネルを表示
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
        /// ナヴィゲーションパーネルにある現在の参加プロジェクトを表示
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult CurrentProjects()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                // get current project list
                // 現在の参加プロジェクトのリストを取得
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Current_Projects(userID);
                // get current project Info
                //そのプロジェクトの情報を取得
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
        /// ナヴィゲーションパーネルにある現在の参加グループを表示
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult JoinedGroups()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                // get joined group list
                //参加したグループのリストを取得
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Joined_Groups(userID);
                // get joined group Info
                //そのグループの情報
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
        /// ナヴィゲーションパーネルにある過去の参加プロジェクトを表示
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult JoinedProjects()
        {
            try
            {
                string userID = Session["UserID"].ToString();

                // get joined project list
                // 過去参加したプロジェクトのリストを取得
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Joined_Projects(userID);
                // get joined project Info
                //そのプロジェクトの情報を取得
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
        /// <summary>
        /// グループをフォロー
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// <summary>
        /// グループのフォローを辞める
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// <summary>
        /// プロジェクトをフォロー
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// <summary>
        /// プロジェクトのフォローをやめる
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// <summary>
        /// グループへの操作
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
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

                ViewBag.IsJoined = relationDAO.Is_Joined(userID, groupID);
                ViewBag.IsRequested = relationDAO.Is_Requested(userID, groupID);
                ViewBag.IsFollowing = relationDAO.Is_Follower(userID, groupID);
                ViewBag.IsReported = relationDAO.Is_Reported(userID, groupID);

                return PartialView("_ActionToGroup");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// グループの参加を要求
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [HttpPost]
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
                    //グループのリーダー全員へ通知を送る
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
        /// <summary>
        /// グループへの参加要求を通知
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool SendJoinGroupNotify(string userID, string groupID)
        {
            try
            {
                //Get group leader(s)ID 
                //グループリーダー全員のIDを取得
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                List<string> leadersID = relationDAO.Get_Leaders(groupID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink actor = userDAO.Get_SDLink(userID);
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                SDLink destination = groupDAO.Get_SDLink(groupID);
                
                //Add Notifcation Item
                //通知を作成
                Notification notif = new Notification(actor, Notify.JOIN_GROUP_REQUEST, actor, destination);

                foreach (var leader in leadersID)
                {
                    userDAO.Add_Notification(leader, notif);
                }
                //Connect to NotificationHub
                //通知ハーブに接続
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getJoinGroupRequests(leadersID);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// グループの参加要求をキャンセル
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [HttpPost]
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

                using (var trans = new TransactionScope())
                {
                    try
                    {
                        //delete sql relation
                        SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                        relationDAO.Delelte_Request(userID, groupID);

                        //Delete Group's leaders join group request notification
                        List<string> leaders = relationDAO.Get_Leaders(groupID);
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        var notifyID = userDAO.Get_JoinGroup_NotifyID(leaders[0], userID, groupID);
                        if (notifyID != null)
                        {
                            foreach (var leader in leaders)
                            {
                                userDAO.Set_Notification_IsSeen(leader, notifyID);
                            }
                        }
                        trans.Complete();
                    }
                    catch
                    {
                        trans.Dispose();
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
        /// <summary>
        /// 他人への操作
        /// </summary>
        /// <param name="otherID"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ActionToOtherUser(string otherID)
        {
            try
            {
                ViewBag.OtherID = otherID;

                //check if is user is login
                //ユーザーがロギングかを判定
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
                //同じユーザーかを判定
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
                //すでに友達関係かを判定
                ViewBag.IsFriend = relationDAO.Is_Friend(userID, otherID);
                //check if user send request
                //すでに友達申請をしたかを判定
                ViewBag.IsRequested = relationDAO.Is_Requested(userID, otherID);

                ViewBag.IsReported = relationDAO.Is_Reported(userID, otherID);

                return PartialView("_ActionToOtherUser");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 共通の友人を取得
        /// </summary>
        /// <param name="otherID"></param>
        /// <returns></returns>
        [ChildActionOnly]
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
        /// <summary>
        /// 友達申請を
        /// </summary>
        /// <param name="otherID"></param>
        /// <returns></returns>
        [HttpPost]
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
                //相手がすでに申請したかを判定
                if (relationDAO.Is_Requested(otherID, userID))
                {
                    AcceptFriendRequest(otherID);
                    //Send Friend Request Accepted Notification 
                    //友達申請を承認
                    SendFriendRequestAccepted(userID, otherID);
                }
                else
                {
                    //check if curent user has sent request or not 
                    //自分がすでに西進したかを判定
                    if (!relationDAO.Is_Requested(userID, otherID))
                    {
                        relationDAO.Add_Request(userID, otherID);
                        // Send notification
                        //申請の通知を放送
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
        /// <summary>
        /// 友達申請の通知を放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool SendFriendRequestNotify(string userID, string otherID)
        {

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            hubContext.Clients.All.getFriendRequests(otherID);
            return true;
        }
        /// <summary>
        /// 友達承認の通知を放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool SendFriendRequestAccepted(string userID, string requestID)
        {
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            try
            {
                //Create Notify for request user
                //相手への通知を作成
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
        /// <summary>
        /// 友達精神をキャンセル
        /// </summary>
        /// <param name="otherID"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// <summary>
        /// プロジェクトへの操作
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
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

                ViewBag.IsRecruiting = projectDAO.IsRecruiting(projectID);

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

                ViewBag.IsJoined = relationDAO.Is_Joined(userID, projectID);
                ViewBag.IsJoinRequested = relationDAO.Is_Join_Requested(userID, projectID);
                ViewBag.IsSponsored = relationDAO.Is_Sponsor(userID, projectID);
                ViewBag.IsSponsorRequested = relationDAO.Is_Sponsor_Requested(userID, projectID);
                ViewBag.IsFollowing = relationDAO.Is_Follower(userID, projectID);
                ViewBag.IsReported = relationDAO.Is_Reported(userID, projectID);

                return PartialView("_ActionToProject");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// プロジェクトへの参加を要求
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
                    //プロジェクトのリーダー全員へ通知を放送
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
        /// <summary>
        /// プロジェクトの参加要求を放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool SendJoinProjectNotify(string userID, string projectID)
        {
            try
            {
                //Get project leader(s)ID 
                //リーダー全員のIDを取得
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                List<string> leadersID = relationDAO.Get_Leaders(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink actor = userDAO.Get_SDLink(userID);
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                SDLink destination = projectDAO.Get_SDLink(projectID);

                Notification notif = new Notification(actor, Notify.JOIN_PROJECT_REQUEST, actor, destination);

                foreach (var leader in leadersID)
                {
                    userDAO.Add_Notification(leader, notif);
                }

                //Connect to NotificationHub
                //通知ハーブに接続
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getJoinProjectRequests(leadersID);

                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// グループの参加要求をキャンセル
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
                using (var trans = new TransactionScope())
                {
                    try
                    {
                        //delete sql relation
                        //SQL＿関係を削除
                        SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                        relationDAO.Delete_Join_Request(userID, projectID);

                        //Delete Group's leaders join group request notification
                        List<string> leaders = relationDAO.Get_Leaders(projectID);
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        var notifyID = userDAO.Get_JoinProject_NotifyID(leaders[0], userID, projectID);
                        if (notifyID != null)
                        {
                            foreach (var leader in leaders)
                            {
                                userDAO.Set_Notification_IsSeen(leader, notifyID);
                            }
                        }
                        trans.Complete();
                    }
                    catch
                    {
                        trans.Dispose();
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
        /// <summary>
        /// プロジェクトへ寄付要求
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// <summary>
        /// プロジェクトへの寄付要求をキャンセル
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
                //SQL_関係を削除
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
        /// <summary>
        /// 友達承認を
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        [HttpPost]
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
                        //SQL_関係を削除
                        SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                        relationDAO.Accept_Request(userID, requestID);

                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        SDLink first = userDAO.Get_SDLink(userID);
                        SDLink second = userDAO.Get_SDLink(requestID);
                        //userDAO.Add_Friend(userID);
                        //userDAO.Add_Friend(requestID);
                        userDAO.Add_Friend_To_List(userID, second);
                        userDAO.Add_Friend_To_List(requestID, first);

                        //Send Friend Request Accepted NOTIFICATION
                        //友達承認の通知を放送
                        SendFriendRequestAccepted(userID, requestID);

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
        /// <summary>
        /// 友達申請を拒否
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        [HttpPost]
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
                //SQL_関係を削除
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
        /// <summary>
        /// 友達申請リストを取得
        /// </summary>
        /// <returns></returns>
        public ActionResult FriendRequestList()
        {
            if (!Request.IsAjaxRequest())
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }

            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                //delete sql relation
                //SQL_関係を削除
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
        /// <summary>
        /// 友達を削除
        /// </summary>
        /// <param name="friendID"></param>
        /// <returns></returns>
        [HttpPost]
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
                        //SQL_関係を削除
                        SQL_AcAc_Relation_DAO relationDAO = new SQL_AcAc_Relation_DAO();
                        relationDAO.Delete_Friend(friendID, userID);
                        relationDAO.Delete_Friend(userID, friendID);

                        //Delete Friend in FriendList
                        //友達リストから削除
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
                return ActionToOtherUser(friendID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// グループのリーダーを辞任
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [HttpPost]
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
                        //SQL_関係を削除
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
                                string alert =  "Trưởng nhóm không thể rời nhóm có 1 trrưởng nhóm.";
                                return JavaScript("alert('" + alert + "')");
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
        /// <summary>
        /// プロジェクトのリーダー位置を辞任
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
                        //SQL_関係を削除
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
                                string alert = "Trưởng dự án không thể rời dự án chỉ có 1 trưởng dự án.";
                                return JavaScript("alert('" + alert + "')");
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
        /// <summary>
        /// 通知パネルで友達承認を
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
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
                        //友達承認の通知を放送
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
        /// <summary>
        /// 通知パーネルで友達申請を拒否
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
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
                //SQL_関係を削除
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
        /// <summary>
        /// 友達申請を数える
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// get friend not in a group
        /// グループにいない友達リストを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult FriendNotInGroup(string groupID)
        {
            if (!Request.IsAjaxRequest())
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }

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
        /// プロジェクトにいない友達リストを取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult FriendNotInProject(string projectID)
        {
            if (!Request.IsAjaxRequest())
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }

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
        /// <summary>
        /// プロジェクトに友達を紹介
        /// </summary>
        /// <param name="friendID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SuggestFriends(string[] friendID, string projectID)
        {
            try
            {
                if (friendID == null)
                {
                    ViewBag.ProjectID = projectID;
                    return FriendNotInProject(projectID);
                }

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
                return PartialView("_NotifyMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 紹介されたプロジェクトリストを取得
        /// </summary>
        /// <returns></returns>
        public ActionResult InvitedProjects()
        {
            if (!Request.IsAjaxRequest())
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }

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
        /// <summary>
        /// プロジェクトの紹介を承認
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// <summary>
        /// プロジェクトの紹介を拒否
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
        /// 管理するグループのリストの情報を取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult LeadGroups(string projectID)
        {
            if (!Request.IsAjaxRequest())
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();

                // get lead group list
                // 現在管理するグループ全員のIDを取得
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Lead_Groups(userID);
                // get joined group Info
                //そのグループの情報を取得
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
        /// <summary>
        /// パスワード変更画面を表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePassword");
        }
        /// <summary>
        /// パスワードを変更
        /// </summary>
        /// <param name="changePasswordModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            try
            {
                if(!ModelState.IsValid) return PartialView("_ChangePassword");

                string userID = Session["UserID"].ToString();
                //hash old password
                changePasswordModel.OldPassword = HashHelper.Hash(changePasswordModel.OldPassword);

                SQL_Account_DAO accountDAO = new SQL_Account_DAO();
                if(!accountDAO.Is_Password_Match(userID, changePasswordModel.OldPassword))
                {
                    ViewBag.Message = "Mật khẩu cũ không chính xác!";
                    return PartialView("_ChangePassword");
                }

                if (changePasswordModel.OldPassword == changePasswordModel.NewPassword)
                {
                    ViewBag.Message = "Mật khẩu mới phải khác mật khẩu cũ!";
                    return PartialView("_ChangePassword");
                }
                //hash new password
                changePasswordModel.NewPassword = HashHelper.Hash(changePasswordModel.NewPassword);

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

		[AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFile()
        {
            string _imgname = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);

                    _imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("/Images/FailureReport/Screenshot_") + _imgname + _ext;
                    _imgname = "Screenshot_" + _imgname + _ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving Image in Original Mode
                    pic.SaveAs(path);

                    // resizing image
                    MemoryStream ms = new MemoryStream();
                    WebImage img = new WebImage(_comPath);

                    if (img.Width > 2048)
                    {
                        int height = (int) (img.Height / (img.Width / 2048));
                        img.Resize(2048, height);
                    }
                    else if(img.Height > 2048)
                    {
                        int width = (int) (img.Width / (img.Height / 2048));
                        img.Resize(width, 2048);
                    }
                    img.Save(_comPath);
                    // end resize
                }
            }
            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddFailureReport()
        {
            return PartialView("_SendFailureReport");
        }    
        [HttpPost]
        public ActionResult AddFailureReport(Mongo_FailureReport info, string ScreenshotName)
        {
            try
            {
                Mongo_FailureReport_DAO mongoDAO = new Mongo_FailureReport_DAO();
                Mongo_FailureReport newInfo = new Mongo_FailureReport(info);

                //HttpPostedFileBase file = Request.Files["ScreenShot"];
                //if (file != null)
                //{
                //    // write your code to save image
                //    string uploadPath = Server.MapPath("/Images/FailureReport/" + newInfo._id.ToString() + ".jpg");
                //    file.SaveAs(uploadPath);
                //    newInfo.HaveScreenshot = true;
                //}

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
                    SDLink sentPerson = mongo_User_DAO.Get_SDLink(userID);
                    newInfo.SentPerson = sentPerson;
                    newInfo.HaveScreenshot = ScreenshotName;
                }
                mongoDAO.Add_FailureReport(newInfo);
                if(newInfo.Type == 1)
                {
                    ViewBag.Message = "Cám ơn bạn đã góp ý để iVolunteer ngày một hoàn thiện hơn.";
                }
                else
                {
                    ViewBag.Message = "Cám ơn bạn đã báo cáo vấn đề cho chúng tôi. Chúng tôi sẽ tìm tìm hiểu nguyên nhân và sửa chữa trong thời gian sớm nhất.";
                }
                return PartialView("_NotifyMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// プロファイル情報更新画面を表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdatePersonalInformation()
        {
            // check if parameter valid
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_PersonalInformation(userID);
                return PartialView("_UpdatePersonalInformation", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// プロファイル情報を更新
        /// </summary>
        /// <param name="newInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdatePersonalInformation(PersonalInformation newInfo)
        {
            /// check if parameter valid
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (!ModelState.IsValid) return PartialView("_UpdatePersonalInformation", newInfo);

            try
            {
                string userID = Session["UserID"].ToString();
                if (userID != newInfo.UserID)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Update_PersonalInformation(userID, newInfo);
                return RedirectToAction("PersonalInformation", "User", new { userID = userID });
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
    }
}