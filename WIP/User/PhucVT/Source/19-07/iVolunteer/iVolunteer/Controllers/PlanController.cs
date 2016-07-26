using iVolunteer.Common;
using iVolunteer.DAL.MongoDB;
using iVolunteer.DAL.SQL;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace iVolunteer.Controllers
{
    public class PlanController : Controller
    {
        public PlanController()
        {
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
        }
        // GET: Plan
        public ActionResult Index()
        {
            return View();
        }

        //Get Detail Plan
        public ActionResult DetailPlanRecord(string projectID, string userRole)
        {
            ViewBag.ProjectID = projectID;
            ViewBag.UserRole = userRole;
            return PartialView("_Plan");
        }

        //Get Detail Plan Phase
        public ActionResult DetailPlanPhase(string projectID, string userRole)
        {
            try
            {
                ViewBag.ProjectID = projectID;
                ViewBag.UserRole = userRole;

                SQL_Plan_DAO sql_DAO = new SQL_Plan_DAO();
                Mongo_Plan_DAO mongo_DAO = new Mongo_Plan_DAO();

                List<PlanPhaseInformation> result = new List<PlanPhaseInformation>();
                result = mongo_DAO.Get_PlanPhaseOfAProject(projectID);

                return PartialView("_PlanPhase", result);

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        //Add Plan Phase
        [HttpGet]
        public ActionResult AddPlanPhase(string projectID, string userRole)
        {
            ViewBag.ProjectID = projectID;
            ViewBag.UserRole = userRole;

            return PartialView("_AddPlanPhase");
        }

        [HttpPost]
        public ActionResult AddPlanPhase(PlanPhaseInformation info, string projectID, string userRole)
        {
            if (!ModelState.IsValid) return View();

            //create project id
            Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
            SDLink project = new SDLink();
            project = projectDAO.Get_SDLink(projectID);

            //add mongo Budget
            Mongo_Plan mongo_Plan = new Mongo_Plan(info);
            mongo_Plan.PlanPhaseInformation.PlanPhaseID = mongo_Plan._id.ToString();
            mongo_Plan.PlanPhaseInformation.Project = project;

            //Create sql Budget
            SQL_Plan sql_Plan = new SQL_Plan();
            sql_Plan.PlanID = mongo_Plan._id.ToString();
            sql_Plan.ProjectID = mongo_Plan.PlanPhaseInformation.Project.ID;

            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();
                        SQL_Plan_DAO sql_Plan_DAO = new SQL_Plan_DAO();

                        //write data to db
                        sql_Plan_DAO.Add_Plan(sql_Plan);
                        mongo_Plan_DAO.Add_Plan(mongo_Plan);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = projectID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Delete Plan Phase
        public ActionResult DeletePlanPhase(string planPhaseID, string userRole)
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
                        Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                        SQL_Plan_DAO sqlDAO = new SQL_Plan_DAO();
                        thisprojectID = sqlDAO.Get_ProjectID(planPhaseID);

                        sqlDAO.Delete_PlanPhase(planPhaseID);
                        mongoDAO.Delete_Plan(planPhaseID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = thisprojectID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Edit Plan Phase
        [HttpGet]
        public ActionResult EditPlanPhase(string planPhaseID, string userRole)
        {
            ViewBag.PlanPhaseID = planPhaseID;
            ViewBag.UserRole = userRole;

            return PartialView("_EditPlanPhase");
        }
        [HttpPost]
        public ActionResult EditPlanPhase(string planPhaseID, string userRole, PlanPhaseInformation newinfo)
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
                        Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();
                        SQL_Plan_DAO sql_Plan_DAO = new SQL_Plan_DAO();
                        thisprojectID = sql_Plan_DAO.Get_ProjectID(planPhaseID);

                        //write data to db
                        mongo_Plan_DAO.Update_PlanPhaseInfo(planPhaseID, newinfo);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = thisprojectID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Get All Main Task of Phase
        public ActionResult DetailMainTask(string planPhaseID, string userRole)
        {
            ViewBag.PlanPhaseID = planPhaseID;
            ViewBag.UserRole = userRole;
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                List<MainTask> result = new List<MainTask>();
                result = mongoDAO.Get_MainTaskList(planPhaseID);

                return PartialView("_MainTask", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        //Get Main Task Detail
        public ActionResult MainTaskDetail(string planPhaseID, string userRole, string mainTaskID)
        {
            ViewBag.PlanPhaseID = planPhaseID;
            ViewBag.UserRole = userRole;
            ViewBag.MainTaskID = mainTaskID;
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                MainTask result = mongoDAO.Get_MainTaskDetail(planPhaseID, mainTaskID);

                return PartialView("_MainTaskDetail", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        //Add Main Task
        [HttpGet]
        public ActionResult AddMainTask(string planPhaseID, string userRole)
        {
            ViewBag.PlanPhaseID = planPhaseID;
            ViewBag.UserRole = userRole;
            return PartialView("_AddMainTask");
        }

        [HttpPost]
        public ActionResult AddMainTask(string planPhaseID, string userRole, MainTask taskInfo)
        {
            if (!ModelState.IsValid) return View();

            //set new info
            MainTask task = new MainTask(taskInfo);

            //start transaction
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        // create DAO instance
                        Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();
                        SQL_Plan_DAO sql_Plan_DAO = new SQL_Plan_DAO();

                        //write data to db
                        mongo_Plan_DAO.Add_MainTask(planPhaseID, task);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("DetailMainTask", "Plan", new { planPhaseID = planPhaseID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Delete Main Task
        public ActionResult DeleteMainTask(string planPhaseID, string userRole, string mainTaskID)
        {
            if (!ModelState.IsValid) return View();

            //start transaction 
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                        mongoDAO.Delete_MainTask(planPhaseID, mainTaskID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                return RedirectToAction("DetailMainTask", "Plan", new { planPhaseID = planPhaseID, userRole = userRole });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }

        //Task Count
        public ActionResult TaskCount(string projectID, string userRole)
        {
            ViewBag.ProjectID = projectID;
            ViewBag.UserRole = userRole;
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                TaskCount result = mongoDAO.Get_TaskCount(projectID);

                return PartialView("_TaskCount", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        // View All Sub Task
        public ActionResult ListSubTask(string planPhaseID, string userRole, string mainTaskID)
        {
            ViewBag.PlanPhaseID = planPhaseID;
            ViewBag.UserRole = userRole;
            ViewBag.MainTaskID = mainTaskID;
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                List<SubTask> result = mongoDAO.Get_SubTaskList(planPhaseID, mainTaskID);

                return PartialView("_ListSubTask", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        // View List Task of Day
        public ActionResult ListTaskOfThisDay(string projectID, string userRole)
        {
            ViewBag.ProjectID = projectID;
            ViewBag.UserRole = userRole;
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                List<SubTask> result = mongoDAO.Get_SubTaskListThisDay(projectID, DateTime.Today);

                return PartialView("_TaskOnThisDay", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        // View List Task of Day
        public ActionResult MyTask(string projectID, string userRole)
        {
            ViewBag.ProjectID = projectID;
            ViewBag.UserRole = userRole;
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                List<SubTask> result = mongoDAO.Get_SubTaskOfUser(projectID, userID);

                return PartialView("_MyTask", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("_ProjectPlan");
            }
        }

        //Add Sub Task
        [HttpGet]
        public ActionResult AddSubTask(string planPhaseID, string userRole, string mainTaskID)
        {
            ViewBag.PlanPhaseID = planPhaseID;
            ViewBag.UserRole = userRole;
            ViewBag.MainTaskID = mainTaskID;

            return PartialView("_AddSubTask");
        }

        [HttpPost]
        public ActionResult AddSubTask(string planPhaseID, string userRole, string mainTaskID, SubTask taskInfo, string txtUserID)
        {
            string err = "";
            bool isValid = true;


            if (!ModelState.IsValid) return View();
            else
            {
                //set new info
                SubTask task = new SubTask(taskInfo);

                //create user id
                if (!txtUserID.Equals(""))
                {
                    Mongo_User_DAO userDAO = new Mongo_User_DAO();
                    SDLink user = new SDLink();
                    user = userDAO.Get_SDLink(txtUserID);
                    task.AssignPeople = user;
                }

                //create mongo dao
                Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();

                ///Check Valid
                // Exist Content
                List<SubTask> subTaskList = mongo_Plan_DAO.Get_SubTaskList(planPhaseID, mainTaskID);
                for (int i = 0; i < subTaskList.Count(); i++)
                {
                    if (subTaskList[i].Content.Equals(task.Content))
                    {
                        err = Error.SUBTASKNAME_EXIST + Environment.NewLine;
                        isValid = false;
                    }
                }
                // Deadline 
                if (!ValidationHelper.IsValidDeadline(task.Deadline))
                {
                    err = Error.DEADLINE_INVALID + Environment.NewLine;
                    isValid = false;
                }

                if (!isValid)
                {
                    ViewBag.Message = err;
                    //return RedirectToAction("AddSubTask", "Plan", new { planPhaseID = planPhaseID, userRole = userRole, mainTaskID = mainTaskID });
                    return PartialView("_AddSubTask");
                }

                //start transaction
                try
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            // create DAO instance
                            SQL_Plan_DAO sql_Plan_DAO = new SQL_Plan_DAO();

                            //write data to db
                            mongo_Plan_DAO.Add_SubTask(planPhaseID, mainTaskID, task);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, userRole = userRole, mainTaskID = mainTaskID });
                }
                catch (Exception e)
                {
                    ViewBag.Message = e.ToString();
                    return PartialView("ErrorMessage");
                    throw;
                }
            }
        }

        //Delete Sub Task
        public ActionResult DeleteSubTask(string planPhaseID, string userRole, string mainTaskID, string subTaskID)
        {
            if (!ModelState.IsValid) return View();

            //start transaction 
            try
            {
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                        mongoDAO.Delete_SubTask(planPhaseID, mainTaskID, subTaskID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, userRole = userRole, mainTaskID = mainTaskID });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }
        }
    }
}