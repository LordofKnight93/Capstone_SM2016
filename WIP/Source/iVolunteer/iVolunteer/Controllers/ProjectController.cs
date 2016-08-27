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
using MongoDB.Bson;
using System.IO;
using Microsoft.AspNet.SignalR;
using iVolunteer.Hubs;
using Newtonsoft.Json;
using iVolunteer.Helpers;
using System.Web.Helpers;

namespace iVolunteer.Controllers
{
    public class ProjectController : Controller
    {
        /// <summary>
        /// プロジェクトを設立画面を表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateProject()
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }
            return PartialView("_CreateProject");
        }
        /// <summary>
        /// プルジェクトを設立
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <param name="tagsList"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateProject(ProjectInformation projectInfo, string[] tagsList)
        {
            int tagsCount = tagsList == null ? 0 : tagsList.Count();
            if (tagsCount > 0)
            {
                projectInfo.TagsString = tagsList[0];
                if (tagsCount > 1)
                {
                    for (int i = 1; i < tagsCount; i++)
                    {
                        projectInfo.TagsString = projectInfo.TagsString + ", " + tagsList[i];
                    }
                }
            }

            if (!ModelState.IsValid) return PartialView("_CreateProject", projectInfo);

            //set missing information for project
            projectInfo.DateCreate = DateTime.Now;
            projectInfo.MemberCount = 1;
            projectInfo.InProgress = Status.ONGOING;
            projectInfo.IsRecruit = Status.IS_RECRUITING;
            projectInfo.IsActivate = Status.IS_ACTIVATE;
            projectInfo.IsRecruit = Status.IS_RECRUITING;

            string userID = Session["UserID"].ToString();

            //create mongo project
            Mongo_Project mongo_Project = new Mongo_Project(projectInfo);

            //create sql project
            SQL_Project sql_Project = new SQL_Project();
            sql_Project.ProjectID = mongo_Project.ProjectInformation.ProjectID;
            sql_Project.IsRecruiting = Status.IS_RECRUITING;
            sql_Project.InProgress = Status.ONGOING;
            sql_Project.IsActivate = true;

            //start transaction
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //create DAO instance 
                    Mongo_Project_DAO mongo_Project_DAO = new Mongo_Project_DAO();
                    Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
                    Mongo_Fund_DAO mongo_Fund_DAO = new Mongo_Fund_DAO();
                    Mongo_Finance_DAO mongo_Finance_DAO = new Mongo_Finance_DAO();
                    SQL_Project_DAO sql_Project_DAO = new SQL_Project_DAO();
                    SQL_AcPr_Relation_DAO sql_User_Project_DAO = new SQL_AcPr_Relation_DAO();

                    //write to DB
                    sql_Project_DAO.Add_Project(sql_Project);
                    sql_User_Project_DAO.Add_Leader(userID, sql_Project.ProjectID);
                    sql_User_Project_DAO.Add_Organizer(userID, sql_Project.ProjectID);

                    mongo_Project_DAO.Add_Project(mongo_Project);
                    mongo_User_DAO.Join_Project(userID);
                    mongo_User_DAO.Organize_Project(userID);

                    // Auto Add Finance and Fund
                    //create project id
                    SDLink project = new SDLink();
                    project = mongo_Project_DAO.Get_SDLink(mongo_Project.ProjectInformation.ProjectID);

                    //add mongo Finance
                    Mongo_Finance mongo_Finance = new Mongo_Finance();
                    mongo_Finance.Project = project;

                    //add Mongo Fund
                    Mongo_Fund mongo_Fund = new Mongo_Fund();
                    mongo_Fund.Project = project;

                    mongo_Fund_DAO.Add_Fund(mongo_Fund);
                    mongo_Finance_DAO.Add_Finance(mongo_Finance);

                    // copy default avatar and cover
                    FileInfo avatar = new FileInfo(Server.MapPath(Default.DEFAULT_AVATAR));
                    avatar.CopyTo(Server.MapPath("/Images/Project/Avatar/" + sql_Project.ProjectID + ".jpg"));
                    FileInfo cover = new FileInfo(Server.MapPath(Default.DEFAULT_COVER));
                    cover.CopyTo(Server.MapPath("/Images/Project/Cover/" + sql_Project.ProjectID + ".jpg"));

                    transaction.Complete();
                }
                catch (Exception e)
                {
                    transaction.Dispose();
                    ViewBag.Message = e.ToString();
                    return PartialView("ErrorMessage");
                }
            }

            return JavaScript("window.location = '" + Url.Action("ProjectHome", "Project", new { projectID = sql_Project.ProjectID }) + "'");
        }
        /// <summary>
        /// プロジェクトホーム画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ProjectHome(string projectID)
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

            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SDLink result = null;

                SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                if (sqlDAO.IsActivate(projectID) || (Session["Role"] != null && Session["Role"].ToString() == "Admin"))
                {
                    Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                    result = mongoDAO.Get_SDLink(projectID);

                    if (Session["UserID"] != null)
                    {
                        SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                        if (Session["Role"].ToString() == "Admin")
                            ViewBag.IsAdmin = true;
                        else
                        {
                            string userID = Session["UserID"].ToString();
                            ViewBag.IsJoined = relationDAO.Is_Joined(userID, projectID);
                            ViewBag.IsSponsored = relationDAO.Is_Sponsor(userID, projectID);
                        }
                    }
                    return View("ProjectHome", result);
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
        /// アバターカバー変更画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [OutputCache(Duration = 1)]
        public ActionResult AvatarCover(string projectID)
        {
            SDLink result = null;
            try
            {
                //get data
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                result = projectDAO.Get_SDLink(projectID);
                //get ralation 
                if (Session["UserID"] == null)
                {
                    ViewBag.CanChange = "false";
                }
                else
                {
                    string userID = Session["UserID"].ToString();
                    if (relationDAO.Is_Leader(userID, projectID))
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
        /// アバター変更画面を表示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeAvatar(string id)
        {
            //check permission here

            ViewBag.Action = "UploadAvatar";
            ViewBag.Controller = "Project";
            ViewBag.ID = id;
            return PartialView("_ImageUpload");
        }
        /// <summary>
        /// アバターをアプロード
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadAvatar(string id)
        {
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {

                var pic = System.Web.HttpContext.Current.Request.Files["Image"];
                if (pic.ContentLength > 0)
                {
                    var _comPath = Server.MapPath("/Images/Project/Avatar/" + id + ".jpg");



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
            ViewBag.Controller = "Project";
            ViewBag.ID = id;
            return PartialView("_ImageUpload");
        }
        /// <summary>
        /// カバーを変更
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
                    var _comPath = Server.MapPath("/Images/Project/Cover/" + id + ".jpg");



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
        /// プロジェクト情報セクション画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ProjectInformation(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectInformation(projectID);
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                return PartialView("_ProjectInformation", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 管理者リスト画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult OrganizedUsers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Organizers(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                ViewBag.ProjectID = projectID;
                return PartialView("_OrganizedUsers", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// リーダーリスト画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ProjectLeaders(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Leaders(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                ViewBag.ProjectID = projectID;
                return PartialView("_ProjectLeaders", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 寄付者画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult SponsoredUsers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Sponsors(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                ViewBag.ProjectID = projectID;
                return PartialView("_SponsoredUsers", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// メンバー画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ProjectMembers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Members(projectID);
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsJoined = relationDAO.Is_Joined(userID, projectID); 
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                ViewBag.ProjectID = projectID;
                return PartialView("_ProjectMembers", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 一覧参加し他グループ表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult JoinedGroups(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();
                var listID = relationDAO.Get_Joined_Groups(projectID);

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO rlDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = rlDAO.Is_Leader(userID, projectID);
                }
                ViewBag.ProjectID = projectID;
                return PartialView("_JoinedGroups", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 寄付したグループ画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult SponsoredGroups(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();
                var listID = relationDAO.Get_Sponsored_Groups(projectID);

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO rlDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = rlDAO.Is_Leader(userID, projectID);
                }
                ViewBag.ProjectID = projectID;
                return PartialView("_SponsoredGroups", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 管理されたグループ画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult OrganizedGroups(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();
                var listID = relationDAO.Get_Organized_Groups(projectID);

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO rlDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = rlDAO.Is_Leader(userID, projectID);
                }
                ViewBag.ProjectID = projectID;
                return PartialView("_OrganizedGroups", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 参加要求したグループ画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult GroupJoinRequests(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();
                string userID = Session["UserID"].ToString();

                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
                else
                {
                    ViewBag.IsLeader = acPrDAO.Is_Leader(userID, projectID);
                }

                SQL_GrPr_Relation_DAO relationDAO = new SQL_GrPr_Relation_DAO();
                var listID = relationDAO.Get_Join_Requests(projectID);

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_GroupJoinRequests", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 寄付要求したグループ画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult GroupSponsorRequests(string projectID)
        {
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();
                string userID = Session["UserID"].ToString();

                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
                else
                {
                    ViewBag.IsLeader = acPrDAO.Is_Leader(userID, projectID);
                }

                SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                var listID = grPrDAO.Get_Sponsor_Requests(projectID);

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_GroupSponsorRequests", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 参加要求したユーザー画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult UserJoinRequests(string projectID)
        {
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                string userID = Session["UserID"].ToString();

                if (!relationDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
                else ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);

                var listID = relationDAO.Get_Join_Requests(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_UserJoinRequests", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 寄付要求したユーザー画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult UserSponsorRequests(string projectID)
        {
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                string userID = Session["UserID"].ToString();

                if (!relationDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
                else ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);

                var listID = relationDAO.Get_Sponsor_Requests(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_UserSponsorRequests", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// プロジェクトを検索
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchProject()
        {
            var searchModel = TempData["SearchModel"] as SearchModel;
            if(searchModel == null)
                searchModel = new SearchModel();

            return View("SearchProject", searchModel);
        }
        /// <summary>
        /// プロジェクトを詳細検索
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult AdvancedSearchProject(SearchModel searchModel)
        {
            try
            {
                if (searchModel == null) searchModel = new SearchModel();

                if (String.IsNullOrWhiteSpace(searchModel.Name))
                    searchModel.Name = "";

                if (String.IsNullOrWhiteSpace(searchModel.Location))
                    searchModel.Location = "";

                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();

                List<ProjectInformation> result = new List<ProjectInformation>();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = projectDAO.Project_Search(searchModel, 0, 10);
                else result = projectDAO.Active_Project_Search(searchModel, 0, 10);

                TempData["SearchModel"] = searchModel;

                return NextResultPage(1);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// 次の検索結果ベージを表示
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult NextResultPage(int page)
        {
            try
            {
                if (page <= 0) page = 1;
                SearchModel searchModel = TempData["SearchModel"] as SearchModel;
                if (searchModel == null) searchModel = new SearchModel();

                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();

                List<ProjectInformation> result = new List<ProjectInformation>();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = projectDAO.Project_Search(searchModel, 10 * (page - 1), 10);
                else result = projectDAO.Active_Project_Search(searchModel, 10 * (page - 1), 10);

                if (result.Count == 0)
                {
                    if (page == 1)
                        ViewBag.Message = "Không tìm thấy kết quả";
                    else
                        ViewBag.Message = "Không còn kết quả nào nữa!";
                    return PartialView("ErrorMessage");
                }

                ViewBag.NextPage = page + 1;

                return PartialView("_ProjectResult", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// 公開セクションを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ProjectPublic(string projectID)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.ProjectID = projectID;
                ViewBag.InSection = "Public";
                return PartialView("_ProjectPublic");
            }
            string userID = Session["UserID"].ToString();

            //Leader will be able to Post in Public Section(in next View returned)
            SQL_AcPr_Relation_DAO relation = new SQL_AcPr_Relation_DAO();
            if (relation.Is_Leader(userID, projectID)) ViewBag.Role = "Leader";
            else ViewBag.Role = "Member";

            ViewBag.ProjectID = projectID;
            ViewBag.InSection = "Public";
            return PartialView("_ProjectPublic");
        }
        /// <summary>
        /// 相談セクションを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ProjectDiscussion(string projectID)
        {
            ViewBag.InSection = "Discussion";
            ViewBag.ProjectID = projectID;
            return PartialView("_ProjectDiscussion");
        }

        public ActionResult ProjectGallery(string projectID)
        {
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relation = new SQL_AcPr_Relation_DAO();
                if (relation.Is_Leader(userID, projectID)) { ViewBag.Role = "Leader"; }
                else if (relation.Is_Member(userID,projectID))
                {
                    ViewBag.Role = "Member";
                }
                else
                {
                    ViewBag.Role = "Guess";
                }
            }
            ViewBag.InSection = "GroupGallery";
            ViewBag.ProjectID = projectID;
            return PartialView("_ProjectGallery");
        }
        /// <summary>
        /// 計画セクションを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ProjectPlan(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectInformation(projectID);

                //Check Current User is Learder of project or not
                if (Session["UserID"] != null)
                {
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.ProjectID = projectID;
                return PartialView("_ProjectPlan");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 構成セクションを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ProjectStructure(string projectID)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                ViewBag.ProjectID = projectID;

                return PartialView("_ProjectStructure");
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
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetLeader(string memberID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    relationDAO.Set_Leader(memberID, projectID);
                    // Send Promotion Notify
                    SendPromotionNotify(userID, memberID, projectID);
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
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool SendPromotionNotify(string userID, string memberID, string projectID)
        {
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
            try
            {
                //Send Friend REquest accepted to requested User
                //Create Notify for request user
                SDLink leader = userDAO.Get_SDLink(userID);
                SDLink target = userDAO.Get_SDLink(memberID);
                SDLink destination = projectDAO.Get_SDLink(projectID);
                Notification notify = new Notification(leader, Notify.LEADER_PROMOTE_IN_PROJECT, target, destination);
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
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetMember(string leaderID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    relationDAO.Set_Member(leaderID, projectID);

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
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExpelMember(string memberID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    if (relationDAO.Is_Organizer(memberID, projectID))
                    {
                        string alert = "Cá nhân đồng tổ chức, không thế trục xuất!";
                        return JavaScript("alert('" + alert + "')");
                    }

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            if (relationDAO.Delete_Member(memberID, projectID))
                            {
                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                userDAO.Out_Project(memberID);
                                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                                projectDAO.Members_Out(projectID, 1);
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
        /// ユーザー参加要求を承認
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AcceptUserJoinRequest(string requestID, string projectID)
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

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                //Get Notification of this request (to set Is_Seen to other leaders)
                Mongo_User_DAO userDAO = new Mongo_User_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Accept_Join_Request(requestID, projectID);
                            userDAO.Join_Project(requestID);
                            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                            projectDAO.Members_Join(projectID, 1);

                            SendJoinProjectRequestAccepted(userID, requestID, projectID);

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
        /// ユーザーの寄付要求を承認
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AcceptUserSponsorRequest(string requestID, string projectID)
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

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Accept_Sponsor_Request(requestID, projectID);
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            userDAO.Sponsor_Project(requestID);

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
        /// ユーザーの参加要求を拒否
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeclineUserJoinRequest(string requestID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    relationDAO.Delete_Join_Request(requestID, projectID);

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
        /// ユーザーの寄付容共を拒否
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeclineUserSponsorRequest(string requestID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    relationDAO.Delete_Sponsor_Request(requestID, projectID);

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
        /// 寄付したユーザを解雇
        /// </summary>
        /// <param name="sponsorID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DismissSponsoredUser(string sponsorID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Delete_Sponsor(sponsorID, projectID);
                            //update mongo accoutn information projectcount
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            userDAO.Not_Sponsor_Project(sponsorID);

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
        /// 寄付登録画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateSponsor(string projectID)
        {
            ViewBag.ProjectID = projectID;
            return PartialView("_CreateSponsor");
        }
        /// <summary>
        /// 寄付登録を
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSponsor(string projectID, Sponsor guest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProjectID = projectID;
                return PartialView("_CreateSponsor", guest);
            }

            if (String.IsNullOrWhiteSpace(guest.SponsorPhone)
                && String.IsNullOrWhiteSpace(guest.SponsorEmail)
                && String.IsNullOrWhiteSpace(guest.SponsorPhone))
            {
                ViewBag.ProjectID = projectID;
                ViewBag.Message = "Bạn cần nhập ít nhất 1 thông tin liên lạc!"; ;
                return PartialView("_CreateSponsor", guest);
            }

            //set mising information
            guest.SponsorID = ObjectId.GenerateNewId().ToString();
            guest.Status = Status.PENDING;

            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                projectDAO.Add_GuestSponsor(projectID, guest);

                //Send Notification to Project Leader
                SendGuestSponsorRequestNotify(guest, projectID);

                ViewBag.Message = "Gửi yêu cầu thành công";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// グループへの寄付要求を通知
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool SendGuestSponsorRequestNotify(Sponsor guest, string projectID)
        {
            try
            {
                //Get project leader(s)ID 
                //グループリーダー全員のIDを取得
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                List<string> leadersID = relationDAO.Get_Leaders(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink actor = new SDLink();
                actor.DisplayName = guest.SponsorName;
                actor.ID = guest.SponsorID;
                actor.Handler = "Guest";

                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                SDLink destination = projectDAO.Get_SDLink(projectID);

                //Add Notifcation Item
                //通知を作成
                Notification notif = new Notification(actor, Notify.GUEST_SPONSOR_RQ, actor, destination);

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
        /// 寄付したゲストを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult SponsoredGuests(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                if (Session["UserID"] != null)
                {
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_GuestSponsors(projectID);

                ViewBag.ProjectID = projectID;
                return PartialView("_SponsoredGuests", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 寄付要求したゲストを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult GuestSponsorRequests(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                if (Session["UserID"] != null)
                {
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_GuestSponsor_Requests(projectID);

                ViewBag.ProjectID = projectID;
                return PartialView("_GuestSponsorRequests", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 寄付したゲストを解雇
        /// </summary>
        /// <param name="sponsorID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DismissSponsoredGuest(string sponsorID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    projectDAO.Delete_GuestSponsor(projectID, sponsorID);

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
        /// ゲストの寄付要求を承認
        /// </summary>
        /// <param name="sponsorID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AcceptGuestSponsorRequest(string sponsorID, string projectID)
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

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    projectDAO.Accept_GuestSponsor(projectID, sponsorID);

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
        /// ゲストの寄付要求を拒否
        /// </summary>
        /// <param name="sponsorID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeclineGuestSponsorRequest(string sponsorID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    projectDAO.Delete_GuestSponsor(projectID, sponsorID);

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
        /// get user that is suggested
        /// 紹介されたユーザーの情報を取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult SuggestedUsers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationlDAO = new SQL_AcPr_Relation_DAO();

                if (relationlDAO.Is_Leader(userID, projectID))
                {
                    var listID = relationlDAO.Get_Suggest_Users(projectID);
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    var result = userDAO.Get_AccountsInformation(listID);

                    ViewBag.ProjectID = projectID;
                    return PartialView("_SuggestedUsers", result);
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
        /// プロジェクトにリストの友達を紹介
        /// </summary>
        /// <param name="friendID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InviteUsers(string[] friendID, string projectID)
        {
            try
            {
                if (friendID == null) return RedirectToAction("FriendNotInProject", "Account", new { projectID = projectID });

                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, projectID))
                {
                    relationDAO.Invite_Users(friendID, projectID);
                    ViewBag.Message = "Đã mời thành công, hãy chờ hồi âm.";
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
        /// プロジェクトに友達を紹介
        /// </summary>
        /// <param name="suggestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InviteUser(string suggestID, string projectID)
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

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Delete_Suggest_User(suggestID, projectID);
                            relationDAO.Invite_User(suggestID, projectID);

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
        /// 紹介されたユーザーを拒否
        /// </summary>
        /// <param name="suggestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeclineSuggestedUser(string suggestID, string projectID)
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

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                relationDAO.Delete_Suggest_User(suggestID, projectID);

                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 参加したグループのメンバーを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult JoinedGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_JoinedGroup_Members(groupID, projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                else ViewBag.IsLeader = false;

                ViewBag.ProjectID = projectID;
                return PartialView("_GroupMembersList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 同管理したグループのメンバーリストを取得
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult OrganizedGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_OrganizedGroup_Members(groupID, projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                else ViewBag.IsLeader = false;

                ViewBag.ProjectID = projectID;
                return PartialView("_GroupMembersList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 寄付したグループのメンバーリストを表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult SponsoredGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_SponsoredGroup_Members(groupID, projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.ProjectID = projectID;
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                else ViewBag.IsLeader = false;

                return PartialView("_GroupMembersList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 寄付要求したグループのめんーばーリストを表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult SponsorRequestGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Sponsor_Requested_Group_Members(groupID, projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_GroupMembersList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 参加要求したグループのメンバーリストを表示
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult JoinRequestGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || String.IsNullOrWhiteSpace(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_Join_Requested_Group_Members(groupID, projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_GroupMembersList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 参加要求を承認
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AcceptGroupJoinRequest(string requestID, string projectID)
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

                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();

                if (acPrDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                            if (grPrDAO.Accept_Join_Request(requestID, projectID))
                            {
                                var acceptList = acPrDAO.Accept_Join_Requested_Group_Members(requestID, projectID);
                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                userDAO.Batch_Join_Project(acceptList);

                                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                                projectDAO.Members_Join(projectID, acceptList.Count);
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
        /// グループの寄付要求を承認
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AcceptGroupSponsorRequest(string requestID, string projectID)
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

                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();

                if (acPrDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                            grPrDAO.Accept_Sponsor_Request(requestID, projectID);

                            var acceptList = acPrDAO.Accept_Sponsor_Requested_Group_Members(requestID, projectID);
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            userDAO.Batch_Sponsor_Project(acceptList);
                            
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
        /// グループの参加要求を拒否
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeclineGroupJoinRequest(string requestID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();

                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                grPrDAO.Delete_Join_Request(requestID, projectID);

                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// グループの寄付要求を拒否
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeclineGroupSponsorRequest(string requestID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();

                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                grPrDAO.Delete_Sponsor_Request(requestID, projectID);

                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 同管理したグループを削除
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DismissOrganizedGroup(string groupID, string projectID)
        {
            // check if parameter valid
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();
                string userID = Session["UserID"].ToString();
                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                grPrDAO.Delete_Organized_Group(groupID, projectID);
                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// グループの寄付要求を削除
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DismissSponsoredGroup(string groupID, string projectID)
        {
            // check if parameter valid
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();
                string userID = Session["UserID"].ToString();
                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                grPrDAO.Delete_Sponsored_Group(groupID, projectID);
                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 参加したグループを削除
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DismissJoinedGroup(string groupID, string projectID)
        {
            // check if parameter valid
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();
                string userID = Session["UserID"].ToString();
                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                if (grPrDAO.Is_Organized(groupID, projectID))
                {
                    string alert = "Nhóm đồng tổ chức, không thế xóa!";
                    return JavaScript("alert('" + alert + "')");
                }
                grPrDAO.Delete_Joined_Group(groupID, projectID);
                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 同管理したユーザーを削除
        /// </summary>
        /// <param name="organizerID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DismissOrganizedUser(string organizerID, string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Delete_Organizer(organizerID, projectID);
                            //update mongo accoutn information projectcount
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            userDAO.Not_Organize_Project(organizerID);

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
        /// 同管理でないメンバーリストを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult MembersNotOrganizer(string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();

                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                var listID = acPrDAO.Get_Member_Not_Organizer(projectID);

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_AccountsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_MembersNotOrganizer", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 同管理でない参加したグループリストを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult JoinedGroupsNotOrganizer(string projectID)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();

                if (!acPrDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
                SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();

                var listID = grPrDAO.Get_JoinedGroups_Not_Organizer(projectID);

                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupsInformation(listID);

                ViewBag.ProjectID = projectID;
                return PartialView("_JoinedgroupsNotOrganizer", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// リストの同管理ユーザーを追加
        /// </summary>
        /// <param name="memberID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrganizedUsers(string[] memberID, string projectID)
        {
            try
            {
                if (memberID == null) return OrganizedUsers(projectID);

                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Add_Organizers(memberID, projectID);
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            userDAO.Batch_Organize_Project(memberID);

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
                    return OrganizedUsers(projectID);
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
        /// リストの同管理グループを追加
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrganizedGroups(string[] groupID, string projectID)
        {
            try
            {
                if (groupID == null) return OrganizedUsers(projectID);

                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO acPrDAO = new SQL_AcPr_Relation_DAO();

                if (acPrDAO.Is_Leader(userID, projectID))
                {
                    SQL_GrPr_Relation_DAO grPrDAO = new SQL_GrPr_Relation_DAO();
                    grPrDAO.Add_Organized_Groups(groupID, projectID);

                    // return group member to content-panel
                    return OrganizedGroups(projectID);
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
        /// プロジェクト情報更新画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateProjectInformation(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, projectID))
                {
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    var result = projectDAO.Get_ProjectInformation(projectID);
                    return PartialView("_UpdateProjectInformation", result);
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
        /// プルジェクト情報を更新
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="newInfo"></param>
        /// <param name="tagsList"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateProjectInformation(string projectID, ProjectInformation newInfo, string[] tagsList)
        {
            // check if parameter valid
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            int tagsCount = tagsList == null ? 0 : tagsList.Count();
            if (tagsCount > 0)
            {
                newInfo.TagsString = tagsList[0];
                if (tagsCount > 1)
                {
                    for (int i = 1; i < tagsCount; i++)
                    {
                        newInfo.TagsString = newInfo.TagsString + ", " + tagsList[i];
                    }
                }
            }

            if (!ModelState.IsValid) return PartialView("_UpdateProjectInformation", newInfo);

            try
            {
                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, projectID))
                {
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    projectDAO.Update_ProjectInformation(projectID, newInfo);
                    return ProjectInformation(projectID);
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
        /// プロジェクトを終了に
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EndProject(string projectID)
        {
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                            sqlDAO.Close(projectID);
                            sqlDAO.Stop_Recruting(projectID);
                            Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                            mongoDAO.Close(projectID);
                            mongoDAO.Stop_Recruiting(projectID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return ProjectInformation(projectID);
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
        /// 参加要求承認の通知を放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requestID"></param>
        /// <param name="projectID"></param>
        /// <param name="notifyID"></param>
        /// <returns></returns>
        public bool SendJoinProjectRequestAccepted(string userID, string requestID, string projectID)
        {
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
            try
            {
                //Send join_project_accepted notify to requested User
                //Create Notify for request user
                SDLink actor = userDAO.Get_SDLink(userID);
                SDLink target = userDAO.Get_SDLink(requestID);
                SDLink destination = projectDAO.Get_SDLink(projectID);
                Notification notify = new Notification(actor, Notify.JOIN_PROJECT_ACCEPTED, target, destination);
                userDAO.Add_Notification(requestID, notify);

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                hubContext.Clients.All.getJoinProjectAccepted(requestID);

                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// ポストを作成
        /// </summary>
        /// <param name="postInfor"></param>
        /// <param name="projectID"></param>
        /// <param name="inSection"></param>
        /// <returns></returns>
        public ActionResult AddPost(PostInformation postInfor, string projectID, string inSection)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = "Error";
                return PartialView("ErrorMessage");
            }
            //Create creator
            string userID = Session["UserID"].ToString();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();

            //If the post is in Public -> creator = destination;
            SDLink creator = new SDLink();
            if (inSection == "Discussion") creator = userDAO.Get_SDLink(userID);
            else creator = projectDAO.Get_SDLink(projectID);
            //create destination 
            SDLink project = projectDAO.Get_SDLink(projectID);

            postInfor.DateCreate = DateTime.Now;
            postInfor.DateLastActivity = DateTime.Now;
            //cretate mongo Post
            Mongo_Post mongo_Post = new Mongo_Post(postInfor);
            mongo_Post.PostInfomation.Creator = creator;
            mongo_Post.PostInfomation.Destination = project;

            //IMAGE Process
            if (postInfor.ImgLink != null && postInfor.ImgLink != "")
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
            sql_Post.ProjectID = projectID;
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
            SendNewPostNotify(userID, sql_Post.PostID, projectID);
            ModelState.Clear();
            ViewBag.ProjectID = projectID;
            if (inSection == "Discussion") return ProjectDiscussion(projectID);
            return ProjectPublic(projectID);
        }
        /// <summary>
        /// Send New post created Notification to all Project's members
        /// 新しいポスト通知を放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <param name="projecID"></param>
        /// <returns></returns>
        public bool SendNewPostNotify(string userID, string postID, string projecID)
        {
            try
            {
                //Get group leader(s)ID 
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                List<string> recieversID = relationDAO.Get_Members(projecID);
                recieversID.AddRange(relationDAO.Get_Leaders(projecID));
                //Send notify to all members except creator
                recieversID.Remove(userID);

                //Get SDLink 
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink actor = userDAO.Get_SDLink(userID);
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                SDLink destination = projectDAO.Get_SDLink(projecID);
                SDLink post = new SDLink(postID);
                //Create Notification
                Notification notif = new Notification(actor, Notify.POST_CREATED_IN_PROJECT, post, destination);

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
        /// ポストリストを取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public PartialViewResult GetPostList(string projectID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> postList = postDAO.Get_Private_Post_By_ProjectID(projectID, 0, 5);
                ViewBag.ProjectID = projectID;
                return PartialView("_PostList", postList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 次のポストリストを取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public PartialViewResult GetPostList_InPublic(string projectID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> postList = postDAO.Get_Public_Post_By_ProjectID(projectID, 0, 5);
                ViewBag.ProjectID = projectID;
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
        public PartialViewResult ShowCommentArea(string postID, string projectID, string cmtCount)
        {
            ViewBag.ProjectID = projectID;
            ViewBag.PostID = postID;
            ViewBag.CommentCount = cmtCount;
            return PartialView("_CommentArea");
        }
        /// <summary>
        /// コメント記載部分を表示
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public PartialViewResult ShowAddCommentArea(string postID, string projectID)
        {
            ViewBag.PostID = postID;
            ViewBag.ProjectID = projectID;
            return PartialView("_PostWriteComment");
        }
        /// <summary>
        /// 残りのコメントを表示
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public PartialViewResult LoadOtherComment(string postID, string projectID)
        {
            try
            {
                //check if user is leader (for delete usage)
                SQL_AcPr_Relation_DAO relation = new SQL_AcPr_Relation_DAO();
                if (relation.Is_Leader(Session["UserID"].ToString(), projectID))
                    ViewBag.IsLeader = true;
                else ViewBag.IsLeader = false;

                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Comment> commentList = postDAO.Get_Comments(postID, 5, 100);
                ViewBag.PostID = postID;
                ViewBag.LoadMore = false;
                return PartialView("_CommentList", commentList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// コメントを記載
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="postID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult AddComment(Comment comment, string postID, string projectID)
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
                SendPostCommentedNotify(userID, postID, projectID);
                return GetCommentList(postID, projectID);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Send Post commented Notification to Post Owner
        /// ポストがコメントされた通知を放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool SendPostCommentedNotify(string userID, string postID, string projectID)
        {
            try
            {
                SQL_AcPo_Relation_DAO relationDAO = new SQL_AcPo_Relation_DAO();
                if (!relationDAO.Is_Owner(userID, postID))
                {
                    string ownerID = relationDAO.Get_Owner(postID);
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    SDLink actor = userDAO.Get_SDLink(userID);
                    SDLink destination = projectDAO.Get_SDLink(projectID);
                    SDLink post = new SDLink(postID);

                    if (!userDAO.Is_Post_Has_Unseen_Notify(ownerID, postID))
                    {
                        Notification notif = new Notification(actor, Notify.POST_CMTED_IN_PROJECT, post, destination);
                        userDAO.Add_Notification(ownerID, notif);
                    }
                    else
                    {
                        string noitfyID = userDAO.Get_PostCmted_NotifyID(ownerID, postID, Notify.POST_CMTED_IN_PROJECT);

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
        public PartialViewResult GetCommentList(string postID, string projectID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Comment> commentList = postDAO.Get_Comments(postID, 0, 5);

                if(Session["UserID"] == null)
                {
                   return PartialView("_CommentList", commentList);
                }
                //check if user is leader (for delete usage)
                SQL_AcPr_Relation_DAO relation = new SQL_AcPr_Relation_DAO();
                if (relation.Is_Leader(Session["UserID"].ToString(), projectID))
                    ViewBag.IsLeader = true;
                else ViewBag.IsLeader = false;
                
                if (postDAO.Get_Cmt_Count(postID) > 5)
                    ViewBag.LoadMore = true;
                ViewBag.PostID = postID;
                ViewBag.ProjectID = projectID;
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
                //return Json(new { status = true, dislike = false });
                return IsLiked(postID);
            }
            catch
            {
                throw;
            }

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
                //write to DB
                mongo_Post_DAO.Set_DateLastActivity(postID, DateTime.Now.ToLocalTime());
                mongo_Post_DAO.Delete_LikerList(postID, Session["UserID"].ToString());

                ViewBag.postID = postID;
                //return Json(new { status = true, dislike = true });
                return IsLiked(postID);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// すでにライクしたかを判定
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
        /// 次のポストリストを取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public PartialViewResult LoadMorePost(string projectID, int times)
        {
            try
            {
                int skipNo = times * 5;
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> postList = postDAO.Get_Private_Post_By_ProjectID(projectID, skipNo, 5);
                ViewBag.ProjectID = projectID;
                return PartialView("_PostList", postList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 公開セクションで次のポストリストを取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public PartialViewResult LoadMorePostInPublic(string projectID, int times)
        {
            try
            {
                int skipNo = times * 5;
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Mongo_Post> postList = postDAO.Get_Public_Post_By_ProjectID(projectID, skipNo, 5);
                ViewBag.ProjectID = projectID;
                return PartialView("_PostList", postList);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get project memebers to assign task
        /// プロジェクトメンバーリストを取得
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        // Get Member to Assign
        public string ProjectMembersToAssign(string sourceID, string target, string name)
        {
            string projectID = "";
            if (target.Equals("plan"))
            {
                Mongo_Plan_DAO planDao = new Mongo_Plan_DAO();
                projectID = planDao.Get_ProjectID(sourceID);
            }

            if (target.Equals("fund"))
            {
                Mongo_Fund_DAO fundDAO = new Mongo_Fund_DAO();
                projectID = fundDAO.Get_ProjectID(sourceID);
            }

            if (target.Equals("finance"))
            {
                Mongo_Finance_DAO financeDAO = new Mongo_Finance_DAO();
                projectID = financeDAO.Get_ProjectID(sourceID);
            }

            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return null;
            }

            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                var listID = relationDAO.Get_AddPaticipants(projectID);
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Search_AccountsInformationForProject(listID, name);

                return JsonConvert.SerializeObject(result);
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
        /// <param name="projectID"></param>
        /// <param name="notifyID"></param>
        /// <param name="postID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult SeenNewPostNotify(string userID, string projectID, string notifyID, string postID, string type)
        {
            SDLink result = null;
            List<Mongo_Post> posts = new List<Mongo_Post>();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                //Check if Post is deleted 
                if (!postDAO.Is_Exist(postID))
                {
                    userDAO.Set_Notification_IsSeen(userID, notifyID);
                    return Json(new { error = true, message = "Bài viết này không tồn tại." });
                }
                posts.Add(postDAO.Get_Mg_Post_By_ID(postID));
                SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                if (sqlDAO.IsActivate(projectID))
                {
                    Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                    result = mongoDAO.Get_SDLink(projectID);
                }

                if (Session["UserID"] != null)
                {
                    SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                    if (Session["UserID"].ToString() == "Admin")
                        ViewBag.IsAdmin = true;
                    else
                    {
                        ViewBag.IsJoined = relationDAO.Is_Joined(userID, projectID);
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
                ViewBag.ProjectID = projectID;
                ViewBag.IsJoined = true;
                var projectHome = RenderRazorViewToString(this.ControllerContext, "ProjectHome", result);
                //var projectDiscusson = RenderRazorViewToString(this.ControllerContext, "_ProjectDiscussion", null);
                var post = RenderRazorViewToString(this.ControllerContext, "_PostList", posts);
                //return Json(new { projectHome, projectDiscusson, post, type });
                return Json(new { projectHome, post, type });
            }
        }
        /// <summary>
        /// ビューをレンダー
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
        /// 募集状態を設定
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StartRecruiting(string projectID)
        {
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                            sqlDAO.Start_Recruting(projectID);

                            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                            projectDAO.Start_Recruiting(projectID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return ProjectInformation(projectID);
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
        /// 募集を終わりに
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StopRecruiting(string projectID)
        {
            if (String.IsNullOrWhiteSpace(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                            sqlDAO.Stop_Recruting(projectID);

                            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                            projectDAO.Stop_Recruiting(projectID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return ProjectInformation(projectID);
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
        public PartialViewResult ActionToPost(string postID, string projectID)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = "";
                return PartialView("ErrorMessage");
            }
            string userID = Session["UserID"].ToString();
            SQL_AcPo_Relation_DAO relation = new SQL_AcPo_Relation_DAO();
            if (relation.Is_Owner(userID, postID))
            {
                ViewBag.IsOwner = true;
            }
            else ViewBag.IsOwner = false;

            SQL_AcPr_Relation_DAO prRelation = new SQL_AcPr_Relation_DAO();
            if (prRelation.Is_Leader(userID, projectID))
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

            ViewBag.ProjectID = projectID;
            ViewBag.PostID = postID;
            return PartialView("_ActionToPost");
        }
        public ActionResult DeletePost(string postID, string projectID)
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
            if (permission) return ProjectPublic(projectID);
            return ProjectDiscussion(projectID);
        }
        public ActionResult PinPost(string postID, string projectID)
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
                        if (permission) currentPinID = mgPost.Get_PinnedPost_ID(projectID, true);
                        else currentPinID = mgPost.Get_PinnedPost_ID(projectID, false);

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
            if (permission) return ProjectPublic(projectID);
            return ProjectDiscussion(projectID);
        }
        public ActionResult UnpinPost(string postID, string projectID)
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
            if (permission) return ProjectPublic(projectID);
            return ProjectDiscussion(projectID);
        }
        public PartialViewResult GetPinnedPost(string projectID)
        {
            Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
            try
            {
                Mongo_Post pinPost = postDAO.Get_Mg_PinnedPost(projectID, false);

                ViewBag.ProjectID = projectID;
                return PartialView("_PinnedPost", pinPost);
            }
            catch
            {
                throw;
            }
        }
        public PartialViewResult GetPinnedPost_InPublic(string projectID)
        {
            Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
            try
            {
                Mongo_Post pinPost = postDAO.Get_Mg_PinnedPost(projectID, true);

                ViewBag.ProjectID = projectID;
                return PartialView("_PinnedPost", pinPost);
            }
            catch
            {
                throw;
            }
        }
        public PartialViewResult AddImageToPost()
        {
            string userID = Session["UserID"].ToString();
            ViewBag.Action = "UploadPostImage";
            ViewBag.Controller = "Project";
            ViewBag.ID = userID;
            return PartialView("_ImageUploadInPost");
        }
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

                    MemoryStream ms = new MemoryStream();
                    WebImage img = new WebImage(uploadPath);

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
                    img.Save(uploadPath);

                    ViewBag.ImageLink = uploadPath;
                    return Json(uploadPath);
                }
                else return Json(null);

            }
            else return PartialView("_ImageUpload");
        }
        public ActionResult ViewPost(string notifyID, string postID, string projectID)
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
                ViewBag.ProjectID = projectID;
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
        public ActionResult DeleteComment(string postID, string commentID, string projectID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                postDAO.Delete_Comment(postID, commentID);
                return GetCommentList(postID, projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /////////////////Album In Project///////////////////////
        public int checkRoleAlbum(string albumID, string targetID)
        {
            int role;
            string userID = Session["UserID"].ToString();
            SQL_AcAl_Relation_DAO relation = new SQL_AcAl_Relation_DAO();
            SQL_AcPr_Relation_DAO relaPr = new SQL_AcPr_Relation_DAO();
            if (relation.Is_Creator(userID, albumID) || relaPr.Is_Leader(userID, targetID))
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
                string projectID = mongo_Album_DAO.Get_TargetID(albumID);
                SQL_AcPr_Relation_DAO relation = new SQL_AcPr_Relation_DAO();
                if (relation.Is_Leader(userID, projectID)) ViewBag.Role = "Leader";
            }
            ImageInformation mongo_Image = new ImageInformation();
            var model = mongo_Album_DAO.Get_Image_By_AlbumID(albumID);
            ViewBag.AlbumID = albumID;
            ViewBag.ProjectID = targetID;
            Session["Album"] = albumID;
            return PartialView("_AlbumShowImage", model);
        }
        public PartialViewResult GetAlbumList(string projectID)
        {
            if (Session["UserID"] != null)
            {
                string userID = Session["UserID"].ToString();
                //Leader will be able to Post in Public Section(in next View returned)
                SQL_AcPr_Relation_DAO relation = new SQL_AcPr_Relation_DAO();
                if (relation.Is_Leader(userID, projectID)) ViewBag.Role = "Leader";
                else ViewBag.Role = "Member";
                ViewBag.UserID = userID;
                ViewBag.targetID = projectID;
            }
            
            try
            {
                Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                List<Mongo_Album> albumList = albumDAO.Get_Private_Album_By_TargetID(projectID, 0, 50);
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
        public PartialViewResult AlbumShowCommentArea(string albumID, string projectID, string cmtCount)
        {
            ViewBag.ProjectID = projectID;
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
        public PartialViewResult AlbumShowAddCommentArea(string albumID, string projectID)
        {
            ViewBag.GroupID = projectID;
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
        public ActionResult AlbumAddComment(Comment comment, string albumID, string projectID)
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

                return AlbumGetCommentList(albumID,projectID);
            }
            catch
            {
                throw;
            }
        }
        public ActionResult AlbumDeleteComment(string albumID, string commentID, string projectID)
        {
            try
            {
                Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                albumDAO.Delete_Comment(albumID, commentID);
                return AlbumGetCommentList(albumID,projectID);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public PartialViewResult AlbumGetCommentList(string albumID,string projectID)
        {
            string userID = Session["UserID"].ToString();
            SQL_AcPr_Relation_DAO relation = new SQL_AcPr_Relation_DAO();
            if (relation.Is_Leader(userID, projectID)) { ViewBag.Role = "Leader"; }
            else if (relation.Is_Member(userID, projectID))
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
        public ActionResult DeleteTempPic()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                string path = Server.MapPath("/Images/Post/" + userID + ".jpg");
                System.IO.File.Delete(path);
                return Json(true);
            }
            catch
            {
                throw;
            }
        }
    }
}