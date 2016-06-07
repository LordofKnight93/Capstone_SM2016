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
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.TableClass;
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

            //craete leader
            UserSD leader = new UserSD();
            leader.UserID = Session["UserID"].ToString();
            leader.DisplayName = Session["DisplayName"].ToString();
            leader.AvtImgLink = Session["Avatar"].ToString();
            //craete member
            Member member = new Member();
            member.JoinDate = DateTime.Now.Date;
            member.User = leader;

            ObjectId _id = ObjectId.GenerateNewId();
            //create mongo Group
            Mongo_Group mongo_Group = new Mongo_Group();
            mongo_Group._id = _id;
            //set information
            mongo_Group.GroupInformation = groupInfo;
            //set structure
            mongo_Group.GroupStructure.Creator = leader;
            mongo_Group.GroupStructure.Leaders.Add(leader);
            mongo_Group.GroupStructure.JoinedUsers.Add(member);

            //create sql Group
            SQL_Group sql_Group = new SQL_Group();
            sql_Group.GroupID = _id.ToString();
            sql_Group.IsActivate = true;

            // crate first relation
            SQL_User_Group relation = new SQL_User_Group();
            relation.UserID = leader.UserID ;
            relation.GroupID = _id.ToString();
            relation.RelationType = Constant.DIRECT_RELATION;

            //start transaction
            using (var transaction = new TransactionScope())
            {
                try
                {
                    Mongo_Group_DAO.Add_Group(mongo_Group);
                    SQL_Group_DAO.Add_Group(sql_Group);
                    SQL_User_Group_DAO.Add_Relation(relation);
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
    }
}