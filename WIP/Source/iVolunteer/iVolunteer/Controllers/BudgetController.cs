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
    public class BudgetController : Controller
    {
        /// <summary>
        /// 予算タブルを表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Get View
        public ActionResult BudgetAllView(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.ProjectID = projectID;

                //Get Received Money in Fund
                Mongo_Fund_DAO fundDAO = new Mongo_Fund_DAO();
                ViewBag.ReceivedMoney = fundDAO.Get_ReceivedMoney(projectID);

                //Get Total Cost Estimated
                Mongo_Budget_DAO budgetDAO = new Mongo_Budget_DAO();
                ViewBag.TotalEstimated = budgetDAO.Get_TotalMoneyCost(projectID);

                return PartialView("_Budget");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 詳細予算を取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Get Detail Budget Record
        public ActionResult DetailBudgetRecord(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                ViewBag.ProjectID = projectID;

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                List<BudgetRecordInformation> result = new List<BudgetRecordInformation>();
                result = mongoDAO.Get_BudgetAllRecord(projectID);

                return PartialView("_BudgetRecord", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 予算を追加画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddBudgetRecordForm(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                ViewBag.ProjectID = projectID;

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                if (ViewBag.IsLeader == true)
                {
                    return PartialView("_BudgetAddRecord");
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
        /// <summary>
        /// 予算を追加
        /// </summary>
        /// <param name="budgetInfo"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Add Budget Record
        [HttpPost]
        public ActionResult AddBudgetRecord(BudgetRecordInformation budgetInfo, string projectID)
        {
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.ProjectID = projectID;
                        return PartialView("_BudgetAddRecord", budgetInfo);
                    }
                    

                    //Variable for validate
                    string errName = "";
                    bool isValid = true;

                    // create DAO instance
                    Mongo_Budget_DAO mongo_DAO = new Mongo_Budget_DAO();

                    //create project id
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    SDLink project = projectDAO.Get_SDLink(projectID);

                    //add mongo Budget
                    Mongo_Budget mongo_Budget = new Mongo_Budget(budgetInfo);
                    mongo_Budget.BudgetRecordInformation.BudgetRecordID = mongo_Budget._id.ToString();
                    mongo_Budget.BudgetRecordInformation.Project = project;

                    List<BudgetRecordInformation> currentBudgetRecordList = mongo_DAO.Get_BudgetAllRecord(projectID);
                    for (int i = 0; i < currentBudgetRecordList.Count(); i++)
                    {
                        if (currentBudgetRecordList[i].Name.Equals(mongo_Budget.BudgetRecordInformation.Name))
                        {
                            errName = Error.BUDGETRECORD_EXIST + Environment.NewLine;
                            isValid = false;
                        }
                    }

                    if (!isValid)
                    {
                        ViewBag.MessageName = errName;
                        ViewBag.ProjectID = projectID;

                        return PartialView("_BudgetAddRecord", budgetInfo);
                    }

                    //start transaction

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //write data to db
                            mongo_DAO.Add_Budget(mongo_Budget);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = projectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                    
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <returns></returns>
        //Delete Budget Record
        public ActionResult DeleteBudgetRecord(string budgetRecordID)
        {
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string projectID = "";

                //get project id
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {

                    //start transaction 
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            mongoDAO.Delete_Budget(budgetRecordID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = projectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }

        }
        /// <summary>
        /// 予算を変更画面を表示
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <returns></returns>
        //Edit Budget Record
        [HttpGet]
        public ActionResult EditBudgetRecord(string budgetRecordID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(budgetRecordID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                ViewBag.BudgetRecordID = budgetRecordID;

                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                if (ViewBag.IsLeader == true)
                {
                    return PartialView("_BudgetEditRecord");
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
            }
            catch
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 予算を変更
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="newinfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditBudgetRecord(string budgetRecordID, BudgetRecordInformation newinfo)
        {
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                string projectID = "";

                //get project id
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    if (!ModelState.IsValid) return PartialView("_BudgetEditRecord", newinfo);

                    //start transaction
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {

                            //write data to db
                            mongoDAO.Update_BudgetRecord(budgetRecordID, newinfo);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = projectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 予算タイトルを変更
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="txtRecordName"></param>
        /// <returns></returns>
        //Update Record Name
        public ActionResult EditBudgetRecordName(string budgetRecordID, string txtRecordName)
        {
            if (!ModelState.IsValid) return View();

            //start transaction
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                //get project id
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //write data to db
                            mongoDAO.Update_BudgetRecordName(budgetRecordID, txtRecordName);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = projectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 詳細予算アイテムを取得
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <returns></returns>
        //Get Budget item
        public ActionResult DetailBudgetItem(string budgetRecordID)
        {
            try
            {
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.BudgetRecordID = budgetRecordID;

                List<BudgetItem> result = new List<BudgetItem>();
                result = mongoDAO.Get_BudgetItemList(budgetRecordID);

                return PartialView("_BudgetItem", result);
            }
            catch
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// 予算アイテムを追加画面を表示
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <returns></returns>
        //Add Budget Item
        [HttpGet]
        public ActionResult AddBudgetItem(string budgetRecordID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(budgetRecordID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.BudgetRecordID = budgetRecordID;

                return PartialView("_BudgetAddItem");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 予算アイテムを追加
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddBudgetItem(string budgetRecordID, BudgetItem itemInfo)
        {
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                //get project id
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.BudgetRecordID = budgetRecordID;

                        return PartialView("_BudgetAddItem", itemInfo);
                    }

                    //variable for validate
                    string errContent = "";
                    bool isValid = true;

                    //set new info
                    BudgetItem item = new BudgetItem(itemInfo);

                    //start validate
                    List<BudgetItem> currentBudgetItemList = mongoDAO.Get_BudgetItemList(budgetRecordID);
                    for (int i = 0; i < currentBudgetItemList.Count(); i++)
                    {
                        if (currentBudgetItemList[i].Content.Equals(item.Content))
                        {
                            errContent = Error.BUDGETITEM_EXIST + Environment.NewLine;
                            isValid = false;
                        }
                    }

                    if (!isValid)
                    {
                        ViewBag.MessageContent = errContent;
                        ViewBag.BudgetRecordID = budgetRecordID;

                        return PartialView("_BudgetAddItem", itemInfo);
                    }

                    //start transaction
                    using (var transaction = new TransactionScope())
                    {
                        try
                        { 
                            //write data to db
                            mongoDAO.Add_BudgetItem(budgetRecordID, item);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailBudgetItem", "Budget", new { budgetRecordID = budgetRecordID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 予算アイテムを削除
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="budgetItemID"></param>
        /// <returns></returns>
        //Delete Budget Item
        public ActionResult DeleteBudgetItem(string budgetRecordID, string budgetItemID)
        {
            if (!ModelState.IsValid) return View();

            //start transaction 
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string projectID = "";

                //get project id
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            mongoDAO.Delete_BudgetItem(budgetRecordID, budgetItemID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("DetailBudgetItem", "Budget", new { budgetRecordID = budgetRecordID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 予算アイテムを変更画面を表示
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="budgetItemID"></param>
        /// <returns></returns>
        //Edit Budget Item
        [HttpGet]
        public ActionResult EditBudgetItem(string budgetRecordID, string budgetItemID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(budgetRecordID) || String.IsNullOrEmpty(budgetItemID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                //Get project ID
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                //Check is leader or not
                string userID = Session["UserID"].ToString();
                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    ViewBag.BudgetRecordID = budgetRecordID;
                    ViewBag.BudgetItemID = budgetItemID;

                    var result = mongoDAO.Get_BudgetItem(budgetRecordID, budgetItemID);
                    return PartialView("_BudgetEditItem", result);
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
        /// <summary>
        /// 予算アイテムを変更
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="budgetItemID"></param>
        /// <param name="newinfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditBudgetItem(string budgetRecordID, string budgetItemID, BudgetItem newinfo)
         {
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                //get project id
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    //string err = ViewData.ModelState["Property"].Errors.ToString();
                    //if (!ModelState.IsValid)
                    //{
                    //    ViewBag.BudgetRecordID = budgetRecordID;
                    //    ViewBag.BudgetItemID = budgetItemID;

                    //    return PartialView("_BudgetEditItem", newinfo);
                    //}

                    //variable for validate
                    string errContent = "";
                    bool isValid = true;

                    //set new info
                    BudgetItem item = new BudgetItem(newinfo);

                    //start validate
                    //List<BudgetItem> currentBudgetItemList = mongoDAO.Get_BudgetItemList(budgetRecordID);
                    //for (int i = 0; i < currentBudgetItemList.Count(); i++)
                    //{
                    //    if (currentBudgetItemList[i].Content.Equals(newinfo.Content))
                    //    {
                    //        errContent = Error.BUDGETITEM_EXIST + Environment.NewLine;
                    //        isValid = false;
                    //    }
                    //}
                    //Check Content null
                    if (newinfo.Content.Equals(""))
                    {
                        errContent = Error.BUDGETITEM_CONTENT_NULL + Environment.NewLine;
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        ViewBag.MessageContent = errContent;
                        ViewBag.BudgetRecordID = budgetRecordID;
                        ViewBag.BudgetItemID = budgetItemID;

                        return PartialView("_BudgetEditItem", newinfo);
                    }

                    //start transaction
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //write data to db
                            mongoDAO.Update_BudgetItem(budgetRecordID, budgetItemID, item);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailBudgetItem", "Budget", new { budgetRecordID = budgetRecordID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 予算アイテムのフィールドを変更画面を表示
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="field"></param>
        /// <param name="budgetItemID"></param>
        /// <returns></returns>
        //Edit Each Field of Budget Item
        [HttpGet]
        public ActionResult EditEachFieldBudgetItem(string budgetRecordID, string field, string budgetItemID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(budgetRecordID) || String.IsNullOrEmpty(budgetItemID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.BudgetRecordID = budgetRecordID;
                ViewBag.Field = field;
                ViewBag.BudgetItemID = budgetItemID;

                return PartialView("_EditEachFieldBudgetItem");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 予算アイテムのフィールドを変更
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="item"></param>
        /// <param name="budgetItemID"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditEachFieldBudgetItem(string budgetRecordID, BudgetItem item, string budgetItemID, string field)
        {
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                //get project id
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                string projectID = mongoDAO.Get_ProjectID(budgetRecordID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                if(relationDAO.Is_Leader(userID, projectID))
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.BudgetRecordID = budgetRecordID;
                        ViewBag.Field = field;
                        ViewBag.BudgetItemID = budgetItemID;

                        return PartialView("_EditEachFieldBudgetItem", item);
                    }

                    switch (field)
                    {
                        case "content":
                            {
                                return RedirectToAction("EditBudgetItemContent", "Budget", new { budgetRecordID = budgetRecordID, budgetItemID = budgetItemID, newContent = item.Content });
                            }
                        case "unitprice":
                            {
                                return RedirectToAction("EditBudgetItemUnitPrice", "Budget", new { budgetRecordID = budgetRecordID, budgetItemID = budgetItemID, unitPrice = item.UnitPrice });
                            }
                        case "quatity":
                            {
                                return RedirectToAction("EditBudgetItemQuatity", "Budget", new { budgetRecordID = budgetRecordID, budgetItemID = budgetItemID, quatity = item.Quatity });
                            }
                        case "unit":
                            {
                                return RedirectToAction("EditBudgetItemUnit", "Budget", new { budgetRecordID = budgetRecordID, budgetItemID = budgetItemID, unit = item.Unit });
                            }
                        default:
                            {
                                ViewBag.Message = Error.WRONG_PASSWORD;
                                return View("ErrorMessage");
                            }
                    }
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return PartialView("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 予算アイテムの内容を変更
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="budgetItemID"></param>
        /// <param name="newContent"></param>
        /// <returns></returns>
        //update content
        public ActionResult EditBudgetItemContent(string budgetRecordID, string budgetItemID, string newContent)
        {
            if (!ModelState.IsValid) return View();
            string thisprojectID = "";

            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Budget_DAO mongo_Budget_DAO = new Mongo_Budget_DAO();
                        SQL_Budget_DAO sql_Budget_DAO = new SQL_Budget_DAO();
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetRecordID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItemContent(budgetRecordID, budgetItemID, newContent);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetRecordID = budgetRecordID });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 予算アイテムの金額を変更
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="budgetItemID"></param>
        /// <param name="unitPrice"></param>
        /// <returns></returns>
        //update unitprice
        public ActionResult EditBudgetItemUnitPrice(string budgetRecordID, string budgetItemID, double unitPrice)
        {
            if (!ModelState.IsValid) return View();
            string thisprojectID = "";

            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Budget_DAO mongo_Budget_DAO = new Mongo_Budget_DAO();
                        SQL_Budget_DAO sql_Budget_DAO = new SQL_Budget_DAO();
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetRecordID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItemUnitPrice(budgetRecordID, budgetItemID, unitPrice);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetRecordID = budgetRecordID });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 予算アイテムの数量を変更
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="budgetItemID"></param>
        /// <param name="quatity"></param>
        /// <returns></returns>
        //update quatity
        public ActionResult EditBudgetItemQuatity(string budgetRecordID, string budgetItemID, int quatity)
        {
            if (!ModelState.IsValid) return View();
            string thisprojectID = "";

            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Budget_DAO mongo_Budget_DAO = new Mongo_Budget_DAO();
                        SQL_Budget_DAO sql_Budget_DAO = new SQL_Budget_DAO();
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetRecordID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItemQuatity(budgetRecordID, budgetItemID, quatity);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetRecordID = budgetRecordID });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 予算アイテムのユニットを変更
        /// </summary>
        /// <param name="budgetRecordID"></param>
        /// <param name="budgetItemID"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        //update unit
        public ActionResult EditBudgetItemUnit(string budgetRecordID, string budgetItemID, string unit)
        {
            if (!ModelState.IsValid) return View();
            string thisprojectID = "";

            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Budget_DAO mongo_Budget_DAO = new Mongo_Budget_DAO();
                        SQL_Budget_DAO sql_Budget_DAO = new SQL_Budget_DAO();
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetRecordID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItemUnit(budgetRecordID, budgetItemID, unit);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetRecordID = budgetRecordID });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }



         /// <summary>
         /// 詳細ファイナンスを表示
         /// </summary>
         /// <param name="projectID"></param>
         /// <returns></returns>
        //Finance View
        public ActionResult DetailFinance(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                //set new ViewBag
                ViewBag.ProjectID = projectID;
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                Mongo_Finance_DAO mongoDAO = new Mongo_Finance_DAO();
                Mongo_Finance result = mongoDAO.Get_Finance(projectID);

                Mongo_Fund_DAO fundDAO = new Mongo_Fund_DAO();
                ViewBag.ReceivedMoney = fundDAO.Get_ReceivedMoney(projectID);

                return PartialView("_Finance", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// ファイナンスアイテムを取得
        /// </summary>
        /// <param name="financeID"></param>
        /// <returns></returns>
        public ActionResult DetailFinanceItem(string financeID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(financeID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            
            try
            {
                ViewBag.FinanceID = financeID;

                //Get Project ID
                Mongo_Finance_DAO mongoDAO = new Mongo_Finance_DAO();
                string projectID = mongoDAO.Get_ProjectID(financeID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                List<FinanceItem> result = new List<FinanceItem>();
                result = mongoDAO.Get_FinanceItemList(financeID);

                return PartialView("_FinanceDetails", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// ファイナンスアイテムを追加画面を表示
        /// </summary>
        /// <param name="financeID"></param>
        /// <returns></returns>
        //Finance Add Item
        [HttpGet]
        public ActionResult AddFinanceItem(string financeID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(financeID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                ViewBag.FinanceID = financeID;

                //Get Project ID
                Mongo_Finance_DAO mongoDAO = new Mongo_Finance_DAO();
                string projectID = mongoDAO.Get_ProjectID(financeID);

                //Set Viewbag IsLeader
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                if (ViewBag.IsLeader == true)
                {
                    return PartialView("_FinanceAddItem");
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
        /// <summary>
        /// ファイナンスアイテムを追加
        /// </summary>
        /// <param name="financeID"></param>
        /// <param name="itemInfo"></param>
        /// <param name="txtUserID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFinanceItem(string financeID, FinanceItem itemInfo, string txtUserID)
        {
            //start transaction
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                //variable for validate form
                string errDate = "";
                bool isValid = true;

                //Get Project ID
                Mongo_Finance_DAO mongoDAO = new Mongo_Finance_DAO();
                string projectID = mongoDAO.Get_ProjectID(financeID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.FinanceID = financeID;
                        return PartialView("_FinanceAddItem", itemInfo);
                    };
                    //set new info
                    FinanceItem item = new FinanceItem(itemInfo);

                    if(!ValidationHelper.IsValidDeadlineTwoDate(item.Date, DateTime.Today))
                    {
                        errDate = Error.FINANCEITEM_PAYDATE_INVALID + Environment.NewLine;
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        ViewBag.FinanceID = financeID;
                        ViewBag.MessagePayDate = errDate;
                        return PartialView("_FinanceAddItem", itemInfo);
                    }

                    //create user id
                    if (txtUserID != "")
                    {
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        SDLink user = new SDLink();
                        user = userDAO.Get_SDLink(txtUserID);
                        item.Payer = user;
                    }

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //write data to db
                            mongoDAO.Add_FinanceItem(financeID, item);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailFinance", "Budget", new { projectID = projectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// ファイナンスアイテムを削除
        /// </summary>
        /// <param name="financeID"></param>
        /// <param name="financeItemID"></param>
        /// <returns></returns>
        //Delete Finance Item
        public ActionResult DeleteFinanceItem(string financeID, string financeItemID)
        {  
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
                string thisprojectID = "";

                //get project id
                Mongo_Finance_DAO mongoDAO = new Mongo_Finance_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(financeID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    //start transaction 
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            mongoDAO.Delete_FinanceItem(financeID, financeItemID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("DetailFinance", "Budget", new { projectID = thisprojectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        /// <summary>
        /// 詳細寄金を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Fund View
        public ActionResult DetailFund(string projectID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(projectID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                //Set ViewBag IsLeader
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }
                //set new ViewBag ProjectID
                ViewBag.ProjectID = projectID;

                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                Mongo_Fund result = mongoDAO.Get_Fund(projectID);

                return PartialView("_Fund", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }
        /// <summary>
        /// 詳細寄付者を表示
        /// </summary>
        /// <param name="fundID"></param>
        /// <returns></returns>
        //Detail Team Found Donator
        public ActionResult DetailTeamFoundDonator(string fundID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(fundID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            
            try
            {
                //Get Project ID
                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                string projectID = mongoDAO.Get_ProjectID(fundID);

                //Set ViewBag IsLeader
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.FundID = fundID;

                List<TeamFoundDonator> result = new List<TeamFoundDonator>();
                result = mongoDAO.Get_TeamFoundDonatorList(fundID);

                return PartialView("_FundTeamFoundDonator", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 詳細寄付者を追加画面を表示
        /// </summary>
        /// <param name="fundID"></param>
        /// <returns></returns>
        //AddTeamFoundDonator
        [HttpGet]
        public ActionResult AddTeamFoundDonator(string fundID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(fundID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            try
            {
                //Set ViewBag FundID
                ViewBag.FundID = fundID;

                //Get Project ID
                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                string projectID = mongoDAO.Get_ProjectID(fundID);

                //Set ViewBage IsLeader
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                if (ViewBag.IsLeader == true)
                {
                    return PartialView("_FundAddTeamFoundDonator");
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
        /// <summary>
        /// 詳細寄付者を追加
        /// </summary>
        /// <param name="fundID"></param>
        /// <param name="donator"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddTeamFoundDonator(string fundID, TeamFoundDonator donator)
        {
            

            //start transaction
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                //Get Project ID
                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                string projectID = mongoDAO.Get_ProjectID(fundID);

                //Check is leader or not and do the action
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.FundID = fundID;
                        return PartialView("_FundAddTeamFoundDonator", donator); 
                    };

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //write data to db
                            mongoDAO.Add_TeamFoundDonator(fundID, donator);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailFund", "Budget", new { projectID = projectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 詳細寄付者を削除
        /// </summary>
        /// <param name="fundID"></param>
        /// <param name="teamFoundDonatorID"></param>
        /// <returns></returns>
        //Delete  Team Found Donator
        public ActionResult DeleteTeamFoundDonator(string fundID, string teamFoundDonatorID)
        {
            if (!ModelState.IsValid) return View();

            //start transaction 
            try
            {
                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                //get project id
                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                string projectID = mongoDAO.Get_ProjectID(fundID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //Delete in DB
                            mongoDAO.Delete_TeamFoundDonator(fundID, teamFoundDonatorID);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("DetailFund", "Budget", new { projectID = projectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 振り込み状態を変更画面を表示
        /// </summary>
        /// <param name="fundID"></param>
        /// <param name="teamFoundDonatorID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IsReceiceMoneyChange(string fundID, string teamFoundDonatorID)
        {
            ViewBag.FundID = fundID;
            ViewBag.TeamFoundDonatorID = teamFoundDonatorID;
            return PartialView("_FundIsReceiveTeamFoundDonator");
        }
        /// <summary>
        /// 振り込み状態を変更
        /// </summary>
        /// <param name="fundID"></param>
        /// <param name="teamFoundDonatorID"></param>
        /// <param name="cbxIsReceive"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsReceiceMoneyChange(string fundID, string teamFoundDonatorID, string cbxIsReceive)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(fundID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            //start transaction 
            try
            {
                if (!ModelState.IsValid) return View();

                //check permission
                if (Session["UserID"] == null)
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }

                //get project id
                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                string projectID = mongoDAO.Get_ProjectID(fundID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);

                if (relationDAO.Is_Leader(userID, projectID))
                {

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            if (cbxIsReceive == "true")
                            {

                                mongoDAO.Update_IsReceiveMoney(fundID, teamFoundDonatorID);

                                transaction.Complete();
                            }
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("DetailFund", "Budget", new { projectID = projectID });
                }
                else
                {
                    ViewBag.Message = Error.ACCESS_DENIED;
                    return View("ErrorMessage");
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        ////Detail Outside Donator
        //public ActionResult DetailOutSideDonator(string fundID, string isLeader)
        //{
        //    ViewBag.FunID = fundID;
        //    ViewBag.IsLeader = isLeader;
        //    try
        //    {
        //        Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
        //        List<OutsideDonator> result = new List<OutsideDonator>();
        //        result = mongoDAO.Get_OutsideDonatorList(fundID);

        //        return PartialView("_FundOutsideDonator", result);
        //    }
        //    catch
        //    {
        //        ViewBag.Message = Error.UNEXPECT_ERROR;
        //        return View("_ProjectPlan");
        //    }
        //}

        ////Detail Inside Donator
        //public ActionResult DetailInSideDonator(string fundID, string isLeader)
        //{
        //    ViewBag.FunID = fundID;
        //    ViewBag.IsLeader = isLeader;
        //    try
        //    {
        //        Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
        //        List<InsideDonator> result = new List<InsideDonator>();
        //        result = mongoDAO.Get_InsideDonatorList(fundID);

        //        return PartialView("_FundInsideDonator", result);
        //    }
        //    catch
        //    {
        //        ViewBag.Message = Error.UNEXPECT_ERROR;
        //        return View("_ProjectPlan");
        //    }
        //}

    }
}