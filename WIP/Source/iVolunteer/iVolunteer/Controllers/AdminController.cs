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
        /// <summary>
        /// 管理者画面を表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Manage()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            return View("AdminHome");
        }
        /// <summary>
        /// 未決報告画面を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayPendingReport()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            return View("PendingReportList");
        }
        /// <summary>
        /// 未決グループ報告を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayPendingReportedGroup()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

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
        /// <summary>
        /// 未決プロジェクト報告を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayPendingReportedProject()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

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
        /// <summary>
        /// 未決ユーザー報告を表示
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayPendingReportedUser()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

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
        /// グループを停止
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeactivateGroup(string groupID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (groupID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    // SQLで停止状態を設定
                    SQL_Group_DAO sqlGroupDAO = new SQL_Group_DAO();
                    sqlGroupDAO.Deactive(groupID);

                    // Delete pending request in SQL
                    // SQLで未決報告を削除
                    SQL_AcGr_Relation_DAO sqlRelation = new SQL_AcGr_Relation_DAO();
                    sqlRelation.Delete_Reports(groupID);

                    //Set banned status in Mongo
                    //Mongoで停止状態を設定
                    Mongo_Group_DAO mgGroupDAO = new Mongo_Group_DAO();
                    mgGroupDAO.Set_Activation_Status(groupID, Status.IS_BANNED);

                    // Delete report in Mongo
                    //Mongoで報告を削除
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Reports(groupID);

                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
            return Json(true);
        }
        /// <summary>
        /// Ignore Group's Report
        /// グループの報告を無視
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult IgnoreGroupReport(string groupID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (groupID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Delete pending request in SQL
                    // SQLでの未決報告を削除
                    SQL_AcGr_Relation_DAO sqlRelation = new SQL_AcGr_Relation_DAO();
                    sqlRelation.Delete_Reports(groupID);

                    // Delete report in Mongo
                    // Mongoで報告を削除
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(groupID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
        }
        /// <summary>
        /// Deactivate Project
        /// プロジェクトを停止
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeactivateProject(string projectID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (projectID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    // SQLで停止状態を設定
                    SQL_Project_DAO sqlProjectDAO = new SQL_Project_DAO();
                    sqlProjectDAO.Deactive(projectID);

                    // Delete pending request in SQL
                    // SQLで未決報告を削除
                    SQL_AcPr_Relation_DAO sqlRelation = new SQL_AcPr_Relation_DAO();
                    sqlRelation.Delete_Reports(projectID);

                    //Set banned status in Mongo
                    //Mongoで停止状態を設定
                    Mongo_Project_DAO mgProjectDAO = new Mongo_Project_DAO();
                    mgProjectDAO.Set_Activation_Status(projectID, Status.IS_BANNED);

                    // Delete report in Mongo
                    // Mongoで報告を削除
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Reports(projectID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
        }
        /// <summary>
        /// Ignore Reports to Project
        /// プロジェクトの報告を無視
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult IgnoreProjectReport(string projectID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (projectID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //Delete pending request in SQL
                    //SQLで未決報告要求を削除
                    SQL_AcPr_Relation_DAO sqlRelation = new SQL_AcPr_Relation_DAO();
                    sqlRelation.Delete_Reports(projectID);

                    //Delete report in Mongo
                    //Mongoで報告を削除
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(projectID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
        }
        /// <summary>
        /// Deactivate User
        /// ユーザーを停止
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult DeactivateUser(string userID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (userID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set Banned status in SQL
                    // SQLで停止状態を設定
                    SQL_Account_DAO sqlUserDAO = new SQL_Account_DAO();
                    sqlUserDAO.Deactive(userID);

                    // Delete pending request in SQL
                    // SQLで未決要求を削除
                    SQL_AcAc_Relation_DAO sqlRelation = new SQL_AcAc_Relation_DAO();
                    sqlRelation.Delete_Reports(userID);

                    //Set banned status in Mongo
                    //Mongoで停止状態を設定
                    Mongo_User_DAO mgUserDAO = new Mongo_User_DAO();
                    mgUserDAO.Set_Activation_Status(userID, Status.IS_BANNED);

                    // Delete report in Mongo
                    //Monoで報告を削除
                    Mongo_Report_DAO mgReportDAO = new Mongo_Report_DAO();
                    mgReportDAO.Delete_Reports(userID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
        }
        /// <summary>
        /// Ignore Reports to User
        /// ユーザー報告を無視
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult IgnoreUserReport(string userID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (userID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    //Delete pending request in SQL
                    //SQLで未決要求を削除
                    SQL_AcAc_Relation_DAO sqlRelation = new SQL_AcAc_Relation_DAO();
                    sqlRelation.Delete_Reports(userID);

                    //Delete report in Mongo
                    //Mongoでの報告を削除
                    Mongo_Report_DAO reportDAO = new Mongo_Report_DAO();
                    reportDAO.Delete_Reports(userID);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
        }
        /// <summary>
        /// 停止したものを表示
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayBannedObjects()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (Session["Role"].ToString() != "Admin" || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
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
        /// グループを再活性化
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActionResult ReactivateGroup(string groupID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (groupID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    // SQLでの許可状態を設定
                    SQL_Group_DAO sqlGroupDAO = new SQL_Group_DAO();
                    sqlGroupDAO.Activate(groupID);

                    //Set activate status in Mongo
                    //Mongoでの許可状態を設定
                    Mongo_Group_DAO mgGroupDAO = new Mongo_Group_DAO();
                    mgGroupDAO.Set_Activation_Status(groupID, Status.IS_ACTIVATE);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
        }
        /// <summary>
        /// Reactivate banned Project
        /// 停止したプロジェクトを再活性化
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult ReactivateProject(string projectID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (projectID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    //SQLでの活性状態を設定
                    SQL_Project_DAO sqlProjectDAO = new SQL_Project_DAO();
                    sqlProjectDAO.Activate(projectID);

                    //Set activate status in Mongo
                    //Mongoでの活性状態を設定
                    Mongo_Project_DAO mgProjectDAO = new Mongo_Project_DAO();
                    mgProjectDAO.Set_Activation_Status(projectID, Status.IS_ACTIVATE);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
        }
        /// <summary>
        /// Reactivate banned User
        /// 停止したユーザーを再活性化
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult ReactivateUser(string userID)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            if (userID == null) return Json(false);
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Set activate status in SQL
                    //SQLでの活性状態を設定
                    SQL_Account_DAO sqlAccountDAO = new SQL_Account_DAO();
                    sqlAccountDAO.Activate(userID);

                    //Set activate status in Mongo
                    //Mongoで活性状態を設定
                    Mongo_User_DAO mgUserDAO = new Mongo_User_DAO();
                    mgUserDAO.Set_Activation_Status(userID, Status.IS_ACTIVATE);

                    transaction.Complete();
                    return Json(true);
                }
                catch
                {
                    transaction.Dispose();
                    return Json(false);
                }
            }
        }

        public ActionResult DisplayAsviseAndFailureReport()
        {
            if (Session["UserID"] == null || !Session["Role"].ToString().Equals("Admin"))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            return View("AdviseAndFailureReport");
        }

        public ActionResult AllAdvise()
        {
            if(Session["UserID"] == null || !Session["Role"].ToString().Equals("Admin"))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            try
            {
                Mongo_FailureReport_DAO mongoDAO = new Mongo_FailureReport_DAO();
                List<Mongo_FailureReport> result = mongoDAO.Get_Advise();
                return PartialView("_AllAdvide", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult AllFailureReport()
        {
            if (Session["UserID"] == null || !Session["Role"].ToString().Equals("Admin"))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            try
            {
                Mongo_FailureReport_DAO mongoDAO = new Mongo_FailureReport_DAO();
                List<Mongo_FailureReport> result = mongoDAO.Get_FailureReport();
                return PartialView("_AllFailureReport", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult DeleteAdviseOrFailureReport(string failureID, string target)
        {
            if (Session["UserID"] == null || !Session["Role"].ToString().Equals("Admin"))
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            try
            {
                if (Session["Role"].ToString().Equals("Admin"))
                {
                    Mongo_FailureReport_DAO mongoDAO = new Mongo_FailureReport_DAO();
                    mongoDAO.Delete_FailureReport(failureID);
                    if (target.Equals("Advise"))
                    {
                        return RedirectToAction("AllAdvise","Admin");
                    }
                    else
                    {
                        return RedirectToAction("AllFailureReport", "Admin");
                    }
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
    }
}