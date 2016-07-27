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
        // GET: Budget
        public ActionResult Index()
        {
            return View();
        }

        //Get View
        public ActionResult BudgetAllView(string projectID, string userRole)
        {
            ViewBag.UserRole = userRole;
            ViewBag.ProjectID = projectID;
            return PartialView("_Budget");
        }

        //Get Detail Budget Record
        public ActionResult DetailBudgetRecord(string projectID, string userRole)
        {
            try
            {
                ViewBag.ProjectID = projectID;
                ViewBag.UserRole = userRole;

                SQL_Budget_DAO sql_DAO = new SQL_Budget_DAO();
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                List<BudgetRecordInformation> result = new List<BudgetRecordInformation>();
                result = mongoDAO.Get_BudgetAllRecord(projectID);

                return PartialView("_BudgetRecord", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        [HttpGet]
        public ActionResult AddBudgetRecordForm(string projectID, string userRole)
        {
            ViewBag.ProjectID = projectID;
            ViewBag.UserRole = userRole;

            return PartialView("_BudgetAddRecord");
        }

        //Add Budget Record
        [HttpPost]
        public ActionResult AddBudgetRecord(BudgetRecordInformation budgetInfo, string projectID, string userRole)
        {
            if (!ModelState.IsValid) return View();

            //create project id
            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
            SDLink project = new SDLink();
            project = projectDAO.Get_SDLink(projectID);

            //add mongo Budget
            Mongo_Budget mongo_Budget = new Mongo_Budget(budgetInfo);
            mongo_Budget.BudgetRecordInformation.BudgetRecordID = mongo_Budget._id.ToString();
            mongo_Budget.BudgetRecordInformation.Project = project;

            //Create sql Budget
            SQL_Budget sql_Budget = new SQL_Budget();
            sql_Budget.BudgetID = mongo_Budget._id.ToString();
            sql_Budget.ProjectID = mongo_Budget.BudgetRecordInformation.Project.ID;

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

                        //write data to db
                        sql_Budget_DAO.Add_Budget(sql_Budget);
                        mongo_Budget_DAO.Add_Budget(mongo_Budget);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = projectID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Delete Budget Record
        public ActionResult DeleteBudgetRecord(string budgetID, string userRole)
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
                        Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                        SQL_Budget_DAO sqlDAO = new SQL_Budget_DAO();
                        thisprojectID = sqlDAO.Get_ProjectID(budgetID);

                        sqlDAO.Delete_BudgetRecord(budgetID);
                        mongoDAO.Delete_Budget(budgetID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = thisprojectID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Edit Budget Record
        [HttpGet]
        public ActionResult EditBudgetRecord(string budgetID, string userRole)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.UserRole = userRole;

            return PartialView("_BudgetEditRecord");
        }

        [HttpPost]
        public ActionResult EditBudgetRecord(string budgetID, string userRole, BudgetRecordInformation newinfo)
        {
            if (!ModelState.IsValid) return View();
            string thisprojectID = "";
            ViewBag.UserRole = userRole;

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
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetRecord(budgetID, newinfo);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = thisprojectID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Get Budget item
        public ActionResult DetailBudgetItem(string budgetID, string userRole)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.UserRole = userRole;
            try
            {
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                List<BudgetItem> result = new List<BudgetItem>();
                result = mongoDAO.Get_BudgetItemList(budgetID);

                return PartialView("_BudgetItem", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        //Add Budget Item
        [HttpGet]
        public ActionResult AddBudgetItem(string budgetID, string userRole)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.UserRole = userRole;
            return PartialView("_BudgetAddItem");
        }

        public ActionResult AddBudgetItem(string budgetID, string userRole, BudgetItem itemInfo)
        {
            if (!ModelState.IsValid) return View();

            //set new info
            BudgetItem item = new BudgetItem(itemInfo);

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

                        //write data to db
                        mongo_Budget_DAO.Add_BudgetItem(budgetID, item);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Delete Budget Item
        public ActionResult DeleteBudgetItem(string budgetID, string userRole, string itemContent)
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
                        Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                        SQL_Budget_DAO sqlDAO = new SQL_Budget_DAO();
                        thisprojectID = sqlDAO.Get_ProjectID(budgetID);

                        mongoDAO.Delete_BudgetItem(budgetID, itemContent);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Edit Budget Item
        [HttpGet]
        public ActionResult EditBudgetItem(string budgetID, string itemContent, string userRole)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.ItemContent = itemContent;
            ViewBag.UserRole = userRole;
            return PartialView("_BudgetEditItem");
        }

        [HttpPost]
        public ActionResult EditBudgetItem(string budgetID, string itemContent, string userRole, BudgetItem newinfo)
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
                        BudgetItem item = new BudgetItem(newinfo);
                        // create DAO instance
                        Mongo_Budget_DAO mongo_Budget_DAO = new Mongo_Budget_DAO();
                        SQL_Budget_DAO sql_Budget_DAO = new SQL_Budget_DAO();
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItem(budgetID, itemContent, item);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, userRole = userRole });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Edit Each Field of Budget Item
        [HttpGet]
        public ActionResult EditEachFieldBudgetItem(string budgetID, string field, string itemContent, string userRole)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.Field = field;
            ViewBag.ItemContent = itemContent;
            ViewBag.UserRole = userRole;
            return PartialView("_EditEachFieldBudgetItem");
        }

        [HttpPost]
        public ActionResult EditEachFieldBudgetItem(string budgetID, BudgetItem item, string itemContent, string field, string userRole)
        {
            switch (field)
            {
                case "content":
                    {
                        return RedirectToAction("EditBudgetItemContent", "Budget", new { budgetID = budgetID, itemContent = itemContent, newContent = item.Content, userRole = userRole });
                        break;
                    }
                case "unitprice":
                    {
                        return RedirectToAction("EditBudgetItemUnitPrice", "Budget", new { budgetID = budgetID, itemContent = itemContent, unitPrice = item.UnitPrice, userRole = userRole });
                        break;
                    }
                case "quatity":
                    {
                        return RedirectToAction("EditBudgetItemQuatity", "Budget", new { budgetID = budgetID, itemContent = itemContent, quatity = item.Quatity, userRole = userRole });
                        break;
                    }
                case "unit":
                    {
                        return RedirectToAction("EditBudgetItemUnit", "Budget", new { budgetID = budgetID, itemContent = itemContent, unit = item.Unit, userRole = userRole });
                    }
                default:
                    {
                        ViewBag.Message = Error.WRONG_PASSWORD;
                        return View("ErrorMessage");
                    }
            }
        }

        //update content
        public ActionResult EditBudgetItemContent(string budgetID, string itemContent, string newContent, string userRole)
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
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItemContent(budgetID, itemContent, newContent);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, userRole = userRole });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        //update unitprice
        public ActionResult EditBudgetItemUnitPrice(string budgetID, string itemContent, double unitPrice, string userRole)
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
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItemUnitPrice(budgetID, itemContent, unitPrice);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, userRole = userRole });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //update quatity
        public ActionResult EditBudgetItemQuatity(string budgetID, string itemContent, int quatity, string userRole)
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
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItemQuatity(budgetID, itemContent, quatity);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, userRole = userRole });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //update unit
        public ActionResult EditBudgetItemUnit(string budgetID, string itemContent, string unit, string userRole)
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
                        thisprojectID = sql_Budget_DAO.Get_ProjectID(budgetID);

                        //write data to db
                        mongo_Budget_DAO.Update_BudgetItemUnit(budgetID, itemContent, unit);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, userRole = userRole });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Get Total Budget
        /*public ActionResult GetTotalMoney(string budgetID)
        {
            try
            {
                Mongo_Budget_DAO mongoDAO = new Mongo_Budget_DAO();
                double totalMoney = mongoDAO.Get_TotalBudget(budgetID);
                return 
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }*/

    }
}