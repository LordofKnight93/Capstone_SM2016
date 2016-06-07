using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.Mvc;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Models.Data_Access_Object.MongoDB;
using iVolunteer.Models.Data_Access_Object.SQL;
using MongoDB.Bson;

namespace iVolunteer.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Index()
        {
            return View();
        }
        //GET : Add project
        [HttpGet]
        public ActionResult AddProject()
        {
            return View();
        }
        //POST : Addproject
        [HttpPost]
        public ActionResult AddProject( ProjectInformation projectInfo )
        {
            ///
            /// This would be check permission and validate data, will add later
            ///

            //set information for project
            projectInfo.DateCreate = DateTime.Now;
            projectInfo.IsActivate = true;
            projectInfo.IsRecruit = true;
            //create mongo project
            Mongo_Project mongo_Project = new Mongo_Project(projectInfo);
            /*
            mongo_Project.ProjectStructure.CreatorID = Session["UserID"].ToString();
            mongo_Project.ProjectStructure.Leaders = new Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass.UserSD[1];
            mongo_Project.ProjectStructure.Leaders[0]._id = new ObjectId(Session["UserID"].ToString());
            */
            // generate an ID for project
            mongo_Project._id = ObjectId.GenerateNewId();
            //create sql project
            SQL_Project sql_Project = new SQL_Project();
            sql_Project.ProjectID = mongo_Project._id.ToString();
            sql_Project.IsActivate = true;
            //start transaction
            using (var transacton = new TransactionScope())
            {
                try
                {
                    Mongo_Project_DAO.Add_Project(mongo_Project);
                    SQL_Project_DAO.AddProject(sql_Project);
                    transacton.Complete();
                }
                catch
                {
                    transacton.Dispose();
                    ViewBag.Message = "Có lỗi xảy ra, vui lòng thử lại sau ít phút";
                }
            }
            return View();
        }
    }
}