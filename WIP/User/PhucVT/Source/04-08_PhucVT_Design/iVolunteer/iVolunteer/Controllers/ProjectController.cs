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

namespace iVolunteer.Controllers
{
    public class ProjectController : Controller
    {
        [HttpGet]
        public ActionResult CreateProject()
        {
            return PartialView("_CreateProject");
        }
        [HttpPost]
        public ActionResult CreateProject(ProjectInformation projectInfo, string[] tagsList)
        {
            if (!ModelState.IsValid) return PartialView("_CreateProject", projectInfo);

            if (projectInfo.DateStart > projectInfo.DateEnd)
            {
                ViewBag.Message = " Ngày bắt đầu không thể muộn hơn ngày kết thúc! ";
                return PartialView("_CreateProject",projectInfo); 
            }

            if (projectInfo.DateStart < DateTime.Now)
            {
                ViewBag.Message = "Ngày bắt đầu không thể sớm hơn hôm nay! ";
                return PartialView("_CreateProject", projectInfo);
            }

            //set missing information for project
            projectInfo.DateCreate = DateTime.Now;
            projectInfo.MemberCount = 1;
            projectInfo.InProgress = Status.ONGOING;
            projectInfo.IsActivate = Status.IS_ACTIVATE;
            projectInfo.IsRecruit = Status.IS_RECRUITING;

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

            string userID = Session["UserID"].ToString();

            //create mongo project
            Mongo_Project mongo_Project = new Mongo_Project(projectInfo);

            //create sql project
            SQL_Project sql_Project = new SQL_Project();
            sql_Project.ProjectID = mongo_Project.ProjectInformation.ProjectID;
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
        [HttpGet]
        public ActionResult ProjectHome(string projectID)
        {
            SDLink result = null;
            try
            {
                SQL_Project_DAO sqlDAO = new SQL_Project_DAO();
                if (sqlDAO.IsActivate(projectID))
                {
                    Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                    result = mongoDAO.Get_SDLink(projectID);
                }

                if (Session["UserID"] != null)
                {
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    if (Session["UserID"].ToString() == "Admin")
                        ViewBag.IsAdmin = true;
                    else
                    {
                        string userID = Session["UserID"].ToString();
                        ViewBag.IsJoined = relationDAO.Is_Joined(userID, projectID);
                        ViewBag.IsSponsored = relationDAO.Is_Sponsor(userID, projectID);
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
            else return View("ProjectHome", result);
        }
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
        [HttpGet]
        public ActionResult ChangeAvatar(string id)
        {
            //check permission here

            ViewBag.Action = "UploadAvatar";
            ViewBag.Controller = "Project";
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
                string uploadPath = Server.MapPath("/Images/Project/Avatar/" + id + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("ProjectHome", "Project", new { projectID = id });
            }
            else return PartialView("_ImageUpload");
        }
        [HttpGet]
        public ActionResult ChangeCover(string id)
        {
            //check permission here

            ViewBag.Action = "UploadCover";
            ViewBag.Controller = "Project";
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
                string uploadPath = Server.MapPath("/Images/Project/Cover/" + id + ".jpg");
                file.SaveAs(uploadPath);
                return RedirectToAction("ProjectHome", "Project", new { projectID = id });
            }
            else return PartialView("_ImageUpload");
        }
        public ActionResult ProjectInformation(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var result = projectDAO.Get_ProjectInformation(projectID);
                if(Session["UserID"] != null)
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
        public ActionResult OrganizedUsers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        public ActionResult ProjectLeaders(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        public ActionResult SponsoredUsers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        public ActionResult ProjectMembers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        public ActionResult JoinedGroups(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        public ActionResult SponsoredGroups(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        public ActionResult OrganizedGroups(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        public ActionResult GroupJoinRequests(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
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
        public ActionResult GroupSponsorRequests(string projectID)
        {
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
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
        public ActionResult UserJoinRequests(string projectID)
        {
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
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
        public ActionResult UserSponsorRequests(string projectID)
        {
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
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
        public ActionResult SearchProject()
        {
            var searchModel = TempData["SearchModel"] as SearchModel;
            return View("SearchProject", searchModel);
        }
        public ActionResult AdvancedSearchProject(SearchModel searchModel)
        {
            try
            {
                if (searchModel == null) searchModel = new SearchModel();

                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();

                List<ProjectInformation> result = new List<ProjectInformation>();
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = projectDAO.Project_Search(searchModel, 0,10);
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

                ViewBag.NextPage = page + 1;

                return PartialView("_ProjectResult", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
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

        public ActionResult ProjectDiscussion(string projectID)
        {
            ViewBag.InSection = "Discussion";
            ViewBag.ProjectID = projectID;
            return PartialView("_ProjectDiscussion");
        }

        public ActionResult ProjectGallery()
        {
            return PartialView("_ProjectGallery");
        }

        public ActionResult ProjectPlan(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="memberID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool SendPromotionNotify(string userID, string memberID, string projectID)
        {
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Project_DAO groupDAO = new Mongo_Project_DAO();
            try
            {
                //Send Friend REquest accepted to requested User
                //Create Notify for request user
                SDLink leader = userDAO.Get_SDLink(userID);
                SDLink target = userDAO.Get_SDLink(memberID);
                SDLink destination = groupDAO.Get_SDLink(projectID);
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
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            if (relationDAO.Delete_Member(memberID, projectID))
                            {
                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                userDAO.Out_Group(memberID);
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
                string notifyID = userDAO.Get_JoinProject_NotifyID(userID, requestID, projectID);
                //Get Leaders list
                List<string> leadersID = relationDAO.Get_Leaders(projectID);
                leadersID.Remove(userID);

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            relationDAO.Accept_Join_Request(requestID, projectID);
                            userDAO.Join_Project(requestID);
                            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                            projectDAO.Members_Join(projectID,1);

                            //Set IsSeen to other leaders's this requestNotify.
                            foreach (var leader in leadersID)
                            {
                                userDAO.Set_Notification_IsSeen(leader, notifyID);
                            }
                            SendJoinProjectRequestAccepted(userID, requestID, projectID, notifyID);

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
        [HttpGet]
        public ActionResult CreateSponsor(string projectID)
        {
            ViewBag.ProjectID = projectID;
            return PartialView("_CreateSponsor");
        }
        [HttpPost]
        public ActionResult CreateSponsor(string projectID, Sponsor guest)
        {
            string err = "";
            bool isValid = true;

            if (!ValidationHelper.IsValidEmail(guest.SponsorEmail))
            {
                err = Error.EMAIL_INVALID + Environment.NewLine;
                isValid = false;
            }

            if (!ValidationHelper.IsValidPhone(guest.SponsorPhone))
            {
                err = Error.PHONE_INVALID + Environment.NewLine;
                isValid = false;
            }

            if (!isValid)
            {
                ViewBag.ProjectID = projectID;
                ViewBag.Message = err;
                return PartialView("_CreateSponsor", guest);
            }
            //set mising information
            guest.SponsorID = ObjectId.GenerateNewId().ToString();
            guest.Status = Status.PENDING;

            try
            {
                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                projectDAO.Add_GuestSponsor(projectID, guest);

                ViewBag.Message = "Gửi yêu cầu thành công";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
        public ActionResult SponsoredGuests(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        public ActionResult GuestSponsorRequests(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID))
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
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult SuggestedUsers(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID)|| Session["UserID"] == null)
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
        public ActionResult InviteUsers(string[] friendID, string projectID)
        {
            try
            {
                if (friendID == null) return RedirectToAction("FriendNotInProject","Account", new { projectID = projectID });

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
        public ActionResult JoinedGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || String.IsNullOrEmpty(groupID))
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
        public ActionResult OrganizedGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || String.IsNullOrEmpty(groupID))
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
        public ActionResult SponsoredGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || String.IsNullOrEmpty(groupID))
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
        public ActionResult SponsorRequestGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || String.IsNullOrEmpty(groupID))
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
        public ActionResult JoinRequestGroupMembers(string groupID, string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || String.IsNullOrEmpty(groupID))
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
                            if (grPrDAO.Accept_Sponsor_Request(requestID, projectID))
                            {
                                var acceptList = acPrDAO.Accept_Sponsor_Requested_Group_Members(requestID, projectID);

                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                userDAO.Batch_Sponsor_Project(acceptList);
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
                grPrDAO.Delete_Joined_Group(groupID, projectID);
                return null;
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
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

                if(!acPrDAO.Is_Leader(userID, projectID))
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
        [HttpGet]
        public ActionResult UpdateProjectInformation(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
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
        [HttpPost]
        public ActionResult UpdateProjectInformation(string projectID, ProjectInformation newInfo, string[] tagsList)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
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

        public ActionResult EndProject(string projectID)
        {
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
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
                            Mongo_Project_DAO mongoDAO = new Mongo_Project_DAO();
                            mongoDAO.Close(projectID);

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
                    return PartialView("ErrorMessage");
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public JsonResult AcceptRequestOnNotify(string userID, string requestID, string projectID, string notifyID)
        {
            SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
            using (var transaction = new TransactionScope())
            {
                try
                {
                    relationDAO.Accept_Join_Request(requestID, projectID);
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    userDAO.Join_Project(requestID);
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    projectDAO.Members_Join(projectID, 1);

                    //Send Join Project request Accepted to requested User
                    SendJoinProjectRequestAccepted(userID, requestID, projectID, notifyID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }

        }
        public bool SendJoinProjectRequestAccepted(string userID, string requestID, string projectID, string notifyID)
        {
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
            try
            {
                //Set is seen
                userDAO.Set_Notification_IsSeen(userID, notifyID);

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
        public JsonResult DenyRequestOnNotify(string userID, string requestID, string projectID, string notifyID)
        {
            try
            {
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    relationDAO.Delete_Join_Request(requestID, projectID);
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    userDAO.Delete_Notification(userID, notifyID);
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return Json(false);
            }
        }
        public PartialViewResult AddPost(PostInformation postInfor, string projectID, string inSection)
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
            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
            SDLink project = projectDAO.Get_SDLink(projectID);

            postInfor.DateCreate = DateTime.Now;
            postInfor.DateLastActivity = DateTime.Now;
            //cretate mongo Post
            Mongo_Post mongo_Post = new Mongo_Post(postInfor);
            mongo_Post.PostInfomation.Creator = creator;
            mongo_Post.PostInfomation.Destination = project;

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
            ViewBag.ProjectID = projectID;
            if (inSection == "Discussion") return GetPostList(projectID);
            return GetPostList_InPublic(projectID);
        }
        /// <summary>
        /// Send New post created Notification to all Project's members except Post creator
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
        public PartialViewResult ShowAddCommentArea(string postID, string projectID)
        {
            ViewBag.PostID = postID;
            ViewBag.ProjectID = projectID;
            return PartialView("_PostWriteComment");
        }
        public PartialViewResult LoadOtherComment(string postID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Comment> commentList = postDAO.Get_Comments(postID, 5, 100);
                ViewBag.PostID = postID;
                ViewBag.IsFirstTime = false;
                return PartialView("_CommentList", commentList);
            }
            catch
            {
                throw;
            }
        }
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
            comment.DateCreate = DateTime.Now.ToLocalTime();
            comment.Creator = creator;

            try
            {
                mongo_Post_DAO.Set_DateLastActivity(postID, comment.DateCreate);
                mongo_Post_DAO.Add_Comment(postID, comment);
                SendPostCommentedNotify(userID, postID, projectID);
                return GetCommentList(postID);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Send Post commented Notification to Post Owner
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
        public PartialViewResult GetCommentList(string postID)
        {
            try
            {
                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
                List<Comment> commentList = postDAO.Get_Comments(postID, 0, 5);
                ViewBag.PostID = postID;
                ViewBag.IsFirstTime = true;
                return PartialView("_CommentList", commentList);
            }
            catch
            {
                throw;
            }
        }
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

        // Get Member to Assign
        public string ProjectMembersToAssign(string planPhaseID, string name)
        {
            //Mongo_Finance_DAO financeDAO = new Mongo_Finance_DAO();
            //var projectID = financeDAO.Get_ProjectID(financeID);
            Mongo_Plan_DAO planDao = new Mongo_Plan_DAO();
            var projectID = planDao.Get_ProjectID(planPhaseID);
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
        public ActionResult SeenNewPostNotify(string userID, string projectID, string notifyID, string postID, string type)
        {
            SDLink result = null;
            List<Mongo_Post> posts = new List<Mongo_Post>();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            try
            {

                Mongo_Post_DAO postDAO = new Mongo_Post_DAO();
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
                var projectDiscusson = RenderRazorViewToString(this.ControllerContext, "_ProjectDiscussion", null);
                var post = RenderRazorViewToString(this.ControllerContext, "_PostList", posts);
                return Json(new { projectHome, projectDiscusson, post, type });
            }
        }
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

    }
}