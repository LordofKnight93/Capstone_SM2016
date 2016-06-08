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
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Index()
        {
            return RedirectToAction("CreateProject");
        }
        //GET : CreateProject
        [HttpGet]
        public ActionResult CreateProject()
        {
            return View();
        }
        //POST : CreateProject
        [HttpPost]
        public ActionResult CreateProject( ProjectInformation projectInfo )
        {
            ///
            /// This would be check permission and validate data, will add later
            ///

            //set missing information for project
            projectInfo.DateCreate = DateTime.Now;
            projectInfo.MemberCount = 1;
            projectInfo.AvtImgLink = Constant.DEFAULT_AVATAR;
            projectInfo.CoverImgLink = Constant.DEFAULT_COVER;
            projectInfo.IsActivate = true;
            projectInfo.IsRecruit = true;

            //create creator
            UserSD creator = new UserSD();
            creator.UserID = Session["UserID"].ToString();
            creator.DisplayName = Session["DisplayName"].ToString();
            creator.AvtImgLink = Session["Avatar"].ToString();

            //create mongo project
            Mongo_Project mongo_Project = new Mongo_Project(creator, projectInfo);

            //create sql project
            SQL_Project sql_Project = new SQL_Project();
            sql_Project.ProjectID = mongo_Project._id.ToString();
            sql_Project.IsActivate = true;

            //create first relation
            SQL_User_Project relation = new SQL_User_Project();
            relation.UserID = creator.UserID;
            relation.ProjectID = mongo_Project._id.ToString();
            relation.RelationType = Constant.DIRECT_RELATION;

            //start transaction
            using (var transaction = new TransactionScope())
            {
                try
                {
                    Mongo_Project_DAO.Add_Project(mongo_Project);
                    SQL_Project_DAO.Add_Project(sql_Project);
                    SQL_User_Project_DAO.Add_Relation(relation);
                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = "Có lỗi xảy ra, vui lòng thử lại sau ít phút";
                    return View();
                }
            }
            ViewBag.Message = "Tạo dự án tình nguyện thành công";
            return RedirectToAction("Newfeed", "Home");
        }
    }
}