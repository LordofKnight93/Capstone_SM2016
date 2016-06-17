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
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Get group list
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public ActionResult ListGroup(int skip = 0, int number = 5)
        {
            //check input, use default value if not pass
            if (skip < 0||number <=0)
            {
                skip = 0;
                number = 5;
            }

            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
            var result = groupDAO.Get_All_GroupInformation(skip, number);
            return View(result);
        }

        public ActionResult ListProject(int skip = 0, int number = 5)
        {
            //check input, use default value if not pass
            if (skip < 0 || number <= 0)
            {
                skip = 0;
                number = 5;
            }

            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
            var result = projectDAO.Get_All_ProjectInformation(skip, number);
            return View(result);
        }

        public ActionResult ListUser(int skip = 0, int number = 5)
        {
            //check input, use default value if not pass
            if (skip < 0 || number <= 0)
            {
                skip = 0;
                number = 5;
            }

            Mongo_User_DAO projectDAO = new Mongo_User_DAO();
            var result = projectDAO.Get_All_UserInformations(skip, number);
            return View(result);
        }
    }
}