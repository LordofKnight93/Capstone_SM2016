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
                Mongo_Group_DAO groupDAO = new Mongo_Group_DAO();
                SDLink destination = groupDAO.Get_SDLink(targetID);


                Mongo_Report_DAO mongoReportDAO = new Mongo_Report_DAO();

                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        //create SQL Report relation
                        // Case 1: Report Target is Group
                        if (targetType == 1)
                        {
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
                            SQL_AcAc_Relation_DAO sqlReportDAO = new SQL_AcAc_Relation_DAO();

                            SQL_AcAc_Relation report = new SQL_AcAc_Relation();
                            report.UserID = creator.ID;
                            report.TargetUserID = targetID;
                            report.Status = Status.PENDING;
                            report.Relation = Relation.REPORT_RELATION;

                            sqlReportDAO.Add_Report(report);
                        }
                        //create Mongo Report
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

    }
}