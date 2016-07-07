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
            if (!ModelState.IsValid) return View();

            //set missing information
            groupInfo.DateCreate = DateTime.Now;
            groupInfo.MemberCount = 1;
            groupInfo.IsActivate = true;

            //craete creator
            SDLink creator = new SDLink();
            creator.ID = Session["UserID"].ToString();
            creator.DisplayName = Session["DisplayName"].ToString();
            creator.Handler = Handler.USER;

            //create mongo Group
            Mongo_Group mongo_Group = new Mongo_Group(creator, groupInfo);

            //craete group SDLink
            SDLink group = new SDLink(mongo_Group.GroupInformation);
            //create sql Group
            SQL_Group sql_Group = new SQL_Group();
            sql_Group.GroupID = mongo_Group.GroupInformation.GroupID;
            sql_Group.IsActivate = true;

            // create first relation
            SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
            relation.UserID = creator.ID;
            relation.GroupID = mongo_Group.GroupInformation.GroupID;
            relation.Relation = Relation.LEADER_RELATION;
            relation.Status = Status.ACCEPTED;

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
                    sql_User_Group_DAO.Add_Relation(relation);
                    mongo_Group_DAO.Add_Group(mongo_Group);
                    mongo_User_DAO.Add_JoinedGroup(creator.ID, group);

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

            return RedirectToAction("GroupHome", "Group", new { groupID = group.ID });
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
            return View("_ImageUpload");
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
            else return View("_ImageUpload");
        }

        [HttpGet]
        public ActionResult ChangeCover(string id)
        {
            //check permission here

            ViewBag.Action = "UploadCover";
            ViewBag.Controller = "Group";
            ViewBag.ID = id;
            return View("_ImageUpload");
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
            else return View("_ImageUpload");
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
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_ActivityHistory(groupID);
                return PartialView("_ActivityHistory", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

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
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_CurrentProjects(groupID);
                return PartialView("_CurrentProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult GroupStructure(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            //get group structure
            try
            {
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_GroupStructure(groupID);

                //get current user subrole
                if (Session["UserID"] == null)
                {
                    ViewBag.IsLeader = false;
                    return PartialView("_GroupStructure", result);
                }
                else
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                    if (relationDAO.Is_Leader(userID, groupID))
                        ViewBag.IsLeader = true;
                }

                ViewBag.GroupID = groupID;
                return PartialView("_GroupStructure", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult RequestList(string groupID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(groupID))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var result = groupDAO.Get_RequestList(groupID);
                ViewBag.GroupID = groupID;
                return PartialView("_RequestList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult DeniedRequest(string groupID, string requestID)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                    SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                    //get request
                    var request = groupDAO.Get_Request(groupID, requestID);
                    //delete relation
                    relationDAO.Delete_Specific_Relation(request.Actor.ID, request.Destination.ID, Relation.MEMBER_RELATION);
                    //delete request
                    groupDAO.Delete_Request(groupID, requestID);
                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }

            ViewBag.Message = "Đã từ chối";
            return View("ErrorMessage");
        }

        public ActionResult AcceptRequest(string groupID, string requestID)
        {
            try
            {
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                //get request
                var request = groupDAO.Get_Request(groupID, requestID);

                using(var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();

                        //update relation
                        relationDAO.Accept_Member(request.Actor.ID, request.Destination.ID);
                        //delete follow relation
                        relationDAO.Delete_Specific_Relation(request.Actor.ID, request.Destination.ID, Relation.FOLLOW_RELATION);
                        //add user to group
                        groupDAO.Add_JoinedUser(request.Destination.ID, request.Actor);
                        //add grup to user joined groups
                        userDAO.Add_JoinedGroup(request.Actor.ID, request.Destination);
                        //delete request
                        groupDAO.Delete_Request(groupID, requestID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                ViewBag.Message = " Đã đồng ý";
                return View("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult GroupSearch(string name, int page)
        {
            try
            {
                if (page <= 0) page = 1;

                List<GroupInformation> result = new List<GroupInformation>();
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                if(Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = groupDAO.Search_Group_By_Name(name, 10 * (page - 1), 10);
                else result = groupDAO.Search_Group_By_Name(name, Status.IS_ACTIVATE, 10 * (page - 1), 10);
                ViewBag.CurentPage = page;

                return PartialView("_GroupResult", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }
    }
}