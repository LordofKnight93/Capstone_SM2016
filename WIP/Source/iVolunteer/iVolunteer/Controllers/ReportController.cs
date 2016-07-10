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
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>

        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public ActionResult ReportTarget(string targetID, int targetType)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }
                // create report creator
                SDLink creator = new SDLink();
                creator.ID = Session["UserID"].ToString();
                creator.DisplayName = Session["DisplayName"].ToString();
                creator.Handler = Handler.USER;

                // create report Target
                SDLink destination = new SDLink();
                
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // Case 1: Report Target is Group
                        if (targetType == 1)
                        {
                            // Target is Group
                            Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                            destination = groupDAO.Get_SDLink(targetID);

                            // Create SQL Report relation
                            SQL_AcGr_Relation_DAO sqlReportDAO = new SQL_AcGr_Relation_DAO();

                            SQL_AcGr_Relation report = new SQL_AcGr_Relation();
                            report.UserID = creator.ID;
                            report.GroupID = targetID;
                            report.Status = Status.PENDING;
                            report.Relation = Relation.REPORT_RELATION;

                            sqlReportDAO.Add_Report(report);
                        }
                        // Case 2: Report Target is Project
                        else if (targetType == 2)
                        {
                            // Target is Project
                            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                            destination = projectDAO.Get_SDLink(targetID);

                            // Create SQL Report relation
                            SQL_AcPr_Relation_DAO sqlReportDAO = new SQL_AcPr_Relation_DAO();

                            SQL_AcPr_Relation report = new SQL_AcPr_Relation();
                            report.UserID = creator.ID;
                            report.ProjectID = targetID;
                            report.Status = Status.PENDING;
                            report.Relation = Relation.REPORT_RELATION;

                            sqlReportDAO.Add_Report(report);
                        }
                        // Case 3: Report Target is Other User
                        else if (targetType == 3)
                        {
                            // Target is Group
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            destination = userDAO.Get_SDLink(targetID);

                            // Create SQL Report relation
                            SQL_AcAc_Relation_DAO sqlReportDAO = new SQL_AcAc_Relation_DAO();

                            SQL_AcAc_Relation report = new SQL_AcAc_Relation();
                            report.UserID = creator.ID;
                            report.TargetUserID = targetID;
                            report.Status = Status.PENDING;
                            report.Relation = Relation.REPORT_RELATION;

                            sqlReportDAO.Add_Report(report);
                        }
                        //create Mongo Report
                        Mongo_Report_DAO mongoReportDAO = new Mongo_Report_DAO();
                        Mongo_Report mgReport = new Mongo_Report(creator, destination, 0);
                        mongoReportDAO.Add_Report(mgReport);

                        transaction.Complete();
                    }
                    catch
                    {
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                ViewBag.Message = "Gửi yêu cầu thành công. Cảm ơn bạn đã góp sức xây dựng cộng đồng iVolunteer lành mạnh";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        public ActionResult CancelReport(string targetID, int targetType)
        {
            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

            string userID = Session["UserID"].ToString();

            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Delete Report in SQL
                    // Case 1: Report Target is Group
                    if (targetType == 1)
                    {
                        SQL_AcGr_Relation_DAO reportDAO = new SQL_AcGr_Relation_DAO();
                        reportDAO.DeleteSentReport(userID, targetID);
                    }
                    // Case 2: Report Target is Project
                    else if (targetType == 2)
                    {
                        SQL_AcPr_Relation_DAO reportDAO = new SQL_AcPr_Relation_DAO();
                        reportDAO.DeleteSentReport(userID, targetID);
                    }
                    // Case 3: Report Target is Other User
                    else if (targetType == 3)
                    {
                        SQL_AcAc_Relation_DAO reportDAO = new SQL_AcAc_Relation_DAO();
                        reportDAO.DeleteSentReport(userID, targetID);
                    }
                    //Delete Report in Mongo
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Report(userID, targetID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }
            }
            ViewBag.Message = "Bạn đã hủy báo cáo vi phạm thành công";
            return PartialView("ErrorMessage");
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

            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    SQL_Group_DAO sqlGroupDAO = new SQL_Group_DAO();
                    sqlGroupDAO.Set_Activation_Status(groupID, Status.IS_BANNED);

                    // Delete pending request in SQL
                    SQL_AcGr_Relation_DAO sqlRelation = new SQL_AcGr_Relation_DAO();
                    sqlRelation.DeleteReportRelation(groupID);

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
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayPendingReport", "Report");
        }
        /// <summary>
        /// Ignore Reports to Group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult IgnoreGroupReport(string groupID)
        {
            using(var transaction = new TransactionScope())
            {
                try
                {
                    //Delete pending request in SQL
                    SQL_AcGr_Relation_DAO sqlRelation = new SQL_AcGr_Relation_DAO();
                    sqlRelation.DeleteReportRelation(groupID);

                    //Delete report in Mongo
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(groupID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayPendingReport", "Report");
        }
        /// <summary>
        /// Deactivate Project
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeactivateProject(string projectID)
        {

            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    SQL_Project_DAO sqlProjectDAO = new SQL_Project_DAO();
                    sqlProjectDAO.Set_Activation_Status(projectID, Status.IS_BANNED);

                    // Delete pending request in SQL
                    SQL_AcPr_Relation_DAO sqlRelation = new SQL_AcPr_Relation_DAO();
                    sqlRelation.DeleteReportRelation(projectID);

                    //Set banned status in Mongo
                    Mongo_Project_DAO mgProjectDAO = new Mongo_Project_DAO();
                    mgProjectDAO.Set_Activation_Status(projectID, Status.IS_BANNED);

                    // Delete report in Mongo
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Reports(projectID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayPendingReport", "Report");
        }
        /// <summary>
        /// Ignore Reports to Project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult IgnoreProjectReport(string projectID)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //Delete pending request in SQL
                    SQL_AcPr_Relation_DAO sqlRelation = new SQL_AcPr_Relation_DAO();
                    sqlRelation.DeleteReportRelation(projectID);

                    //Delete report in Mongo
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(projectID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayPendingReport", "Report");
        }
        /// <summary>
        /// Deactivate User
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeactivateUser(string userID)
        {

            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    SQL_Account_DAO sqlUserDAO = new SQL_Account_DAO();
                    sqlUserDAO.Set_Activation_Status(userID, Status.IS_BANNED);

                    // Delete pending request in SQL
                    SQL_AcAc_Relation_DAO sqlRelation = new SQL_AcAc_Relation_DAO();
                    sqlRelation.DeleteReportRelation(userID);

                    //Set banned status in Mongo
                    Mongo_User_DAO mgUserDAO = new Mongo_User_DAO();
                    mgUserDAO.Set_Activation_Status(userID, Status.IS_BANNED);

                    // Delete report in Mongo
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Reports(userID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayPendingReport", "Report");
        }
        /// <summary>
        /// Ignore Reports to User
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult IgnoreUserReport(string userID)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //Delete pending request in SQL
                    SQL_AcAc_Relation_DAO sqlRelation = new SQL_AcAc_Relation_DAO();
                    sqlRelation.DeleteReportRelation(userID);

                    //Delete report in Mongo
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(userID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayPendingReport", "Report");
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
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    SQL_Group_DAO sqlGroupDAO = new SQL_Group_DAO();
                    sqlGroupDAO.Set_Activation_Status(groupID, Status.IS_ACTIVATE);

                    //Set activate status in Mongo
                    Mongo_Group_DAO mgGroupDAO = new Mongo_Group_DAO();
                    mgGroupDAO.Set_Activation_Status(groupID, Status.IS_ACTIVATE);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayBannedObjects", "Report");
        }
        /// <summary>
        /// Reactivate banned Project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ReactivateProject(string projectID)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    SQL_Project_DAO sqlProjectDAO = new SQL_Project_DAO();
                    sqlProjectDAO.Set_Activation_Status(projectID, Status.IS_ACTIVATE);

                    //Set activate status in Mongo
                    Mongo_Project_DAO mgProjectDAO = new Mongo_Project_DAO();
                    mgProjectDAO.Set_Activation_Status(projectID, Status.IS_ACTIVATE);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayBannedObjects", "Report");
        }
        /// <summary>
        /// Reactivate banned User
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult ReactivateUser(string userID)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    SQL_Account_DAO sqlAccountDAO = new SQL_Account_DAO();
                    sqlAccountDAO.Set_Activation_Status(userID, Status.IS_ACTIVATE);

                    //Set activate status in Mongo
                    Mongo_User_DAO mgUserDAO = new Mongo_User_DAO();
                    mgUserDAO.Set_Activation_Status(userID, Status.IS_ACTIVATE);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return View("ErrorMessage");
                }
            }
            return RedirectToAction("DisplayBannedObjects", "Report");
        }

    }
}