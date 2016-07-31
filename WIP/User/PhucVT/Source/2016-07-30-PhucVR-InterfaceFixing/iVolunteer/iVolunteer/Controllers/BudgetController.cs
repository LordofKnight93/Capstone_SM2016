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
        public ActionResult BudgetAllView(string projectID, string isLeader)
        {
            ViewBag.IsLeader = isLeader;
            ViewBag.ProjectID = projectID;

            //Get Received Money in Fund
            Mongo_Fund_DAO fundDAO = new Mongo_Fund_DAO();
            ViewBag.ReceivedMoney = fundDAO.Get_ReceivedMoney(projectID);

            //Get Total Cost Estimated
            Mongo_Budget_DAO budgetDAO = new Mongo_Budget_DAO();
            ViewBag.TotalEstimated = budgetDAO.Get_TotalMoneyCost(projectID);

            return PartialView("_Budget");
        }

        //Get Detail Budget Record
        public ActionResult DetailBudgetRecord(string projectID, string isLeader)
        {
            try
            {
                ViewBag.ProjectID = projectID;
                ViewBag.IsLeader = isLeader;

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
        public ActionResult AddBudgetRecordForm(string projectID, string isLeader)
        {
            ViewBag.ProjectID = projectID;
            ViewBag.IsLeader = isLeader;

            return PartialView("_BudgetAddRecord");
        }

        //Add Budget Record
        [HttpPost]
        public ActionResult AddBudgetRecord(BudgetRecordInformation budgetInfo, string projectID, string isLeader)
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
                return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = projectID, isLeader = isLeader });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Delete Budget Record
        public ActionResult DeleteBudgetRecord(string budgetID, string isLeader)
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

                return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = thisprojectID, isLeader = isLeader });
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
        public ActionResult EditBudgetRecord(string budgetID, string isLeader)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.IsLeader = isLeader;

            return PartialView("_BudgetEditRecord");
        }

        [HttpPost]
        public ActionResult EditBudgetRecord(string budgetID, string isLeader, BudgetRecordInformation newinfo)
        {
            if (!ModelState.IsValid) return View();
            string thisprojectID = "";
            ViewBag.IsLeader = isLeader;

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
                return RedirectToAction("DetailBudgetRecord", "Budget", new { projectID = thisprojectID, isLeader = isLeader });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Get Budget item
        public ActionResult DetailBudgetItem(string budgetID, string isLeader)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.IsLeader = isLeader;
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
        public ActionResult AddBudgetItem(string budgetID, string isLeader)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.IsLeader = isLeader;
            return PartialView("_BudgetAddItem");
        }

        public ActionResult AddBudgetItem(string budgetID, string isLeader, BudgetItem itemInfo)
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
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, isLeader = isLeader });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Delete Budget Item
        public ActionResult DeleteBudgetItem(string budgetID, string isLeader, string itemContent)
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

                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, isLeader = isLeader });
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
        public ActionResult EditBudgetItem(string budgetID, string itemContent, string isLeader)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.ItemContent = itemContent;
            ViewBag.IsLeader = isLeader;
            return PartialView("_BudgetEditItem");
        }

        [HttpPost]
        public ActionResult EditBudgetItem(string budgetID, string itemContent, string isLeader, BudgetItem newinfo)
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
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, isLeader = isLeader });
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
        public ActionResult EditEachFieldBudgetItem(string budgetID, string field, string itemContent, string isLeader)
        {
            ViewBag.BudgetID = budgetID;
            ViewBag.Field = field;
            ViewBag.ItemContent = itemContent;
            ViewBag.IsLeader = isLeader;
            return PartialView("_EditEachFieldBudgetItem");
        }

        [HttpPost]
        public ActionResult EditEachFieldBudgetItem(string budgetID, BudgetItem item, string itemContent, string field, string isLeader)
        {
            switch (field)
            {
                case "content":
                    {
                        return RedirectToAction("EditBudgetItemContent", "Budget", new { budgetID = budgetID, itemContent = itemContent, newContent = item.Content, isLeader = isLeader });
                        break;
                    }
                case "unitprice":
                    {
                        return RedirectToAction("EditBudgetItemUnitPrice", "Budget", new { budgetID = budgetID, itemContent = itemContent, unitPrice = item.UnitPrice, isLeader = isLeader });
                        break;
                    }
                case "quatity":
                    {
                        return RedirectToAction("EditBudgetItemQuatity", "Budget", new { budgetID = budgetID, itemContent = itemContent, quatity = item.Quatity, isLeader = isLeader });
                        break;
                    }
                case "unit":
                    {
                        return RedirectToAction("EditBudgetItemUnit", "Budget", new { budgetID = budgetID, itemContent = itemContent, unit = item.Unit, isLeader = isLeader });
                    }
                default:
                    {
                        ViewBag.Message = Error.WRONG_PASSWORD;
                        return View("ErrorMessage");
                    }
            }
        }

        //update content
        public ActionResult EditBudgetItemContent(string budgetID, string itemContent, string newContent, string isLeader)
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
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, isLeader = isLeader });
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
        public ActionResult EditBudgetItemUnitPrice(string budgetID, string itemContent, double unitPrice, string isLeader)
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
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, isLeader = isLeader });
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
        public ActionResult EditBudgetItemQuatity(string budgetID, string itemContent, int quatity, string isLeader)
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
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, isLeader = isLeader });
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
        public ActionResult EditBudgetItemUnit(string budgetID, string itemContent, string unit, string isLeader)
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
                return RedirectToAction("DetailBudgetItem", "Budget", new { budgetID = budgetID, isLeader = isLeader });
                //return PartialView("_ProjectBudgetItem");
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        ///sumary
        /// Finance
        /// 

        //Finance View
        public ActionResult DetailFinance(string projectID, string isLeader)
        {
            try
            {
                //set new ViewBag
                ViewBag.ProjectID = projectID;
                ViewBag.IsLeader = isLeader;

                Mongo_Finance_DAO mongoDAO = new Mongo_Finance_DAO();
                Mongo_Finance result = mongoDAO.Get_Finance(projectID);

                Mongo_Fund_DAO fundDAO = new Mongo_Fund_DAO();
                ViewBag.ReceivedMoney = fundDAO.Get_ReceivedMoney(projectID);

                return PartialView("_Finance", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        public ActionResult DetailFinanceItem(string financeID, string isLeader)
        {
            ViewBag.FinanceID = financeID;
            ViewBag.IsLeader = isLeader;
            try
            {
                Mongo_Finance_DAO mongoDAO = new Mongo_Finance_DAO();
                List<FinanceItem> result = new List<FinanceItem>();
                result = mongoDAO.Get_FinanceItemList(financeID);

                return PartialView("_FinanceDetails", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        //Finance Add Item
        [HttpGet]
        public ActionResult AddFinanceItem(string financeID, string isLeader)
        {
            ViewBag.FinanceID = financeID;
            ViewBag.IsLeader = isLeader;
            return PartialView("_FinanceAddItem");
        }

        [HttpPost]
        public ActionResult AddFinanceItem(string financeID, string isLeader, FinanceItem itemInfo, string txtPayer)
        {
            if (!ModelState.IsValid) return View();

            //create user id
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            SDLink user = new SDLink();
            user = userDAO.Get_SDLink(txtPayer);

            //set new info
            FinanceItem item = new FinanceItem(itemInfo);
            item.Payer = user;

            //projectID
            string projectID = "";

            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Finance_DAO mongo_DAO = new Mongo_Finance_DAO();

                        //write data to db
                        mongo_DAO.Add_FinanceItem(financeID, item);

                        //Get ProjectID
                        projectID = mongo_DAO.Get_ProjectID(financeID);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailFinance", "Budget", new { projectID = projectID, isLeader = isLeader });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Delete Finance Item
        public ActionResult DeleteFinanceItem(string financeID, string isLeader, string financeItemID)
        {
            if (!ModelState.IsValid) return View();

            //projectID
            string projectID = "";

            //start transaction 
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        Mongo_Finance_DAO mongoDAO = new Mongo_Finance_DAO();

                        mongoDAO.Delete_FinanceItem(financeID, financeItemID);

                        //Get projectID
                        projectID = mongoDAO.Get_ProjectID(financeID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                return RedirectToAction("DetailFinance", "Budget", new { projectID = projectID, isLeader = isLeader });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        ///
        /// Fund
        /// 

        //Fund View
        public ActionResult DetailFund(string projectID, string isLeader)
        {
            try
            {
                //set new ViewBag
                ViewBag.ProjectID = projectID;
                ViewBag.IsLeader = isLeader;

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

        //Detail Team Found Donator
        public ActionResult DetailTeamFoundDonator(string fundID, string isLeader)
        {
            ViewBag.FundID = fundID;
            ViewBag.IsLeader = isLeader;
            try
            {
                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                List<TeamFoundDonator> result = new List<TeamFoundDonator>();
                result = mongoDAO.Get_TeamFoundDonatorList(fundID);

                return PartialView("_FundTeamFoundDonator", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        //AddTeamFoundDonator
        [HttpGet]
        public ActionResult AddTeamFoundDonator(string fundID, string isLeader)
        {
            ViewBag.FundID = fundID;
            ViewBag.IsLeader = isLeader;
            return PartialView("_FundAddTeamFoundDonator");
        }

        [HttpPost]
        public ActionResult AddTeamFoundDonator(string fundID, string isLeader, TeamFoundDonator donator)
        {
            if (!ModelState.IsValid) return View();

            //projectID
            string projectID = "";

            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Fund_DAO mongo_DAO = new Mongo_Fund_DAO();

                        //write data to db
                        mongo_DAO.Add_TeamFoundDonator(fundID, donator);

                        //Get ProjectID
                        projectID = mongo_DAO.Get_ProjectID(fundID);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailFund", "Budget", new { projectID = projectID, isLeader = isLeader });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Delete  Team Found Donator
        public ActionResult DeleteTeamFoundDonator(string fundID, string isLeader, string teamFoundDonatorID)
        {
            if (!ModelState.IsValid) return View();

            //projectID
            string projectID = "";

            //start transaction 
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();

                        mongoDAO.Delete_TeamFoundDonator(fundID, teamFoundDonatorID);

                        //Get projectID
                        projectID = mongoDAO.Get_ProjectID(fundID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                return RedirectToAction("DetailFund", "Budget", new { projectID = projectID, isLeader = isLeader });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
        [HttpGet]
        public ActionResult IsReceiceMoneyChange(string fundID, string isLeader, string teamFoundDonatorID)
        {
            ViewBag.FundID = fundID;
            ViewBag.IsLeader = isLeader;
            ViewBag.TeamFoundDonatorID = teamFoundDonatorID;
            return PartialView("_FundIsReceiveTeamFoundDonator");
        }

        [HttpPost]
        public ActionResult IsReceiceMoneyChange(string fundID, string isLeader, string teamFoundDonatorID, string cbxIsReceive)
        {
            if (!ModelState.IsValid) return View();

            //projectID
            string projectID = "";

            //start transaction 
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        if (cbxIsReceive == "on")
                        {
                            Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();

                            mongoDAO.Update_IsReceiveMoney(fundID, teamFoundDonatorID);

                            //Get projectID
                            projectID = mongoDAO.Get_ProjectID(fundID);

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

                return RedirectToAction("DetailFund", "Budget", new { projectID = projectID, isLeader = isLeader });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Detail Outside Donator
        public ActionResult DetailOutSideDonator(string fundID, string isLeader)
        {
            ViewBag.FunID = fundID;
            ViewBag.IsLeader = isLeader;
            try
            {
                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                List<OutsideDonator> result = new List<OutsideDonator>();
                result = mongoDAO.Get_OutsideDonatorList(fundID);

                return PartialView("_FundOutsideDonator", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        //Detail Inside Donator
        public ActionResult DetailInSideDonator(string fundID, string isLeader)
        {
            ViewBag.FunID = fundID;
            ViewBag.IsLeader = isLeader;
            try
            {
                Mongo_Fund_DAO mongoDAO = new Mongo_Fund_DAO();
                List<InsideDonator> result = new List<InsideDonator>();
                result = mongoDAO.Get_InsideDonatorList(fundID);

                return PartialView("_FundInsideDonator", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

    }
}