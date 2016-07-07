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
        public ActionResult ReportGroup(string groupID)
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

                SQL_AcGr_Report_DAO reportDAO = new SQL_AcGr_Report_DAO();
                
                //If report has been sent 
                if (reportDAO.IsSentReport(creator.ID, groupID))
                {
                    ViewBag.Message = "Bạn đã báo cáo vi phạm nhóm tình nguyện này rồi.";
                    return PartialView("ErrorMessage");
                }
                
                //create SQL Report 
                try
                {
                    SQL_AcGr_Report report = new SQL_AcGr_Report();
                    report.UserID = creator.ID;
                    report.GroupID = groupID;
                    report.ReportType = 1;
                    report.Status = Status.PENDING_REPORT;

                    reportDAO.Add_Report(report);
                }
                catch
                {
                    ViewBag.Message = Error.UNEXPECT_ERROR;
                    return PartialView("ErrorMessage");
                }
                ViewBag.Message = "Gửi yêu cầu thành công. <br /> Cảm ơn bạn đã góp sức xây dựng cộng đồng iVolunteer lành mạnh";
                return PartialView("ErrorMessage");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
            
    }
}