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
using MongoDB.Bson;
using iVolunteer.Helpers;
using System.Web.Helpers;

namespace iVolunteer.Controllers
{
    public class GroupController : Controller
    {
        /// <summary>
        /// グループの設立画面を表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateGroup()
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }
            return PartialView("_CreateGroup");
        }
        /// <summary>
        /// グループを節理る
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateGroup(GroupInformation groupInfo)
        {
            if (!ModelState.IsValid) return PartialView("_CreateGroup", groupInfo);

            //set missing information
            groupInfo.DateCreate = DateTime.Now;
            groupInfo.MemberCount = 1;
            groupInfo.IsActivate = Status.IS_ACTIVATE;

            //create creator
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
        /// <summary>
        /// グループのホーム画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GroupHome(string groupID)
        {
            // get cookie code block
            if (Session["UserID"] == null && Request.Cookies["Cookie"] != null)
            {
                HttpCookie cookie = Request.Cookies["Cookie"];
                string email = Request.Cookies["Cookie"].Values["Email"];
                //decrypt here
                string passwod = Request.Cookies["Cookie"].Values["Password"];
                //decrypt here
                SQL_Account account = null;
                try
                {
                    SQL_Account_DAO accountDAO = new SQL_Account_DAO();
                    account = accountDAO.Get_Account_By_Email(email);
                    if (account != null && account.IsActivate == Status.IS_ACTIVATE
                                       && account.IsConfirm == Status.IS_CONFIRMED
                                       && account.Password == passwod)
                    {
                        Session["UserID"] = account.UserID;
                        Session["DisplayName"] = account.DisplayName;
                        Session["Role"] = account.IsAdmin ? "Admin" : "User";
                    }
                    else
                    {
                        HttpCookie myCookie = new HttpCookie("Cookie");
                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie);
                    }
                }
                catch
                {

                }
            }

            if (String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SDLink result = null;
                SQL_Group_DAO sqlDAO = new SQL_Group_DAO();
                if (sqlDAO.IsActivate(groupID) || (Session["Role"] != null && Session["Role"].ToString() == "Admin"))
                {
                    Mongo_Group_DAO mongoDAO = new Mongo_Group_DAO();
                    result = mongoDAO.Get_SDLink(groupID);

                    if (Session["UserID"] != null)
                    {
                        if (Session["Role"].ToString() == "Admin")
                            ViewBag.IsAdmin = true;
                        else
                        {
                            SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                            string userID = Session["UserID"].ToString();
                            ViewBag.IsJoined = relationDAO.Is_Joined(userID, groupID);
                        }
                    }
                    return View("GroupHome", result);
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
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// グループのアバターとカバーの変更画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// アバターを変更
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {

                var pic = System.Web.HttpContext.Current.Request.Files["Image"];
                if (pic.ContentLength > 0)
                {
                    var _comPath = Server.MapPath("/Images/Group/Avatar/" + id + ".jpg");



                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving Image in Original Mode
                    pic.SaveAs(path);

                    // resizing image
                    MemoryStream ms = new MemoryStream();
                    WebImage img = new WebImage(_comPath);

                    if (img.Width > 500)
                    {
                        int height = (int)(img.Height / (img.Width / 500));
                        img.Resize(500, height);
                    }
                    else if (img.Height > 500)
                    {
                        int width = (int)(img.Width / (img.Height / 500));
                        img.Resize(width, 500);
                    }
                    img.Save(_comPath);
                    // end resize
                }
                else
                {
                    ViewBag.Message = "Upload không thành công.Vui lòng thử lại.";
                    return PartialView("_ImageUpload");
                }
            }
            return JavaScript("location.reload(true)");
        }
        /// <summary>
        /// カバー変更画面を表示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeCover(string id)
        {
            //check permission here

            ViewBag.Action = "UploadCover";
            ViewBag.Controller = "Group";
            ViewBag.ID = id;
            return PartialView("_ImageUpload");
        }
        /// <summary>
        /// カバー写真を変更
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadCover(string id)
        {
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {

                var pic = System.Web.HttpContext.Current.Request.Files["Image"];
                if (pic.ContentLength > 0)
                {
                    var _comPath = Server.MapPath("/Images/Group/Cover/" + id + ".jpg");



                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving Image in Original Mode
                    pic.SaveAs(path);

                    // resizing image
                    MemoryStream ms = new MemoryStream();
                    WebImage img = new WebImage(_comPath);

                    if (img.Width > 2048)
                    {
                        int height = (int)(img.Height / (img.Width / 2048));
                        img.Resize(2048, height);
                    }
                    else if (img.Height > 2048)
                    {
                        int width = (int)(img.Width / (img.Height / 2048));
                        img.Resize(width, 2048);
                    }
                    img.Save(_comPath);
                    // end resize
                }
                else
                {
                    ViewBag.Message = "Upload không thành công.Vui lòng thử lại.";
                    return PartialView("_ImageUpload");
                }
            }
			return JavaScript("location.reload(true)");
        }
        /// <summary>
        /// グループの情報画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult GroupInformation(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
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
        /// <summary>
        /// グループのリーダーリスト画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult GroupLeaders(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
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
        /// <summary>
        /// グループのメンバーリスト画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult GroupMembers(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
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
        /// <summary>
        /// グループの参加要求リスト画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult GroupRequests(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
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
        /// <summary>
        /// 公開セクション画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 相談セクション画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult GroupDiscussion(string groupID)
        {
            ViewBag.InSection = "Discussion";
            ViewBag.GroupID = groupID;
            return PartialView("_GroupDiscussion");
        }
        /// <summary>
        /// 写真アルバムセクション画面を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupGallery(string groupID)
        {
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();
                //Leader will be able to Post in Public Section(in next View returned)
                SQL_AcGr_Relation_DAO relation = new SQL_AcGr_Relation_DAO();
                if (relation.Is_Leader(userID, groupID)) { ViewBag.Role = "Leader"; }
                else if (relation.Is_Member(userID,groupID))
                {
                    ViewBag.Role = "Member";
                }
                else
                {
                    ViewBag.Role = "Guess";
                }
            }
            ViewBag.InSection = "GroupGallery";
            ViewBag.GroupID = groupID;
            return PartialView("_GroupGallery");
        }
        /// <summary>
        /// 計画セクション画面を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupPlan()
        {
            return PartialView("_GroupPlan");
        }
        /// <summary>
        /// 構成セクション画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// グループ検索画面を表示
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult SearchGroup(string name)
        {
            ViewBag.Name = name;
            return View("SearchGroup");
        }
        /// <summary>
        /// 検索結果画面を表示
        /// </summary>
        /// <param name="name"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult NextResultPage(string name, int page)
        {
            try
            {
                if (page <= 0) page = 1;
                if(String.IsNullOrWhiteSpace(name.Trim()))
                {
                    ViewBag.Message = "Rất tiếc, chúng tôi không hiểu tìm kiếm này. Vui lòng thử truy vấn theo cách khác.";
                    return PartialView("ErrorMessage");
                }

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();

                List<GroupInformation> result = new List<GroupInformation>();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = groupDAO.Group_Search(name, 10 * (page - 1), 10);
                else result = groupDAO.Active_Group_Search(name, 10 * (page - 1), 10);

                ViewBag.Name = name;

                if (result.Count == 0)
                {
                    if (page == 1)
                        ViewBag.Message = "Không tìm thấy kết quả";
                    else
                        ViewBag.Message = "Không còn kết quả nào nữa!";
                    return PartialView("ErrorMessage");
                }

                ViewBag.NextPage = page + 1;

                return PartialView("_GroupResult", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// 参加要求を承認
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
                //Get Notification of this request (to set Is_Seen to other leaders)
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                string notifyID = userDAO.Get_JoinGroup_NotifyID(userID, requestID, groupID);
                //Get Leaders list
                List<string> leadersID = relationDAO.Get_Leaders(groupID);
                leadersID.Remove(userID);
                if (relationDAO.Is_Leader(userID, groupID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Accept_Request(requestID, groupID);
                            userDAO.Join_Group(requestID);
                            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                            groupDAO.Members_Join(groupID, 1);
                            //Set IsSeen to other leaders's this requestNotify.
                            foreach (var leader in leadersID)
                            {
                                userDAO.Set_Notification_IsSeen(leader, notifyID);
                            }
                            SendJoinGroupRequestAccepted(userID, requestID, groupID, notifyID);

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
        /// <summary>
        /// 参加要求を拒否
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// リーダーを昇進させる
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// 昇進通知を放送
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
        /// <summary>
        /// メンバー状態を設定
        /// </summary>
        /// <param name="leaderID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// メンバーを排出
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 開催するプロジェクトリスト画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        //[ChildActionOnly]
        public ActionResult OrganizedProjects(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get organized project list
                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();
                var listID = relationDAO.Get_Organized_Projects(groupID);
                // get organized group Info
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
        /// <summary>
        /// 寄付したプロジェクトリスト画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        //[ChildActionOnly]
        public ActionResult SponsoredProjects(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get sponsored group list
                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();
                var listID = relationDAO.Get_Sponsored_Projects(groupID);
                // get sponsored group Info
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
        /// <summary>
        /// 参加したプロジェクトのリスト画面をひょじ
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        //[ChildActionOnly]
        public ActionResult ParticipatedProjects(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get joined group list
                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();
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
        /// <summary>
        /// 活動セクション画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult ActivityHistory(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
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
        /// <summary>
        /// 通知パーネルで参加要求を承認
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requestID"></param>
        /// <param name="groupID"></param>
        /// <param name="notifyID"></param>
        /// <returns></returns>
        public JsonResult AcceptRequestOnNotif(string userID, string requestID, string groupID, string notifyID)
        {
            SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            List<string> leadersID = relationDAO.Get_Leaders(groupID);
            leadersID.Remove(userID);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    relationDAO.Accept_Request(requestID, groupID);
                    userDAO.Join_Group(requestID);
                    groupDAO.Members_Join(groupID, 1);
                    //Set this Notification's tatus of other GroupLeaders to SEEN
                    foreach (var item in leadersID)
                    {
                        userDAO.Set_Notification_IsSeen(item, notifyID);
                    }
                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
            //Send notification to requested User
            SendJoinGroupRequestAccepted(userID, requestID, groupID, notifyID);
            return Json(true);
        }
        /// <summary>
        /// 通知パーネルで参加要求を拒否
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requestID"></param>
        /// <param name="groupID"></param>
        /// <param name="notifyID"></param>
        /// <returns></returns>
        public JsonResult DenyRequestOnNotif(string userID, string requestID, string groupID, string notifyID)
        {
            if (userID == null) return Json(false);

            SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
            List<string> leadersID = relationDAO.Get_Leaders(groupID);
            leadersID.Remove(userID);

            using (var transaction = new TransactionScope())
            {
                try
                {
                    relationDAO.Delelte_Request(requestID, groupID);
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    userDAO.Delete_Notification(userID, notifyID);
                    //Set this Notification's tatus of other GroupLeaders to IS_SEEN
                    foreach (var item in leadersID)
                    {
                        userDAO.Set_Notification_IsSeen(item, notifyID);
                    }
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
        /// <summary>
        /// 要求承認の通知を放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requestID"></param>
        /// <param name="groupID"></param>
        /// <param name="notifyID"></param>
        /// <returns></returns>
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
        /// ポストを作成
        /// </summary>
        /// <param name="postInfor"></param>
        /// <param name="groupID"></param>
        /// <param name="inSection"></param>
        /// <returns></returns>
        public ActionResult AddPost(PostInformation postInfor, string groupID, string inSection)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = "Error";
                return PartialView("ErrorMessage");
            }
            //Create creator
            string userID = Session["UserID"].ToString();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();

            //If the post is in Public -> creator = destination;
            SDLink creator = new SDLink();
            if (inSection == "Discussion") creator = userDAO.Get_SDLink(userID);
            else creator = groupDAO.Get_SDLink(groupID);
            //create destination 
            SDLink group = groupDAO.Get_SDLink(groupID);

            postInfor.DateCreate = DateTime.Now;
            postInfor.DateLastActivity = DateTime.Now;
            //cretate mongo Post
            Mongo_Post mongo_Post = new Mongo_Post(postInfor);
            mongo_Post.PostInfomation.Creator = creator;
            mongo_Post.PostInfomation.Destination = group;

            //IMAGE Process
            if (postInfor.ImgLink != null)
            {
                string oldPath = Server.MapPath("/Images/Post/" + userID + ".jpg");
                //Change old path of temp file to the new one (map with postID)
                string newPath = Server.MapPath("/Images/Post/" + mongo_Post.PostInfomation.PostID + ".jpg");
                System.IO.File.Copy(oldPath, newPath);
                //Delete temp
                System.IO.File.Delete(oldPath);
                mongo_Post.PostInfomation.ImgLink = newPath;
            }

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
                    relation.Add_Relation(userID, sql_Post.PostID, AcPoRelation.CREATOR_RELATION);
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
            ModelState.Clear();
            if (inSection == "Discussion") return GroupDiscussion(groupID);
            return GroupPublic(groupID);
        }
        /// <summary>
        /// Send New post created Notification to all Group's members
        /// 新しいポストの通知を放送
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
        /// <summary>
        /// 相談セクションにあるポストリストを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 公開セクションにあるポストリストを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
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
        /// <summary>
        /// コメントブロークを表示
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public PartialViewResult ShowCommentArea(string postID, string groupID, string cmtCount)
        {
            ViewBag.GroupID = groupID;
            ViewBag.PostID = postID;
            ViewBag.CommentCount = cmtCount;
            return PartialView("_CommentArea");
        }
        /// <summary>
        /// コメント記載部分を表示
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public PartialViewResult ShowAddCommentArea(string postID, string groupID)
        {
            ViewBag.GroupID = groupID;
            ViewBag.PostID = postID;
            return PartialView("_PostWriteComment");
        }
        /// <summary>
        /// コメントを記載
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult AddComment(Comment comment, string postID, string groupID)
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
            comment.CommentID = ObjectId.GenerateNewId().ToString();
            comment.DateCreate = DateTime.Now.ToLocalTime();
            comment.Creator = creator;

            try
            {
                mongo_Post_DAO.Set_DateLastActivity(postID, comment.DateCreate);
                mongo_Post_DAO.Add_Comment(postID, comment);
                SendPostCommentedNotify(userID, postID, groupID);

                return GetCommentList(postID, groupID);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Send Post commented Notification
        /// ポストがコメントされた通知を放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool SendPostCommentedNotify(string userID, string postID, string groupID)
        {
            try
            {
                SQL_AcPo_Relation_DAO relationDAO = new SQL_AcPo_Relation_DAO();
                if (!relationDAO.Is_Owner(userID, postID))
                {
                    string ownerID = relationDAO.Get_Owner(postID);
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                    SDLink actor = userDAO.Get_SDLink(userID);
                    SDLink destination = groupDAO.Get_SDLink(groupID);
                    SDLink post = new SDLink(postID);

                    if (!userDAO.Is_Post_Has_Unseen_Notify(ownerID, postID))
                    {
                        Notification notif = new Notification(actor, Notify.POST_CMTED_IN_GROUP, post, destination);
                        userDAO.Add_Notification(ownerID, notif);
                    }
                    else
                    {
                        string noitfyID = userDAO.Get_PostCmted_NotifyID(ownerID, postID, Notify.POST_CMTED_IN_GROUP);

                        if (!userDAO.Is_Actor_In_Notify(ownerID, noitfyID, userID))
                        {
                            userDAO.Add_Actor_To_PostCmted_Notify(ownerID, noitfyID, actor);
                        }
                    }
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    hubContext.Clients.All.getTaskRelatedInfo(ownerID);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// コメントリストを取得
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public PartialViewResult GetCommentList(string postID, string groupID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Comment> commentList = postDAO.Get_Comments(postID, 0, 5);

                if (Session["UserID"] == null)
                {
                    return PartialView("_CommentList", commentList);
                }
                //check if user is leader (for delete usage)
                SQL_AcGr_Relation_DAO relation = new SQL_AcGr_Relation_DAO();
                if (relation.Is_Leader(Session["UserID"].ToString(), groupID))
                    ViewBag.IsLeader = true;
                else ViewBag.IsLeader = false;

                if (postDAO.Get_Cmt_Count(postID) > 5)
                    ViewBag.LoadMore = true;
                ViewBag.PostID = postID;
                ViewBag.GroupID = groupID;
                return PartialView("_CommentList", commentList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 残りのコメントを取得
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public PartialViewResult LoadOtherComment(string postID, string groupID)
        {
            try
            {
                //check if user is leader (for delete usage)
                SQL_AcGr_Relation_DAO relation = new SQL_AcGr_Relation_DAO();
                if (relation.Is_Leader(Session["UserID"].ToString(), groupID))
                    ViewBag.IsLeader = true;
                else ViewBag.IsLeader = false;

                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Comment> commentList = postDAO.Get_Comments(postID, 5, 100);
                ViewBag.PostID = postID;
                ViewBag.GroupID = groupID;
                ViewBag.LoadMore = false;
                return PartialView("_CommentList", commentList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// ポストをライク
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public ActionResult LikePost(string postID)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Newfeed", "Home");
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
                return IsLiked(postID);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// ポストをライクしたかを判定
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public PartialViewResult IsLiked(string postID)
        {
            int likeCount = 0;
            if (Session["UserID"] == null)
            {
                return PartialView("_LikeStatus");
            }
            string userID = Session["UserID"].ToString();
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                likeCount = postDAO.Get_Post_By_ID(postID).LikeCount;
                if (postDAO.Is_User_Liked(userID, postID))
                {
                    ViewBag.IsLiked = true;
                }
                else
                {
                    ViewBag.IsLiked = false;
                }
            }
            catch
            {
                throw;
            }
            ViewBag.LikeCount = likeCount;
            ViewBag.PostID = postID;
            return PartialView("_LikeStatus");
        }
        /// <summary>
        /// ポストをディスライク
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public ActionResult DislikePost(string postID)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Newfeed", "Home");
            }
            Mongo_Post_DAO mongo_Post_DAO = new Mongo_Post_DAO();
            try
            {
                mongo_Post_DAO.Set_DateLastActivity(postID, DateTime.Now.ToLocalTime());
                mongo_Post_DAO.Delete_LikerList(postID, Session["UserID"].ToString());

                ViewBag.postID = postID;
                return IsLiked(postID);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 現在参加プルジェクト画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult CurrentProjects(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                // get group curent project list
                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();
                var listID = relationDAO.Get_Current_Projects(groupID);
                // get group curent project Info
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
        /// グループに友達を紹介
        /// </summary>
        /// <param name="friendID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddMembers(string[] friendID, string groupID)
        {
            try
            {
                if (friendID == null)
                {
                    ViewBag.GroupID = groupID;
                    return GroupMembers(groupID);
                }

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
        /// <summary>
        /// グループ情報を更新画面を表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateGroupInformation(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID) || Session["UserID"] == null)
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
        /// <summary>
        /// グループ情報を更新
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="newInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateGroupInformation(string groupID, GroupInformation newInfo)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if(!ModelState.IsValid) return PartialView("_UpdateGroupInformation", newInfo);

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
        /// <summary>
        /// プロジェクトへ操作
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ActionToProject(string groupID, string projectID)
        {
            try
            {
                SQL_Project_DAO projectDAO = new SQL_Project_DAO();
                ViewBag.IsRecruiting = projectDAO.IsRecruiting(projectID);

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
        /// プロジェクトにいないグループのメンバーを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult MemberNotInProject(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
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
        /// represent group to send join request to a project
        /// グループの代表としてプロジェクトへ参加要求
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <param name="memberID"></param>
        /// <returns></returns>
        [HttpPost]
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
                    return PartialView("_NotifyMessage");
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
        /// プロジェクトの寄付者でもなく寄付要求者でもないグループメンバーを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult MemberNotSponsorProject(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(groupID))
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
        /// グループの代表としてプロジェクトに寄付要求
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <param name="memberID"></param>
        /// <returns></returns>
        [HttpPost]
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
                    return PartialView("_NotifyMessage");
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
        /// メンバーを辞任
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
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
                grPrDAO.Delete_Organized_Group(groupID, projectID);

                return ActionToProject(groupID, projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 相談セクションでさらにポストを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="times"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 公開セクションでさらにぽうとを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="times"></param>
        /// <returns></returns>
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
        /// <summary>
        /// ポストにリダイレクト
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <param name="notifyID"></param>
        /// <param name="postID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult SeenNewPostNotify(string userID, string groupID, string notifyID, string postID, string type)
        {
            SDLink result = null;
            List<Mongo_Post> posts = new List<Mongo_Post>();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                if (!postDAO.Is_Exist(postID))
                {
                    userDAO.Set_Notification_IsSeen(userID, notifyID);
                    return Json(new { error = true, message = "Bài viết này không tồn tại." });
                }
                posts.Add(postDAO.Get_Mg_Post_By_ID(postID));
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
            else
            {
                userDAO.Set_Notification_IsSeen(userID, notifyID);
                ViewBag.GroupID = groupID;
                var groupHome = RenderRazorViewToString(this.ControllerContext, "GroupHome", result);
                //var groupDiscusson = RenderRazorViewToString(this.ControllerContext, "_GroupDiscussion", null);
                var post = RenderRazorViewToString(this.ControllerContext, "_PostList", posts);
                //return Json(new { groupHome, groupDiscusson, post, type });
                return Json(new { groupHome, post, type });
            }
        }
        /// <summary>
        /// 多重ビューをレンダー
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        /// <summary>
        /// ポストへ操作
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public PartialViewResult ActionToPost(string postID, string groupID)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = "";
                return PartialView("ErrorMessage");
            }
            string userID = Session["UserID"].ToString();
            SQL_AcPo_Relation_DAO relation = new SQL_AcPo_Relation_DAO();
            if (relation.Is_Owner(userID, postID)) {
                ViewBag.IsOwner = true;
            }
            else ViewBag.IsOwner = false;

            SQL_AcGr_Relation_DAO grRelation = new SQL_AcGr_Relation_DAO();
            if (grRelation.Is_Leader(userID, groupID))
            {
                ViewBag.IsLeader = true;
            }
            else ViewBag.IsLeader = false;

            Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
            if (postDAO.Is_Pinned(postID))
            {
                ViewBag.IsPinned = true;
            }
            else ViewBag.IsPinned = false;

            ViewBag.GroupID = groupID;
            ViewBag.PostID = postID;
            return PartialView("_ActionToPost");
        }
        /// <summary>
        /// ポストを削除
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeletePost(string postID, string groupID)
        {

            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            string userID = Session["UserID"].ToString();
            Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
            SQL_AcPo_Relation_DAO relation = new SQL_AcPo_Relation_DAO();
            SQL_Post_DAO sqlPost = new SQL_Post_DAO();
            bool permission = true;

            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    permission = postDAO.Is_InPublic(postID);
                    //Delete all relation related to this Post
                    relation.Delete_all_relations(postID);
                    //Delete Post in SQL
                    sqlPost.Delete_Post(postID);
                    //Delete Create relation
                    relation.Delete_relation(userID, postID, AcPoRelation.CREATOR_RELATION);
                    //Delete all like relation
                    relation.Delete_All_Likes(postID);
                    postDAO.Delete_Post(postID);

                    trans.Complete();
                }
                catch
                {
                    trans.Dispose();
                    throw;
                }
            }
            if (permission) return GroupPublic(groupID);
            return GroupDiscussion(groupID);
        }
        /// <summary>
        /// ポストを固定化
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult PinPost(string postID, string groupID)
        {
            SQL_Post_DAO sqlPost = new SQL_Post_DAO();
            Mongo_Post_DAO mgPost = new Mongo_Post_DAO();
            bool permission = true;
            //Check if this is Current Pinned Post
            if (!mgPost.Is_Pinned(postID))
            {
                using (var trans = new TransactionScope())
                {
                    try
                    {
                        permission = mgPost.Is_InPublic(postID);
                        //get current Pin
                        string currentPinID = "";
                        if (permission) currentPinID = mgPost.Get_PinnedPost_ID(groupID, true);
                        else currentPinID = mgPost.Get_PinnedPost_ID(groupID, false);

                        if (currentPinID != null)
                        {
                            //Delete current Pin
                            sqlPost.UnpinPost(currentPinID);
                            mgPost.Set_IsNotPin(currentPinID);
                        }
                        //SEt new Pin
                        sqlPost.PinPost(postID);
                        mgPost.Set_IsPin(postID);

                        trans.Complete();
                    }
                    catch
                    {
                        trans.Dispose();
                        throw;
                    }
                }
            }
            if (permission) return GroupPublic(groupID);
            return GroupDiscussion(groupID);
        }
        /// <summary>
        /// ポストの固定状態を削除
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult UnpinPost(string postID, string groupID)
        {
            SQL_Post_DAO sqlPost = new SQL_Post_DAO();
            Mongo_Post_DAO mgPost = new Mongo_Post_DAO();
            bool permission = true;
            using (var trans = new TransactionScope())
            {
                try
                {
                    permission = mgPost.Is_InPublic(postID);
                    sqlPost.UnpinPost(postID);
                    mgPost.Set_IsNotPin(postID);

                    trans.Complete();
                }
                catch
                {
                    trans.Dispose();
                    throw;
                }
            }
            if (permission) return GroupPublic(groupID);
            return GroupDiscussion(groupID);
        }
        /// <summary>
        /// ピンポストを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public PartialViewResult GetPinnedPost(string groupID)
        {
            Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
            try
            {
                Mongo_Post pinPost = postDAO.Get_Mg_PinnedPost(groupID, false);

                ViewBag.GroupID = groupID;
                return PartialView("_PinnedPost", pinPost);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// ピンポストを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public PartialViewResult GetPinnedPost_InPublic(string groupID)
        {
            Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
            try
            {
                Mongo_Post pinPost = postDAO.Get_Mg_PinnedPost(groupID, true);

                ViewBag.GroupID = groupID;
                return PartialView("_PinnedPost", pinPost);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// ポストに写真追加画面を表示
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AddImageToPost()
        {
            string userID = Session["UserID"].ToString();
            ViewBag.Action = "UploadPostImage";
            ViewBag.Controller = "Group";
            ViewBag.ID = userID;
            return PartialView("_ImageUploadInPost");
        }
        /// <summary>
        /// ポスト写真をアプロード
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UploadPostImage(string id)
        {
            HttpPostedFileBase file = Request.Files["Image"];
            if (file != null)
            {
                if (file.FileName != "")
                {
                    // write your code to save image
                    string uploadPath = Server.MapPath("/Images/Post/" + id + ".jpg");
                    file.SaveAs(uploadPath);
                    ViewBag.ImageLink = uploadPath;
                    return Json(uploadPath);
                }
                else return Json(null);
                
            }
            else return PartialView("_ImageUpload");
        }
        public ActionResult ViewPost(string notifyID, string postID, string groupID)
        {
            if (Session["UserID"].ToString() == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                //CHeck if post is deleted
                if (!postDAO.Is_Exist(postID))
                {
                    ViewBag.Message = "Bài viết không tồn tại.";
                    return PartialView("ErrorMessage");
                }
                Mongo_Post post = postDAO.Get_Mg_Post_By_ID(postID);
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                //Set notify isSEEN
                userDAO.Set_Notification_IsSeen(userID, notifyID);
                ViewBag.GroupID = groupID;
                return PartialView("_PinnedPost", post);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// コメントを削除
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="commentID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeleteComment(string postID, string commentID, string groupID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                postDAO.Delete_Comment(postID, commentID);
                return GetCommentList(postID, groupID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        /// <summary>
        /// represent group to send join request to a project
        /// グループの代表としてプロジェクトへ参加要求をキャンセル
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelRepresentJoinRequest(string groupID, string projectID)
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

                SQL_AcGr_Relation_DAO acGrDAO = new SQL_AcGr_Relation_DAO();

                if (acGrDAO.Is_Leader(userID, groupID))
                {
                    SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                    grPrDAO.Delete_Join_Request(groupID, projectID);

                    return ActionToProject(groupID, projectID);
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
        /// represnt group to send sponsor request to sa project
        /// グループの代表としてプロジェクトに寄付要求をキャンセル
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelRepresentSponsorRequest(string groupID, string projectID)
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

                SQL_AcGr_Relation_DAO acGrDAO = new SQL_AcGr_Relation_DAO();

                if (acGrDAO.Is_Leader(userID, groupID))
                {
                    SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                    grPrDAO.Delete_Sponsor_Request(groupID, projectID);

                    return ActionToProject(groupID, projectID);
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
        /// ///album/////
        /// </summary>
        /// <param name="albumID"></param>
        /// <returns></returns>

        public int checkRoleAlbum(string albumID, string targetID)
        {
            int role;
            string userID = Session["UserID"].ToString();
            SQL_AcAl_Relation_DAO relation = new SQL_AcAl_Relation_DAO();
            SQL_AcGr_Relation_DAO relaGr = new SQL_AcGr_Relation_DAO();
            if (relation.Is_Creator(userID, albumID) || relaGr.Is_Leader(userID, targetID))
            {
                role = 1;
            }
            else
            {
                role = 0;
            }
            return role;
        }
        public ActionResult AlbumAddImage(string albumID, string targetID)
        {
            ViewBag.AlbumID = albumID;
            Session["Album"] = albumID;
            if (checkRoleAlbum(albumID, targetID) == 1)
            {
                return PartialView("_AlbumAddImage");
            }
            else
            {
                return GetAlbumList(targetID);
            }

        }
        public ActionResult AlbumEditImage(string albumID, string targetID)
        {
            ViewBag.targetID = targetID;
            ViewBag.AlbumID = albumID;
            Session["Album"] = albumID;
            if (checkRoleAlbum(albumID, targetID) == 1)
            {
                return PartialView("_AlbumEditImage");
            }
            else
            {
                return GetAlbumList(targetID);
            }
        }
        public ActionResult AlbumShowImage(string albumID, string targetID)
        {
            Mongo_Album_DAO mongo_Album_DAO = new Mongo_Album_DAO();
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();
                string groupID = mongo_Album_DAO.Get_TargetID(albumID);
                SQL_AcGr_Relation_DAO relation = new SQL_AcGr_Relation_DAO();
                if (relation.Is_Leader(userID, groupID)) ViewBag.Role = "Leader";
                else ViewBag.Role = "Member";
            }
            ImageInformation mongo_Image = new ImageInformation();
            var model = mongo_Album_DAO.Get_Image_By_AlbumID(albumID);
            ViewBag.AlbumID = albumID;
            ViewBag.GroupID = targetID;
            Session["Album"] = albumID;
            return PartialView("_AlbumShowImage", model);
        }
        public PartialViewResult GetAlbumList(string groupID)
        {
            if(Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();
                //Leader will be able to Post in Public Section(in next View returned)
                SQL_AcGr_Relation_DAO relation = new SQL_AcGr_Relation_DAO();
                if (relation.Is_Leader(userID, groupID)) ViewBag.Role = "Leader";
                else ViewBag.Role = "Member";
                ViewBag.UserID = userID;
            }
            ViewBag.targetID = groupID;
            try
            {
                Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                List<Mongo_Album> albumList = albumDAO.Get_Private_Album_By_TargetID(groupID, 0, 50);
                Random rd = new Random();
                int rand = rd.Next(0, int.MaxValue);
                ViewBag.rd = rand;
                return PartialView("_AlbumList", albumList);
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public ActionResult CreateAlbum(string targetID)
        {
            ViewBag.TargetID = targetID;
            return PartialView("_AlbumCreate");
        }
        [HttpPost]
        public ActionResult CreateAlbum(AlbumInformation albumInfo, string targetID, int targetType)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TargetID = targetID;
                return PartialView("_AlbumCreate", albumInfo);
            };
            //create album creator
            string userID = Session["UserID"].ToString();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            SDLink creator = userDAO.Get_SDLink(userID);
            albumInfo.DateCreate = DateTime.Now;
            albumInfo.DateLastActivity = DateTime.Now;
            //Create mongo Album
            Mongo_Album mongo_Album = new Mongo_Album(albumInfo);
            mongo_Album.AlbumInformation = albumInfo;
            mongo_Album.AlbumInformation.AlbumID = mongo_Album._id.ToString();
            mongo_Album.AlbumInformation.Creator = creator;
            //Create sql Album
            SQL_Album sql_Album = new SQL_Album();
            sql_Album.AlbumID = mongo_Album._id.ToString();
            if (targetType == 1)
            {
                sql_Album.GroupID = targetID;
            }
            else if (targetType == 2)
            {
                sql_Album.ProjectID = targetID;
            }
            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Album_DAO mongo_Album_DAO = new Mongo_Album_DAO();
                        SQL_Album_DAO sql_Album_DAO = new SQL_Album_DAO();
                        SQL_AcAl_Relation_DAO sql_User_Album_DAO = new SQL_AcAl_Relation_DAO();
                        //write data to db
                        sql_Album_DAO.Add_Album(sql_Album);
                        sql_User_Album_DAO.Add_Creator(userID, sql_Album.AlbumID);
                        mongo_Album_DAO.Add_Album(mongo_Album);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        return PartialView("ErrorMessage");
                    }
                }
                ViewBag.AlbumID = sql_Album.AlbumID;
                Session["Album"] = sql_Album.AlbumID;
                return GetAlbumList(targetID);
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        public ActionResult DeleteAlbum(string albumID, string targetID)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            string userID = Session["UserID"].ToString();


            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                    SQL_AcAl_Relation_DAO relation = new SQL_AcAl_Relation_DAO();
                    SQL_AcIm_Relation_DAO relationIm = new SQL_AcIm_Relation_DAO();
                    SQL_Album_DAO sqlAlbum = new SQL_Album_DAO();
                    SQL_AcPr_Relation_DAO relaPr = new SQL_AcPr_Relation_DAO();
                    int type;
                    if (checkRoleAlbum(albumID,targetID)==1)
                    {
                        type = 1;
                    }
                    else
                    {
                        return GetAlbumList(targetID);
                    }
                    //Delete all relation related to this Album
                    relationIm.Delete_all_relations_Im(albumID);
                    relation.Delete_relation_Al(albumID, type);
                    sqlAlbum.Delete_Image(albumID);
                    sqlAlbum.Delete_Album(albumID);
                    //Delete mongo
                    albumDAO.Delete_Album(albumID);
                    trans.Complete();
                }
                catch
                {
                    trans.Dispose();
                    throw;
                }
            }
            return GetAlbumList(targetID);
        }
        ////////Comment In ALbum////////////////
        public PartialViewResult AlbumShowCommentArea(string albumID, string groupID, string cmtCount)
        {
            ViewBag.GroupID = groupID;
            ViewBag.AlbumID = albumID;
            ViewBag.CommentCount = cmtCount;
            return PartialView("_AlbumCommentArea");
        }
        /// <summary>
        /// コメント記載部分を表示
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public PartialViewResult AlbumShowAddCommentArea(string albumID, string groupID)
        {
            ViewBag.GroupID = groupID;
            ViewBag.AlbumID = albumID;
            return PartialView("_AlbumWriteComment");
        }
        /// <summary>
        /// コメントを記載
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="albumID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult AlbumAddComment(Comment comment, string albumID, string groupID)
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
            Mongo_Album_DAO mongo_Album_DAO = new Mongo_Album_DAO();
            Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
            SDLink creator = mongo_User_DAO.Get_SDLink(userID);

            //Add more mongo Comment information
            comment.CommentID = ObjectId.GenerateNewId().ToString();
            comment.DateCreate = DateTime.Now.ToLocalTime();
            comment.Creator = creator;

            try
            {
                mongo_Album_DAO.Set_DateLastActivity(albumID, comment.DateCreate);
                mongo_Album_DAO.Add_Comment(albumID, comment);

                return AlbumGetCommentList(albumID,groupID);
            }
            catch
            {
                throw;
            }
        }
        public PartialViewResult AlbumGetCommentList(string albumID, string groupID)
        {
               
                string userID = Session["UserID"].ToString();
                SQL_AcGr_Relation_DAO relation = new SQL_AcGr_Relation_DAO();
                if (relation.Is_Leader(userID, groupID)) { ViewBag.Role = "Leader"; }
                else if (relation.Is_Member(userID, groupID))
                {
                    ViewBag.Role = "Member";
                }
                else
                {
                    ViewBag.Role = null;
                }
           
            try
            {
                Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                List<Comment> commentList = albumDAO.Get_Comments(albumID, 0, 5);
                if (albumDAO.Get_Cmt_Count(albumID) > 5)
                    ViewBag.LoadMore = true;
                ViewBag.AlbumID = albumID;
                return PartialView("_AlbumCommentList", commentList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 残りのコメントを取得
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public PartialViewResult AlbumLoadOtherComment(string albumID)
        {
            try
            {
                Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                List<Comment> commentList = albumDAO.Get_Comments(albumID, 5, 100);
                ViewBag.AlbumID = albumID;
                ViewBag.LoadMore = false;
                return PartialView("_AlbumCommentList", commentList);
            }
            catch
            {
                throw;
            }
        }
        public ActionResult AlbumDeleteComment(string albumID, string commentID, string groupID)
        {
            try
            {
                Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                albumDAO.Delete_Comment(albumID,commentID);
                return AlbumGetCommentList(albumID,groupID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// ポストをライク
        /// </summary>
        /// <param name="albumID"></param>
        /// <returns></returns>
        public ActionResult LikeAlbum(string albumID)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Newfeed", "Home");
            }
            string userID = Session["UserID"].ToString();
            Mongo_Album_DAO mongo_Album_DAO = new Mongo_Album_DAO();
            Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();

            SDLink creator = mongo_User_DAO.Get_SDLink(userID);
            //If User has liked this Post 
            if (mongo_Album_DAO.Is_User_Liked(userID, albumID)) return DislikeAlbum(albumID);
            try
            {
                mongo_Album_DAO.Set_DateLastActivity(albumID, DateTime.Now.ToLocalTime());
                mongo_Album_DAO.Add_LikerList(albumID, creator);

                ViewBag.albumID = albumID;
                return AlbumIsLiked(albumID);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// ポストをライクしたかを判定
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public PartialViewResult AlbumIsLiked(string albumID)
        {
            int likeCount = 0;
            if (Session["UserID"] == null)
            {
                return PartialView("_LikeStatus");
            }
            string userID = Session["UserID"].ToString();
            try
            {
                Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                likeCount = albumDAO.Get_Album_By_ID(albumID).LikeCount;
                if (albumDAO.Is_User_Liked(userID, albumID))
                {
                    ViewBag.IsLiked = true;
                }
                else
                {
                    ViewBag.IsLiked = false;
                }
            }
            catch
            {
                throw;
            }
            ViewBag.LikeCount = likeCount;
            ViewBag.AlbumID = albumID;
            return PartialView("_AlbumLikeStatus");
        }
        /// <summary>
        /// ポストをディスライク
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public ActionResult DislikeAlbum(string albumID)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Newfeed", "Home");
            }
            Mongo_Album_DAO mongo_Album_DAO = new Mongo_Album_DAO();
            try
            {
                mongo_Album_DAO.Set_DateLastActivity(albumID, DateTime.Now.ToLocalTime());
                mongo_Album_DAO.Delete_LikerList(albumID, Session["UserID"].ToString());

                ViewBag.AlbumID = albumID;
                return AlbumIsLiked(albumID);
            }
            catch
            {
                throw;
            }
        }
    }
}