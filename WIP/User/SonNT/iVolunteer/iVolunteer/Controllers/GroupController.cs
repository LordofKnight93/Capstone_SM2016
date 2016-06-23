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
        // GET: Group
        public ActionResult Index()
        {
            return View();
        }
        // GET : CreateGroup
        [HttpGet]
        public ActionResult CreateGroup()
        {
            return View();
        }

        public ActionResult CreateGroup(GroupInformation groupInfo)
        {
            if (!ModelState.IsValid) return View();
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

            //create sql Group
            SQL_Group sql_Group = new SQL_Group();
            sql_Group.GroupID = mongo_Group.GroupInformation.GroupID;
            sql_Group.IsActivate = true;

            // create first relation
            SQL_User_Group relation = new SQL_User_Group();
            relation.UserID = creator.ID ;
            relation.GroupID = mongo_Group.GroupInformation.GroupID;
            relation.RelationType = Relation.LEADER_RELATION;
            

            //this code will change user information, will add later

            //start transaction
            
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // create DAO instance
                    Mongo_Group_DAO mongo_Group_DAO = new Mongo_Group_DAO();
                    SQL_Group_DAO sql_Group_DAO = new SQL_Group_DAO();
                    SQL_User_Group_DAO sql_User_Group_DAO = new SQL_User_Group_DAO();

                    //write to DB
                    sql_Group_DAO.Add_Group(sql_Group);
                    sql_User_Group_DAO.Add_Relation(relation);
                    mongo_Group_DAO.Add_Group(mongo_Group);

                    // copy default avatar and cover
                    FileInfo avatar = new FileInfo(Server.MapPath("~") + Default.DEFAULT_AVATAR);
                    avatar.CopyTo(Server.MapPath("~") + "/Images/Group/Avatar/" + sql_Group.GroupID + ".jpg");
                    FileInfo cover = new FileInfo(Server.MapPath("~") + Default.DEFAULT_COVER);
                    cover.CopyTo(Server.MapPath("~") + "/Images/Group/Cover/" + sql_Group.GroupID + ".jpg");

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = "Có lỗi xảy ra, vui lòng thử lại sau ít phút";
                    return View();
                }
            }

            ViewBag.Message = "Tạo nhóm tình nguyện thành công";
            return RedirectToAction("Newfeed","Home");
        }

        public ActionResult Information(string groupID)
        {
            GroupInformation result = new GroupInformation();
            try
            {
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                result = groupDAO.Get_GroupInformation(groupID);
            }
            catch
            {
                result = null;
            }
            return View(result);
        }
        public ActionResult AvatarCover(string groupID)
        {
            SDLink result = new SDLink();
            try
            {
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                result = groupDAO.Get_SDLink(groupID);
            }
            catch
            {
                RedirectToAction("FrontPage", "Home");
            }
            return View("_AvatarCover", result);
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