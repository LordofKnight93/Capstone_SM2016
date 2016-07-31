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
    public class GroupController : Controller
    {
        [HttpGet]
        public ActionResult CreateGroup()
        {
            return PartialView("_CreateGroup");
        }
        [HttpPost]
        public ActionResult CreateGroup(GroupInformation groupInfo)
        {
            if (!ModelState.IsValid) return PartialView("_CreateGroup", groupInfo);

            //set missing information
            groupInfo.DateCreate = DateTime.Now;
            groupInfo.MemberCount = 1;
            groupInfo.IsActivate = Status.IS_ACTIVATE;

            //craete creator
            string userID = Session["UserID"].ToString();

            //create mongo Group
            Mongo_Group mongo_Group = new Mongo_Group(groupInfo);

            //create sql Group
            SQL_Group sql_Group = new SQL_Group();
            sql_Group.GroupID = mongo_Group.GroupInformation.GroupID;
            sql_Group.IsActivate = true;

            //start transaction

            using (var transaction = new TransactionScope())
            {
                try
                {
                    // create DAO instance
                    Mongo_Group_DAO mongo_Group_DAO = new Mongo_Group_DAO();
                    Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
                    SQL_Group_DAO sql_Group_DAO = new SQL_Group_DAO();
                    SQL_AcGr_Relation_DAO sql_User_Group_DAO = new SQL_AcGr_Relation_DAO();

                    //write to DB
                    sql_Group_DAO.Add_Group(sql_Group);
                    sql_User_Group_DAO.Add_Leader(userID, sql_Group.GroupID);

                    mongo_Group_DAO.Add_Group(mongo_Group);
                    mongo_User_DAO.Join_Group(userID);

                    // copy default avatar and cover
                    FileInfo avatar = new FileInfo(Server.MapPath(Default.DEFAULT_AVATAR));
                    avatar.CopyTo(Server.MapPath("/Images/Group/Avatar/" + sql_Group.GroupID + ".jpg"));
                    FileInfo cover = new FileInfo(Server.MapPath(Default.DEFAULT_COVER));
                    cover.CopyTo(Server.MapPath("/Images/Group/Cover/" + sql_Group.GroupID + ".jpg"));

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }
            }

            return JavaScript("window.location = '" + Url.Action("GroupHome", "Group", new { groupID = sql_Group.GroupID }) + "'");
        }
        [HttpGet]
        public ActionResult GroupHome(string groupID)
        {
            SDLink result = null;
            try
            {
                SQL_Group_DAO sqlDAO = new SQL_Group_DAO();
                if (sqlDAO.IsActivate(groupID))
                {
                    Mongo_Group_DAO mongoDAO = new Mongo_Group_DAO();
                    result = mongoDAO.Get_SDLink(groupID);
                }

                if (Session["UserID"] != null)
                {
                    SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                    if (Session["UserID"].ToString() == "Admin")
                        ViewBag.IsAdmin = true;
                    else
                    {
                        string userID = Session["UserID"].ToString();
                        ViewBag.IsJoined = relationDAO.Is_Joined(userID, groupID);
                    }
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }

            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }
            else return View("GroupHome", result);
        }
        [ChildActionOnly]
        [OutputCache(Duration = 1)]
        public ActionResult AvatarCover(string groupID)
        {
            try
            {
                //get data
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var result = groupDAO.Get_SDLink(groupID);
                //get ralation 
                if (Session["UserID"] == null)
                {
                    ViewBag.CanChange = "false";
                }
                else
                {
                    string userID = Session["UserID"].ToString();
                    if (relationDAO.Is_Leader(userID, groupID))
                        ViewBag.CanChange = "true";
                    else ViewBag.CanChange = "false";
                }
                return PartialView("_AvatarCover", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [HttpGet]
        public ActionResult ChangeAvatar(string id)
        {
            //check permission here

            ViewBag.Action = "UploadAvatar";
            ViewBag.Controller = "Group";
            ViewBag.ID = id;
            return PartialView("_ImageUpload");
        }
        [HttpPost]
        public ActionResult UploadAvatar(string id)
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/Group/Avatar/" + id + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("GroupHome", "Group", new { groupID = id });
            }
            else return PartialView("_ImageUpload");
        }
        [HttpGet]
        public ActionResult ChangeCover(string id)
        {
            //check permission here

            ViewBag.Action = "UploadCover";
            ViewBag.Controller = "Group";
            ViewBag.ID = id;
            return PartialView("_ImageUpload");
        }
        [HttpPost]
        public ActionResult UploadCover(string id)
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                // write your code to save image
                string uploadPath = Server.MapPath("/Images/Group/Cover/" + id + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("GroupHome", "Group", new { groupID = id });
            }
            else return PartialView("_ImageUpload");
        }
        public ActionResult GroupInformation(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                if (Session["UserID"] == null)
                    ViewBag.IsLeader = false;
                else
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, groupID);
                }

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupInformation(groupID);
                return PartialView("_GroupInformation", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult GroupLeaders(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Leaders(groupID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, groupID);
                }
                ViewBag.GroupID = groupID;
                return PartialView("_GroupLeaders", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult GroupMembers(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Members(groupID);
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                if(Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, groupID);
                }
                ViewBag.GroupID = groupID;
                return PartialView("_GroupMembers", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult GroupRequests(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Requesters(groupID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.GroupID = groupID;

                return PartialView("_GroupRequests", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult GroupPublic(string groupID)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.GroupID = groupID;
                ViewBag.InSection = "Public";
                return PartialView("_GroupPublic");
            }

            string userID = Session["UserID"].ToString();

            //Leader will be able to Post in Public Section(in next View returned)
            SQL_AcGr_Relation_DAO relation = new SQL_AcGr_Relation_DAO();
            if (relation.Is_Leader(userID, groupID)) ViewBag.Role = "Leader";
            else ViewBag.Role = "Member";

            ViewBag.GroupID = groupID;
            ViewBag.InSection = "Public";
            return PartialView("_GroupPublic");
        }
        public ActionResult GroupDiscussion(string groupID)
        {
            ViewBag.InSection = "Discussion";
            ViewBag.GroupID = groupID;
            return PartialView("_GroupDiscussion");
        }
        public ActionResult GroupGallery()
        {
            return PartialView("_GroupGallery");
        }
        public ActionResult GroupPlan()
        {
            return PartialView("_GroupPlan");
        }
        public ActionResult GroupStructure(string groupID)
        {
            try
            {

                if (Session["UserID"] != null)
                {
                    SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, groupID);
                }
                ViewBag.GroupID = groupID;

                return PartialView("_GroupStructure");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult SearchGroup(string name)
        {
            ViewBag.Name = name;
            return View("SearchGroup");
        }
        public ActionResult NextResultPage(string name, int page)
        {
            try
            {
                if (page <= 0) page = 1;
                if(String.IsNullOrEmpty(name.Trim()))
                {
                    ViewBag.Message = "Độ dài chuỗi tìm kiếm từ 1 đến 100 ký tự";
                    return PartialView("ErrorMessage");
                }

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();

                List<GroupInformation> result = new List<GroupInformation>();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = groupDAO.Group_Search(name, 10 * (page - 1), 10);
                else result = groupDAO.Active_Group_Search(name, 10 * (page - 1), 10);

                ViewBag.Name = name;
                ViewBag.NextPage = page + 1;

                return PartialView("_GroupResult", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        public ActionResult AcceptRequest(string requestID, string groupID)
        {
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, groupID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Accept_Request(requestID, groupID);
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            userDAO.Join_Group(requestID);
                            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                            groupDAO.Members_Join(groupID, 1);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return null;
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult DeclineRequest(string requestID, string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, groupID))
                {
                    relationDAO.Delelte_Request(requestID, groupID);

                    return null;
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult SetLeader(string memberID, string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, groupID))
                {
                    relationDAO.Set_Leader(memberID, groupID);
                    // Send promote Leader notification 
                    SendPromotionNotify(userID, memberID, groupID);
                    return null;
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// Send notification to promoted member
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="memberID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool SendPromotionNotify(string userID, string memberID, string groupID)
        {
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            try
            {
                //Send Friend REquest accepted to requested User
                //Create Notify for request user
                SDLink leader = userDAO.Get_SDLink(userID);
                SDLink target = userDAO.Get_SDLink(memberID);
                SDLink destination = groupDAO.Get_SDLink(groupID);
                Notification notify = new Notification(leader, Notify.LEADER_PROMOTE_IN_GROUP, target, destination);
                userDAO.Add_Notification(memberID, notify);

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getJoinGroupAccepted(memberID);

                return true;
            }
            catch
            {
                throw;
            }
        }
        public ActionResult SetMember(string leaderID, string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, groupID))
                {
                    relationDAO.Set_Member(leaderID, groupID);

                    return null;
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult ExpelMember(string memberID, string groupID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, groupID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            if (relationDAO.Delete_Member(memberID, groupID))
                            {
                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                userDAO.Out_Group(memberID);
                                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                                groupDAO.Member_Out(groupID);
                            }

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return null;
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [ChildActionOnly]
        public ActionResult OrganizedProjects(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Organized_Projects(groupID);
                // get joined group Info
                Mongo_Project_DAO groupDAO = new Mongo_Project_DAO();
                var result = groupDAO.Get_ProjectsInformation(listID);

                return PartialView("_OrganizedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [ChildActionOnly]
        public ActionResult SponsoredProjects(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Sponsored_Projects(groupID);
                // get joined group Info
                Mongo_Project_DAO groupDAO = new Mongo_Project_DAO();
                var result = groupDAO.Get_ProjectsInformation(listID);

                return PartialView("_SponsoredProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [ChildActionOnly]
        public ActionResult ParticipatedProjects(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Participated_Projects(groupID);
                // get joined group Info
                Mongo_Project_DAO groupDAO = new Mongo_Project_DAO();
                var result = groupDAO.Get_ProjectsInformation(listID);

                return PartialView("_ParticipatedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult ActivityHistory(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                ViewBag.GroupID = groupID;
                return PartialView("_ActivityHistory");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public JsonResult AcceptRequestOnNotif(string userID, string requestID, string groupID, string notifyID)
        {
            SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
            using (var transaction = new TransactionScope())
            {
                try
                {
                    relationDAO.Accept_Request(requestID, groupID);
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    userDAO.Join_Group(requestID);
                    Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                    groupDAO.Members_Join(groupID, 1);

                    //Send notification to requested User
                    SendJoinGroupRequestAccepted(userID, requestID, groupID, notifyID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
            return Json(true);
        }
        public JsonResult DenyRequestOnNotif(string userID, string requestID, string groupID, string notifyID)
        {
            if (userID == null) return Json(false);
            try
            {
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                relationDAO.Delelte_Request(requestID, groupID);
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                userDAO.Delete_Notification(userID, notifyID);

                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }
        public bool SendJoinGroupRequestAccepted(string userID, string requestID, string groupID, string notifyID)
        {
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            try
            {

                //Set is seen
                userDAO.Set_Notification_IsSeen(userID, notifyID);

                //Send join_group_accepted notify to requested User
                //Create Notify for request user
                SDLink actor = userDAO.Get_SDLink(userID);
                SDLink target = userDAO.Get_SDLink(requestID);
                SDLink destination = groupDAO.Get_SDLink(groupID);
                Notification notify = new Notification(actor, Notify.JOIN_GROUP_ACCEPTED, target, destination);
                userDAO.Add_Notification(requestID, notify);

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getJoinGroupAccepted(requestID);

                return true;
            }
            catch
            {
                throw;
            }

        }
        /// <summary>
        /// Create Post 
        /// </summary>
        /// <param name="postInfor"></param>
        /// <param name="groupID"></param>
        /// <param name="inSection"></param>
        /// <returns></returns>
        public PartialViewResult AddPost(PostInformation postInfor, string groupID, string inSection)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = "Error";
                return PartialView("ErrorMessage");
            }
            //Create creator
            string userID = Session["UserID"].ToString();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            SDLink creator = userDAO.Get_SDLink(userID);
            //create destination 
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            SDLink group = groupDAO.Get_SDLink(groupID);

            postInfor.DateCreate = DateTime.Now;
            postInfor.DateLastActivity = DateTime.Now;
            //cretate mongo Post
            Mongo_Post mongo_Post = new Mongo_Post(postInfor);
            mongo_Post.PostInfomation.Creator = creator;
            mongo_Post.PostInfomation.Destination = group;

            //Create sql Pos
            SQL_Post sql_Post = new SQL_Post();
            sql_Post.PostID = mongo_Post._id.ToString();
            sql_Post.DateCreate = DateTime.Now.ToLocalTime();
            sql_Post.DateLastActivity = DateTime.Now.ToLocalTime();
            sql_Post.GroupID = groupID;
            sql_Post.IsPinned = false;
            if (inSection == "Discussion")
            {
                sql_Post.IsPublic = false;
                mongo_Post.PostInfomation.IsPublic = false;
            }
            else
            {
                sql_Post.IsPublic = true;
                mongo_Post.PostInfomation.IsPublic = true;
            }

            Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
            SQL_Post_DAO sql_Post_DAO = new SQL_Post_DAO();
            SQL_AcPo_Relation_DAO relation = new SQL_AcPo_Relation_DAO();
            //start transaction
            using (var transaction = new TransactionScope())
            {
                try
                {
                    sql_Post_DAO.Add_Post(sql_Post);
                    relation.Add(userID, sql_Post.PostID);
                    postDAO.Add_Post(mongo_Post);
                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }
            }
            SendNewPostNotify(userID, sql_Post.PostID, groupID);
            ViewBag.GroupID = groupID;
            if (inSection == "Discussion") return GetPostList(groupID);
            return GetPostList_InPublic(groupID);
        }
        /// <summary>
        /// Send New post created Notification to all Group's members except Post creator
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool SendNewPostNotify(string userID, string postID, string groupID)
        {
            try
            {
                //Get group leader(s)ID 
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                List<string> recieversID = relationDAO.Get_Members(groupID);
                recieversID.AddRange(relationDAO.Get_Leaders(groupID));
                //Send notify to all members except creator
                recieversID.Remove(userID);

                //Get SDLink 
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink actor = userDAO.Get_SDLink(userID);
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                SDLink destination = groupDAO.Get_SDLink(groupID);
                SDLink post = new SDLink(postID);
                //Create Notification
                Notification notif = new Notification(actor, Notify.POST_CREATED_IN_GROUP, post, destination);

                foreach (var member in recieversID)
                {
                    userDAO.Add_Notification(member, notif);
                }

                //Connect to NotificationHub
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getJoinGroupRequests(recieversID);

                return true;
            }
            catch
            {
                throw;
            }
        }
        public PartialViewResult GetPostList(string groupID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> postList = postDAO.Get_Private_Post_By_GroupID(groupID, 0, 5);
                ViewBag.GroupID = groupID;
                return PartialView("_PostList", postList);
            }
            catch
            {
                throw;
            }
        }
        public PartialViewResult GetPostList_InPublic(string groupID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> postList = postDAO.Get_Public_Post_By_GroupID(groupID, 0, 5);
                ViewBag.GroupID = groupID;
                return PartialView("_PostList", postList);
            }
            catch
            {
                throw;
            }
        }
        public PartialViewResult ShowAddCommentArea(string postID, string groupID)
        {
            ViewBag.PostID = postID;
            return PartialView("_PostWriteComment");
        }
        public ActionResult AddComment(Comment comment, string postID)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Nhap sai";
                return View("ErrorMessage");
            }
            string userID = Session["UserID"].ToString();
            Mongo_Post_DAO mongo_Post_DAO = new Mongo_Post_DAO();
            Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
            SDLink creator = mongo_User_DAO.Get_SDLink(userID);

            //Add more mongo Comment information
            comment.DateCreate = DateTime.Now.ToLocalTime();
            comment.Creator = creator;

            try
            {
                mongo_Post_DAO.Set_DateLastActivity(postID, comment.DateCreate);
                mongo_Post_DAO.Add_Comment(postID, comment);
                return GetCommentList(postID);
            }
            catch
            {
                throw;
            }
        }
        public PartialViewResult GetCommentList(string postID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Comment> commentList = postDAO.Get_Comments(postID, 0, 5);

                return PartialView("_CommentList", commentList);
            }
            catch
            {
                throw;
            }
        }
        public JsonResult LikePost(string postID)
        {
            if (Session["UserID"] == null)
            {
                return Json(false);
            }
            string userID = Session["UserID"].ToString();
            Mongo_Post_DAO mongo_Post_DAO = new Mongo_Post_DAO();
            Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();

            SDLink creator = mongo_User_DAO.Get_SDLink(userID);
            //If User has liked this Post 
            if (mongo_Post_DAO.Is_User_Liked(userID, postID)) return DislikePost(postID);
            try
            {
                mongo_Post_DAO.Set_DateLastActivity(postID, DateTime.Now.ToLocalTime());
                mongo_Post_DAO.Add_LikerList(postID, creator);

                ViewBag.postID = postID;
                return Json(new { status = true, dislike = false });
            }
            catch
            {
                throw;
            }
        }
        public JsonResult DislikePost(string postID)
        {
            if (Session["UserID"] == null)
            {
                return Json(false);
            }
            Mongo_Post_DAO mongo_Post_DAO = new Mongo_Post_DAO();
            try
            {
                //write to DB
                mongo_Post_DAO.Set_DateLastActivity(postID, DateTime.Now.ToLocalTime());
                mongo_Post_DAO.Delete_LikerList(postID, Session["UserID"].ToString());

                ViewBag.postID = postID;
                return Json(new { status = true, dislike = true });
            }
            catch
            {
                throw;
            }
        }
        [ChildActionOnly]
        public ActionResult CurrentProjects(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get joined group list
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Current_Projects(groupID);
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
        [HttpPost]
        public ActionResult AddMembers(string[] friendID, string groupID)
        {
            try
            {
                if(friendID == null) return GroupMembers(groupID);

                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, groupID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Add_Members(friendID, groupID);
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            userDAO.Batch_Join_Group(friendID);
                            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                            groupDAO.Members_Join(groupID, friendID.Count());

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    // return group member to content-panel
                    return GroupMembers(groupID);
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [HttpGet]
        public ActionResult UpdateGroupInformation(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, groupID))
                {
                    Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                    var result = groupDAO.Get_GroupInformation(groupID);
                    return PartialView("_UpdateGroupInformation", result);
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        [HttpPost]
        public ActionResult UpdateGroupInformation(string groupID, GroupInformation newInfo)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, groupID))
                {
                    Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                    groupDAO.Update_GroupInformation(groupID, newInfo);
                    return GroupInformation(groupID);
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult ActionToProject(string groupID, string projectID)
        {
            try
            {
                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();

                ViewBag.IsJoined = relationDAO.Is_Joined(groupID, projectID);
                ViewBag.IsJoinRequested = relationDAO.Is_Join_Requested(groupID, projectID);
                ViewBag.IsSponsored = relationDAO.Is_Sponsored(groupID, projectID);
                ViewBag.isSponsorRequested = relationDAO.Is_Sponsor_Requested(groupID, projectID);

                ViewBag.GroupID = groupID;
                ViewBag.ProjectID = projectID;

                return PartialView("_ActionToProject");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// get member that not join or send join request to project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult MemberNotInProject(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Members_Not_Join_Project(groupID, projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.GroupID = groupID;
                ViewBag.ProjectID = projectID;

                ViewBag.Action = "RepresentJoinRequest";
                return PartialView("_MemberSelector", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// represnt group to send join request to sa project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <param name="memberID"></param>
        /// <returns></returns>
        public ActionResult RepresentJoinRequest(string groupID, string projectID, string[] memberID)
        {
            try
            {
                if (memberID == null) return MemberNotInProject(groupID, projectID);

                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO acGrDAO = new SQL_AcGr_Relation_DAO();

                if (acGrDAO.Is_Leader(userID, groupID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();
                            acPrDAO.Add_Join_Requests(memberID, projectID);
                            SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                            grPrDAO.Add_Join_Request(groupID, projectID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    // return group member to content-panel
                    ViewBag.Message = "Gửi yêu cầu thành công.";
                    return PartialView("ErrorMessage");
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// get member that not sponsor or send sponsor request to project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult MemberNotSponsorProject(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                var listID = relationDAO.Get_Members_Not_Sponsor_Project(groupID, projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.GroupID = groupID;
                ViewBag.ProjectID = projectID;

                ViewBag.Action = "RepresentSponsorRequest";

                return PartialView("_MemberSelector", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// represnt group to send sponsor request to sa project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <param name="memberID"></param>
        /// <returns></returns>
        public ActionResult RepresentSponsorRequest(string groupID, string projectID, string[] memberID)
        {
            try
            {
                if (memberID == null) return MemberNotInProject(groupID, projectID);

                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcGr_Relation_DAO acGrDAO = new SQL_AcGr_Relation_DAO();

                if (acGrDAO.Is_Leader(userID, groupID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();
                            acPrDAO.Add_Sponsor_Requests(memberID, projectID);
                            SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                            grPrDAO.Add_Sponsor_Request(groupID, projectID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    // return group member to content-panel
                    ViewBag.Message = "Gửi yêu cầu thành công.";
                    return PartialView("ErrorMessage");
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult ProjectResign(string groupID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();
                SQL_AcGr_Relation_DAO acGrDAO = new SQL_AcGr_Relation_DAO();
                if(!acGrDAO.Is_Leader(userID, groupID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                grPrDAO.Delete_Joined_Group(groupID, projectID);

                return ActionToProject(groupID, projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public PartialViewResult LoadMorePost(string groupID, int times)
        {
            try
            {
                int skipNo = times * 5;
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> postList = postDAO.Get_Private_Post_By_GroupID(groupID, skipNo, 5);
                ViewBag.GroupID = groupID;
                return PartialView("_PostList", postList);
            }
            catch
            {
                throw;
            }
        }
        public PartialViewResult LoadMorePostInPublic(string groupID, int times)
        {
            try
            {
                int skipNo = times * 5;
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> postList = postDAO.Get_Public_Post_By_GroupID(groupID, skipNo, 5);
                ViewBag.GroupID = groupID;
                return PartialView("_PostList", postList);
            }
            catch
            {
                throw;
            }
        }
    }
}