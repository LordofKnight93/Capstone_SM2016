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
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Report USER|GROUP|PROJECT
        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public ActionResult ReportTarget(ReportModel report, string targetID, int targetType)
        {
            if (!ModelState.IsValid) return Json(new { status = false, message = "Mời bạn nêu lý do trước khi gửi báo cáo!" });
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
                // initiallize report destination
                SDLink destination = new SDLink();
                //report reason 
                //int reason = 0;
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

                            sqlReportDAO.Add_Report(creator.ID, targetID);
                        }
                        // Case 2: Report Target is Project
                        else if (targetType == 2)
                        {
                            // Target is Project
                            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                            destination = projectDAO.Get_SDLink(targetID);

                            // Create SQL Report relation
                            SQL_AcPr_Relation_DAO sqlReportDAO = new SQL_AcPr_Relation_DAO();

                            sqlReportDAO.Add_Report(creator.ID, targetID);
                        }
                        // Case 3: Report Target is Other User
                        else if (targetType == 3)
                        {
                            // Target is Group
                            Mongo_User_DAO userDAO = new Mongo_User_DAO();
                            destination = userDAO.Get_SDLink(targetID);

                            // Create SQL Report relation
                            SQL_AcAc_Relation_DAO sqlReportDAO = new SQL_AcAc_Relation_DAO();

                            sqlReportDAO.Add_Report(creator.ID, targetID);
                        }
                        //create Mongo Report
                        Mongo_Report_DAO mongoReportDAO = new Mongo_Report_DAO();
                        if (report.Detail == null)
                        {
                            Mongo_Report mgReport = new Mongo_Report(creator, destination, report.Reason);
                            mongoReportDAO.Add_Report(mgReport);
                        }
                        else
                        {
                            Mongo_Report mgReport = new Mongo_Report(creator, destination, report.Reason, report.Detail);
                            mongoReportDAO.Add_Report(mgReport);
                        }
                        transaction.Complete();
                    }
                    catch
                    {
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                ViewBag.Message = "Cảm ơn bạn đã góp sức xây dựng cộng đồng iVolunteer lành mạnh";
                //return PartialView("ErrorMessage");
                return Json(new { status = true, message = "Cảm ơn bạn đã góp sức xây dựng cộng đồng iVolunteer lành mạnh" });
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
                        reportDAO.Delete_Report(userID, targetID);
                    }
                    // Case 2: Report Target is Project
                    else if (targetType == 2)
                    {
                        SQL_AcPr_Relation_DAO reportDAO = new SQL_AcPr_Relation_DAO();
                        reportDAO.Delete_Report(userID, targetID);
                    }
                    // Case 3: Report Target is Other User
                    else if (targetType == 3)
                    {
                        SQL_AcAc_Relation_DAO reportDAO = new SQL_AcAc_Relation_DAO();
                        reportDAO.Delete_Report(userID, targetID);
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
            //ViewBag.Message = "Bạn đã hủy báo cáo vi phạm thành công";
            //return PartialView("ErrorMessage");

            //return RedirectToAction("ActionToGroup", "Account", new { groupID = targetID });
            var ctrl = new AccountController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.ActionToGroup(targetID);
        }
        public ActionResult DisplayReport(string targetID, int targetType)
        {
            ViewBag.TargetID = targetID;
            if (targetType == 1) return PartialView("_GroupReport");
            if (targetType == 2) return PartialView("_ProjectReport");
            if (targetType == 3) return PartialView("_UserReport");
            return PartialView("ErrorMessage");
        }
    }

}