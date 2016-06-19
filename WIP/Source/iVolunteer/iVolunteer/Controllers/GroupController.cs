using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.Mvc;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Models.Data_Access_Object.MongoDB;
using iVolunteer.Models.Data_Access_Object.SQL;
using iVolunteer.Common;
using MongoDB.Bson;

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
            ///
            /// This would be check permission and validate data, will add later
            ///

            
            //set missing information
            groupInfo.DateCreate = DateTime.Now;
            groupInfo.MemberCount = 1;
            groupInfo.AvtImgLink = Constant.DEFAULT_AVATAR;
            groupInfo.CoverImgLink = Constant.DEFAULT_COVER;
            groupInfo.IsActivate = true;

            //craete creator
            UserSD creator = new UserSD();
            creator.UserID = Session["UserID"].ToString();
            creator.DisplayName = Session["DisplayName"].ToString();
            creator.AvtImgLink = Session["Avatar"].ToString();

            //create mongo Group
            Mongo_Group mongo_Group = new Mongo_Group(creator,groupInfo);

            //create sql Group
            SQL_Group sql_Group = new SQL_Group();
            sql_Group.GroupID = mongo_Group._id.ToString();
            sql_Group.IsActivate = true;

            // create first relation
            SQL_User_Group relation = new SQL_User_Group();
            relation.UserID = creator.UserID ;
            relation.GroupID = mongo_Group._id.ToString();
            relation.RelationType = Constant.DIRECT_RELATION;

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
                    mongo_Group_DAO.Add_Group(mongo_Group);
                    sql_Group_DAO.Add_Group(sql_Group);
                    sql_User_Group_DAO.Add_Relation(relation);
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

        public ActionResult GroupInformation(string groupID)
        {
            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            var result = groupDAO.Get_GroupInformation(groupID);
            return View(result);
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