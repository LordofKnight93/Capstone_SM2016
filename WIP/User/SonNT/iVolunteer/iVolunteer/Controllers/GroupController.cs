using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
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
    public class GroupController : Controller
    {
        // GET : CreateGroup
        [HttpGet]
        [Authorize]
        public ActionResult CreateGroup()
        {
            return View();
        }

        public ActionResult CreateGroup(GroupInformation groupInfo)
        {
            ///
            /// This would be check permission and validate data, will add later
            ///

            
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
            Mongo_Group mongo_Group = new Mongo_Group(creator,groupInfo);

            //craete group SDLink
            SDLink group = new SDLink(mongo_Group.GroupInformation);
            //create sql Group
            SQL_Group sql_Group = new SQL_Group();
            sql_Group.GroupID = mongo_Group.GroupInformation.GroupID;
            sql_Group.IsActivate = true;

            // create first relation
            SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
            relation.UserID = creator.ID ;
            relation.GroupID = mongo_Group.GroupInformation.GroupID;
            relation.Relation = Relation.LEADER_RELATION;
            
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

            ViewBag.Message = "Tạo nhóm tình nguyện thành công";
            return RedirectToAction("Newfeed","Home");
        }
        public ActionResult ChangeAvatar(string id)
        {
            ViewBag.Option = "GroupAvatar";
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
        public ActionResult ChangeCover(string id)
        {
            ViewBag.Option = "GroupCover";
            ViewBag.ID = id;
            return View("_ImageUpload");
        }
        [HttpPost]
        public ActionResult UploadCover( string id)
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

        public ActionResult Information(string groupID)
        {
            GroupInformation result = null;
            try
            {
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                result = groupDAO.Get_GroupInformation(groupID);
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
            return PartialView(result);
        }
        public ActionResult AvatarCover(string groupID)
        {
            SDLink result = null;
            try
            {
                //get data
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                SQL_AcGr_Relation_DAO relationDAO = new SQL_AcGr_Relation_DAO();
                result = groupDAO.Get_SDLink(groupID);
                //get ralation 
                if (Session["UserID"] == null)
                {
                    ViewBag.CanChange = "false";
                }
                else
                {
                    string userID = Session["UserID"].ToString();
                    if (relationDAO.Get_Specific_Relation(userID, groupID) == Relation.LEADER_RELATION)
                        ViewBag.CanChange = "true";
                    else ViewBag.CanChange = "false";
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
            return PartialView("_AvatarCover", result);
        }

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
                return PartialView("ErrorMessage");
            }
            if (result == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return View("GroupHome", result);
        }

        public ActionResult GroupSearch(string name, int page)
        {
            List<GroupInformation> result = new List<GroupInformation>();
            if(page <= 0)
            {
                page = 1;
            }

            try
            {
                Mongo_Group_DAO mongoDAO = new Mongo_Group_DAO();
                //search depend on role
                if (Session["Role"] != null && Session["Role"].ToString() == "Admin")
                    result = mongoDAO.Search_Group_By_Name(name, 10 * (page - 1), 10);
                else
                    result = mongoDAO.Search_Group_By_Name(name,Status.IS_ACTIVATE, 10 * (page - 1), 10);

                    ViewBag.CurrentPage = page;
                    return PartialView("_GroupResult", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult GroupRequests(string groupID)
        {
            List<RequestItem> result = new List<RequestItem>();
            try
            {
                Mongo_Group_DAO mongoDAO = new Mongo_Group_DAO();
                result = mongoDAO.Get_RequestList(groupID);
                ViewBag.GroupID = groupID;
                return PartialView("_GroupRequests", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult DeniedRequest(string groupID, string requestID)
        {
            try
            {
                Mongo_Group_DAO mongoDAO = new Mongo_Group_DAO();
                mongoDAO.Delete_Request(groupID, requestID);
                ViewBag.Message = "Đã từ chối yêu cầu.";
                return View("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult AcceptRequest(string groupID, string requestID)
        {
            try
            {
                Mongo_Group_DAO mongo_Group_DAO = new Mongo_Group_DAO();
                //get request information
                RequestItem request = mongo_Group_DAO.Get_Request(groupID, requestID);

                SQL_AcGr_Relation_DAO sqlDAO = new SQL_AcGr_Relation_DAO();
                //create sql realation
                SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                relation.UserID = request.Actor.ID;
                relation.GroupID = groupID;
                relation.Relation = Relation.MEMBER_RELATION;
                //write relation
                sqlDAO.Add_Relation(relation);
                //add user to group
                mongo_Group_DAO.Add_JoinedUser(groupID, request.Actor);
                //add group to user information
                Mongo_User_DAO mongo_User_DAO = new Mongo_User_DAO();
                mongo_User_DAO.Add_JoinedGroup(request.Actor.ID, request.Destination);
                //delete request
                mongo_Group_DAO.Delete_Request(groupID, requestID);
                ViewBag.Message = "Đã chấp nhận yêu cầu.";
                return View("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult group_master()
        {
            return View();
        }

        public PartialViewResult DisplayPublic()
        {
            return PartialView("public_layout");
        }

        public PartialViewResult DisplayDiscussion()
        {
            return PartialView("discussion_layout");
        }
    }
}