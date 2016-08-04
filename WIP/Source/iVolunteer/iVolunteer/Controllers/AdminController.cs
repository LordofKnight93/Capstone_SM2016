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
using System.IO;
namespace iVolunteer.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Manage()
        {
            return View("AdminHome");
        }
        public ActionResult DisplayPendingReport()
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            return View("PendingReportList");
        }
        public ActionResult DisplayPendingReportedGroup()
        {
            try
            {
                Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                var result = reportDAO.Get_GroupReport();
                var reportedGroups = reportDAO.Get_ReportedGroup().ToList();
                var tuple = new Tuple<List<Mongo_Report>, List<SDLink>>(result, reportedGroups);

                return PartialView("_PendingReportedGroups", tuple);
            }
            catch
            {
                throw;
            }
        }
        public ActionResult DisplayPendingReportedProject()
        {
            try
            {
                Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                var result = reportDAO.Get_ProjectReport();
                var reportedProjects = reportDAO.Get_ReportedProject().ToList();
                var tuple = new Tuple<List<Mongo_Report>, List<SDLink>>(result, reportedProjects);

                return PartialView("_PendingReportedProjects", tuple);
            }
            catch
            {
                throw;
            }
        }
        public ActionResult DisplayPendingReportedUser()
        {
            try
            {
                Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                var result = reportDAO.Get_UserReport();
                var reportedUsers = reportDAO.Get_ReportedUser().ToList();
                var tuple = new Tuple<List<Mongo_Report>, List<SDLink>>(result, reportedUsers);

                return PartialView("_PendingReportedUsers", tuple);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Deactivate Group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeactivateGroup(string groupID)
        {
            if (groupID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    SQL_Group_DAO sqlGroupDAO = new SQL_Group_DAO();
                    sqlGroupDAO.Deactive(groupID);

                    // Delete pending request in SQL
                    SQL_AcGr_Relation_DAO sqlRelation = new SQL_AcGr_Relation_DAO();
                    sqlRelation.Delete_Reports(groupID);

                    //Set banned status in Mongo
                    Mongo_Group_DAO mgGroupDAO = new Mongo_Group_DAO();
                    mgGroupDAO.Set_Activation_Status(groupID, Status.IS_BANNED);

                    // Delete report in Mongo
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Reports(groupID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayPendingReport", "Admin");
            return Json(true);
        }
        /// <summary>
        /// Ignore Reports to Group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult IgnoreGroupReport(string groupID)
        {
            if (groupID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //Delete pending request in SQL
                    SQL_AcGr_Relation_DAO sqlRelation = new SQL_AcGr_Relation_DAO();
                    sqlRelation.Delete_Reports(groupID);

                    //Delete report in Mongo
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(groupID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayPendingReport", "Admin");
        }
        /// <summary>
        /// Deactivate Project
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeactivateProject(string projectID)
        {
            if (projectID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    SQL_Project_DAO sqlProjectDAO = new SQL_Project_DAO();
                    sqlProjectDAO.Deactive(projectID);

                    // Delete pending request in SQL
                    SQL_AcPr_Relation_DAO sqlRelation = new SQL_AcPr_Relation_DAO();
                    sqlRelation.Delete_Reports(projectID);

                    //Set banned status in Mongo
                    Mongo_Project_DAO mgProjectDAO = new Mongo_Project_DAO();
                    mgProjectDAO.Set_Activation_Status(projectID, Status.IS_BANNED);

                    // Delete report in Mongo
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Reports(projectID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");'
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayPendingReport", "Admin");
        }
        /// <summary>
        /// Ignore Reports to Project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult IgnoreProjectReport(string projectID)
        {
            if (projectID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //Delete pending request in SQL
                    SQL_AcPr_Relation_DAO sqlRelation = new SQL_AcPr_Relation_DAO();
                    sqlRelation.Delete_Reports(projectID);

                    //Delete report in Mongo
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(projectID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayPendingReport", "Admin");
        }
        /// <summary>
        /// Deactivate User
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeactivateUser(string userID)
        {
            if (userID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    SQL_Account_DAO sqlUserDAO = new SQL_Account_DAO();
                    sqlUserDAO.Deactive(userID);

                    // Delete pending request in SQL
                    SQL_AcAc_Relation_DAO sqlRelation = new SQL_AcAc_Relation_DAO();
                    sqlRelation.Delete_Reports(userID);

                    //Set banned status in Mongo
                    Mongo_User_DAO mgUserDAO = new Mongo_User_DAO();
                    mgUserDAO.Set_Activation_Status(userID, Status.IS_BANNED);

                    // Delete report in Mongo
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Reports(userID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayPendingReport", "Admin");
        }
        /// <summary>
        /// Ignore Reports to User
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult IgnoreUserReport(string userID)
        {
            if (userID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //Delete pending request in SQL
                    SQL_AcAc_Relation_DAO sqlRelation = new SQL_AcAc_Relation_DAO();
                    sqlRelation.Delete_Reports(userID);

                    //Delete report in Mongo
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(userID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayPendingReport", "Admin");
        }
        public ActionResult DisplayBannedObjects()
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
            try
            {
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                var groups = groupDAO.Get_Banned_Groups();

                Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                var projects = projectDAO.Get_Banned_Projects();

                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var users = userDAO.Get_Banned_Users();

                var tuple = new Tuple<List<SDLink>, List<SDLink>, List<SDLink>>(groups, projects, users);
                return View("BannedObjects", tuple);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Reactivate Banned Group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult ReactivateGroup(string groupID)
        {
            if (groupID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    SQL_Group_DAO sqlGroupDAO = new SQL_Group_DAO();
                    sqlGroupDAO.Activate(groupID);

                    //Set activate status in Mongo
                    Mongo_Group_DAO mgGroupDAO = new Mongo_Group_DAO();
                    mgGroupDAO.Set_Activation_Status(groupID, Status.IS_ACTIVATE);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayBannedObjects", "Admin");
        }
        /// <summary>
        /// Reactivate banned Project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ReactivateProject(string projectID)
        {
            if (projectID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    SQL_Project_DAO sqlProjectDAO = new SQL_Project_DAO();
                    sqlProjectDAO.Activate(projectID);

                    //Set activate status in Mongo
                    Mongo_Project_DAO mgProjectDAO = new Mongo_Project_DAO();
                    mgProjectDAO.Set_Activation_Status(projectID, Status.IS_ACTIVATE);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayBannedObjects", "Admin");
        }
        /// <summary>
        /// Reactivate banned User
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult ReactivateUser(string userID)
        {
            if (userID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    SQL_Account_DAO sqlAccountDAO = new SQL_Account_DAO();
                    sqlAccountDAO.Activate(userID);

                    //Set activate status in Mongo
                    Mongo_User_DAO mgUserDAO = new Mongo_User_DAO();
                    mgUserDAO.Set_Activation_Status(userID, Status.IS_ACTIVATE);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    //ViewBag.Message = Error.UNEXPECT_ERROR;
                    //return View("ErrorMessage");
                    return Json(false);
                }
            }
            //return RedirectToAction("DisplayBannedObjects", "Admin");
        }

    }
}