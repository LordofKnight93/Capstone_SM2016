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
        /// <summary>
        /// 段階リストを取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Get Detail Plan
        public ActionResult DetailPlanRecord(string projectID)
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
                return PartialView("_Plan");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

        }
        /// <summary>
        /// 詳細段階を取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Get Detail Plan Phase
        public ActionResult DetailPlanPhase(string projectID)
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

                SQL_Plan_DAO sql_DAO = new SQL_Plan_DAO();
                Mongo_Plan_DAO mongo_DAO = new Mongo_Plan_DAO();

                List<PlanPhaseInformation> result = new List<PlanPhaseInformation>();
                result = mongo_DAO.Get_PlanPhaseIsNotComplete(projectID);

                return PartialView("_PlanPhase", result);

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 詳細完成段階を取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Get Detail Plan Phase is Completed
        public ActionResult DetailPlanPhaseIsCompleted(string projectID)
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

                SQL_Plan_DAO sql_DAO = new SQL_Plan_DAO();
                Mongo_Plan_DAO mongo_DAO = new Mongo_Plan_DAO();

                List<PlanPhaseInformation> result = new List<PlanPhaseInformation>();
                result = mongo_DAO.Get_PlanPhaseIsCompleted(projectID);

                return PartialView("_PhaseCompleted", result);

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ActionResult PhaseCompletedMainTaskList(string planPhaseID)
        {
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.PlanPhaseID = planPhaseID;

                List<MainTask> result = new List<MainTask>();
                result = mongoDAO.Get_MainTaskList(planPhaseID);

                return PartialView("_PhaseCompletedMainTaskList", result);
            }
            catch
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public ActionResult PhaseCompletedMainTaskDetail(string planPhaseID, string mainTaskID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(planPhaseID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.PlanPhaseID = planPhaseID;
                ViewBag.MainTaskID = mainTaskID;

                MainTask result = mongoDAO.Get_MainTaskDetail(planPhaseID, mainTaskID);

                return PartialView("_PhaseCompletedMainTaskDetail", result);

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// 段階を追加画面を表示
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Add Plan Phase
        [HttpGet]
        public ActionResult AddPlanPhase(string projectID)
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
                    return PartialView("_AddPlanPhase");
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
        /// 段階を追加
        /// </summary>
        /// <param name="info"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPlanPhase(PlanPhaseInformation info, string projectID)
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
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.ProjectID = projectID;
                        return PartialView("_AddPlanPhase", info);
                    };

                    //variable for validate
                    string errName = "";
                    string errTime = "";
                    bool isValid = true;

                    //create project id
                    Mongo_Project_DAO projectDAO = new Mongo_Project_DAO();
                    SDLink project = projectDAO.Get_SDLink(projectID);

                    //add mongo Budget
                    Mongo_Plan mongo_Plan = new Mongo_Plan(info);
                    mongo_Plan.PlanPhaseInformation.PlanPhaseID = mongo_Plan._id.ToString();
                    mongo_Plan.PlanPhaseInformation.Project = project;

                    // create DAO instance
                    Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();
                    Mongo_Project_DAO mongo_ProjectDAO = new Mongo_Project_DAO();

                    //start validate
                    List<PlanPhaseInformation> currentPhaseList = mongo_Plan_DAO.Get_PlanPhaseOfAProject(projectID);
                    for (int i = 0; i < currentPhaseList.Count(); i++)
                    {
                        if (currentPhaseList[i].Name.Equals(mongo_Plan.PlanPhaseInformation.Name))
                        {
                            errName = Error.PLANPHASE_EXIST + Environment.NewLine;
                            isValid = false;
                        }
                    }

                    if (mongo_Plan.PlanPhaseInformation.StartTime > mongo_Plan.PlanPhaseInformation.EndTime)
                    {
                        errTime += Error.PLANPHASE_TIME_INVALID + Environment.NewLine;
                        isValid = false;
                    }

                    if (mongo_ProjectDAO.Get_ProjectInformation(projectID).DateStart > mongo_Plan.PlanPhaseInformation.StartTime)
                    {
                        errTime += Error.PLANPHASE_START_TIME_INVALID + Error.PROJECT_START_TIME + mongo_ProjectDAO.Get_ProjectInformation(projectID).DateStart.ToString("dd/MM/yyyy") + Environment.NewLine;
                        isValid = false;
                    }

                    if (mongo_Plan.PlanPhaseInformation.EndTime > mongo_ProjectDAO.Get_ProjectInformation(projectID).DateEnd)
                    {
                        errTime += Error.PLANPHASE_END_TIME_INVALID + Error.PROJECT_END_TIME + mongo_ProjectDAO.Get_ProjectInformation(projectID).DateEnd.ToString("dd/MM/yyyy") + Environment.NewLine;
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        ViewBag.MessageName = errName;
                        ViewBag.MessageTime = errTime;
                        ViewBag.ProjectID = projectID;

                        return PartialView("_AddPlanPhase", info);
                    }


                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //write data to db
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
                    return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = projectID });
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
        /// 段階を削除
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <returns></returns>
        //Delete Plan Phase
        public ActionResult DeletePlanPhase(string planPhaseID)
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
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

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

                    return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = thisprojectID });
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
        /// <param name="planPhaseID"></param>
        /// <returns></returns>
        public ActionResult CompletePlanPhase(string planPhaseID)
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
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

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
                            mongoDAO.Update_PlanPhaseIsComplete(planPhaseID, true);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = thisprojectID });
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
        /// <param name="planPhaseID"></param>
        /// <returns></returns>
        public ActionResult ReWorkPlanPhase(string planPhaseID)
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
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

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
                            mongoDAO.Update_PlanPhaseIsComplete(planPhaseID, false);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = thisprojectID });
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
        /// 段階を変更画面を表示
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <returns></returns>
        //Edit Plan Phase
        [HttpGet]
        public ActionResult EditPlanPhase(string planPhaseID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(planPhaseID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                ViewBag.PlanPhaseID = planPhaseID;

                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                if (ViewBag.IsLeader == "True")
                {
                    return PartialView("_EditPlanPhase");
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
        /// 段階を変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="newinfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditPlanPhase(string planPhaseID, PlanPhaseInformation newinfo)
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
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                ViewBag.IsLeader = relationDAO.Is_Leader(userID, thisprojectID);

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    if (!ModelState.IsValid) return PartialView("_EditPlanPhase", newinfo);

                    //start transaction
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {

                            //write data to db
                            mongoDAO.Update_PlanPhaseInfo(planPhaseID, newinfo);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = thisprojectID });
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
        /// メーンタスクを取得
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <returns></returns>
        //Get All Main Task of Phase
        public ActionResult DetailMainTask(string planPhaseID)
        {
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.PlanPhaseID = planPhaseID;

                List<MainTask> result = new List<MainTask>();
                result = mongoDAO.Get_MainTaskList(planPhaseID);

                return PartialView("_MainTask", result);
            }
            catch
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return View("ErrorMessage");
            }
        }
        /// <summary>
        /// 段階のタイトルを変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="txtPhaseName"></param>
        /// <returns></returns>
        //Update Phase Name
        public ActionResult EditPlanPhaseName(string planPhaseID, string txtPhaseName)
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
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    //variable for validate
                    string err = "";
                    bool isValid = true;
                    if (txtPhaseName.Equals(""))
                    {
                        err += Error.PLANPHASE_NAME_NULL + Environment.NewLine;
                        isValid = false;
                    }
                    List<PlanPhaseInformation> currentPhaseList = mongoDAO.Get_PlanPhaseOfAProject(projectID);
                    for (int i = 0; i < currentPhaseList.Count(); i++)
                    {
                        if (currentPhaseList[i].Name.Equals(txtPhaseName) && currentPhaseList[i].PlanPhaseID != planPhaseID)
                        {
                            err += Error.PLANPHASE_EXIST + Environment.NewLine;
                            isValid = false;
                        }
                    }

                    if (!isValid)
                    {
                        ViewBag.PlanPhaseID = planPhaseID;
                        ViewBag.Message = err;
                        return PartialView("_PlanError");
                    }

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //write data to db
                            mongoDAO.Update_PlanPhaseName(planPhaseID, txtPhaseName);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = projectID });
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
        /// 段階の期間を変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="txtStartTime"></param>
        /// <param name="txtEndTime"></param>
        /// <returns></returns>
        //Update Phase Time
        public ActionResult EditPlanPhaseTime(string planPhaseID, string txtStartTime, string txtEndTime)
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
                Mongo_Project_DAO mongo_ProjectDAO = new Mongo_Project_DAO();
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                ViewBag.IsLeader = relationDAO.Is_Leader(userID, thisprojectID);

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    //Variable for validate
                    string err = "";
                    bool isValid = true;

                    //Parse date
                    DateTime startTime = DateTime.Parse(txtStartTime);
                    DateTime endTime = DateTime.Parse(txtEndTime);

                    if (startTime > endTime)
                    {
                        err += Error.PLANPHASE_TIME_INVALID + Environment.NewLine;
                        isValid = false;
                    }

                    if (mongo_ProjectDAO.Get_ProjectInformation(thisprojectID).DateStart > startTime)
                    {
                        err += Error.PLANPHASE_START_TIME_INVALID + Error.PROJECT_START_TIME + mongo_ProjectDAO.Get_ProjectInformation(thisprojectID).DateStart.ToString("dd/MM/yyyy") + Environment.NewLine;
                        isValid = false;
                    }

                    if (endTime > mongo_ProjectDAO.Get_ProjectInformation(thisprojectID).DateEnd)
                    {
                        err += Error.PLANPHASE_END_TIME_INVALID + Error.PROJECT_END_TIME + mongo_ProjectDAO.Get_ProjectInformation(thisprojectID).DateEnd.ToString("dd/MM/yyyy") + Environment.NewLine;
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        ViewBag.Message = err;
                        ViewBag.ProjectID = thisprojectID;

                        return PartialView("_PlanError");
                    }

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //write data to db
                            mongoDAO.Update_PlanPhaseTime(planPhaseID, txtStartTime, txtEndTime);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("DetailPlanPhase", "Plan", new { projectID = thisprojectID });
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
        /// 詳細メーンタスクを取得
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        //Get Main Task Detail
        public ActionResult MainTaskDetail(string planPhaseID, string mainTaskID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(planPhaseID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.PlanPhaseID = planPhaseID;
                ViewBag.MainTaskID = mainTaskID;

                MainTask result = mongoDAO.Get_MainTaskDetail(planPhaseID, mainTaskID);

                return PartialView("_MainTaskDetail", result);

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// メーンタスクを追加画面を表示
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <returns></returns>
        //Add Main Task
        [HttpGet]
        public ActionResult AddMainTask(string planPhaseID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(planPhaseID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }

            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.PlanPhaseID = planPhaseID;

                return PartialView("_AddMainTask");

            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// メーンタスクを追加
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddMainTask(string planPhaseID, MainTask taskInfo)
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
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.PlanPhaseID = planPhaseID;

                        return PartialView("_AddMainTask", taskInfo);
                    }

                    // create DAO instance
                    Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();

                    //variable for validate
                    string errName = "";
                    string errDuedate = "";
                    bool isValid = true;

                    //set new info
                    MainTask task = new MainTask(taskInfo);

                    //start validate
                    List<MainTask> currentMainTaskList = mongo_Plan_DAO.Get_MainTaskList(planPhaseID);
                    for (int i = 0; i < currentMainTaskList.Count(); i++)
                    {
                        if (currentMainTaskList[i].Name.Equals(task.Name))
                        {
                            errName = Error.MAINTASKNAME_EXIST + Environment.NewLine;
                            isValid = false;
                        }
                    }

                    // Duedate 
                    if (!ValidationHelper.IsValidDeadlineWithToday(task.Duedate))
                    {
                        errDuedate = Error.DUEDATE_INVALID_TODAY + Environment.NewLine;
                        isValid = false;
                    }

                    //Check Duedate with phase end time
                    if (!ValidationHelper.IsValidDeadlineTwoDate(task.Duedate, mongo_Plan_DAO.Get_PlanPhase_By_PlanID(planPhaseID).EndTime))
                    {
                        errDuedate += Error.DUEDATE_INVALID_END_PLANPHASE + " Giai đoạn kết thúc ngày: " + mongo_Plan_DAO.Get_PlanPhase_By_PlanID(planPhaseID).EndTime.ToString("dd/MM/yyy hh:mm") + Environment.NewLine;
                        isValid = false;
                    }
                    else

                    //Check Duedate with phase start time
                    if (!ValidationHelper.IsValidDeadlineTwoDate(mongo_Plan_DAO.Get_PlanPhase_By_PlanID(planPhaseID).StartTime, task.Duedate))
                    {
                        errDuedate += Error.DUEDATE_INVALID_START_PLANPHASE + " Giai đoạn bắt đầu ngày: " + mongo_Plan_DAO.Get_PlanPhase_By_PlanID(planPhaseID).StartTime.ToString("dd/MM/yyy hh:mm") + Environment.NewLine;
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        ViewBag.MessageName = errName;
                        ViewBag.MessageDuedate = errDuedate;
                        ViewBag.PlanPhaseID = planPhaseID;

                        return PartialView("_AddMainTask", taskInfo);
                    }

                    //start transaction
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
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
                    return RedirectToAction("DetailMainTask", "Plan", new { planPhaseID = planPhaseID });
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
        /// メーンタスクを削除
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        //Delete Main Task
        public ActionResult DeleteMainTask(string planPhaseID, string mainTaskID)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
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

                    return RedirectToAction("DetailMainTask", "Plan", new { planPhaseID = planPhaseID });
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
        /// メーンタスクのタイトルを変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="txtMainTaskContent"></param>
        /// <returns></returns>
        //Update Main Task Name
        public ActionResult UpdateMainTaskName(string planPhaseID, string mainTaskID, string txtMainTaskContent)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    //variable for validate
                    string err = "";
                    bool isValid = true;

                    if (txtMainTaskContent.Equals(""))
                    {
                        err = Error.MAINTASK_NAME_NULL + Environment.NewLine;
                        isValid = false;
                    }

                    //start validate
                    List<MainTask> currentMainTaskList = mongoDAO.Get_MainTaskList(planPhaseID);
                    for (int i = 0; i < currentMainTaskList.Count(); i++)
                    {
                        if (currentMainTaskList[i].Name.Equals(txtMainTaskContent) && !currentMainTaskList[i].MainTaskID.ToString().Equals(mainTaskID))
                        {
                            err = Error.MAINTASKNAME_EXIST + Environment.NewLine;
                            isValid = false;
                        }
                    }

                    if (!isValid)
                    {
                        ViewBag.PlanPhaseID = planPhaseID;
                        ViewBag.MainTaskID = mainTaskID;
                        ViewBag.Message = err;
                        return PartialView("_PlanError");
                    }

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            // create DAO instance
                            Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();
                            SQL_Plan_DAO sql_Plan_DAO = new SQL_Plan_DAO();

                            //write data to db
                            mongo_Plan_DAO.Update_MainTaskName(planPhaseID, mainTaskID, txtMainTaskContent);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("MainTaskDetail", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// メーンタスクの記述を変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="txtMainTaskDescription"></param>
        /// <returns></returns>
        //Update Main Task Description
        public ActionResult UpdateMainTaskDescription(string planPhaseID, string mainTaskID, string txtMainTaskDescription)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            // create DAO instance
                            Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();
                            SQL_Plan_DAO sql_Plan_DAO = new SQL_Plan_DAO();

                            //write data to db
                            mongo_Plan_DAO.Update_MainTaskDescription(planPhaseID, mainTaskID, txtMainTaskDescription);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("MainTaskDetail", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// メーンタスクの締め切りを変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="txtMainTaskDuedate"></param>
        /// <returns></returns>
        public ActionResult UpdateMainTaskDuedate(string planPhaseID, string mainTaskID, string txtMainTaskDuedate)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    //variable for validate
                    string err = "";
                    bool isValid = true;

                    //start validate

                    //Parse duedate
                    DateTime duedate = DateTime.Parse(txtMainTaskDuedate);

                    // Duedate 
                    if (!ValidationHelper.IsValidDeadlineWithToday(duedate))
                    {
                        err = Error.DUEDATE_INVALID_TODAY + Environment.NewLine;
                        isValid = false;
                    }

                    //Check Duedate with phase end time
                    if (!ValidationHelper.IsValidDeadlineTwoDate(duedate, mongoDAO.Get_PlanPhase_By_PlanID(planPhaseID).EndTime))
                    {
                        err += Error.DUEDATE_INVALID_END_PLANPHASE + " Giai đoạn kết thúc ngày: " + mongoDAO.Get_PlanPhase_By_PlanID(planPhaseID).EndTime.ToString("dd/MM/yyy hh:mm") + Environment.NewLine;
                        isValid = false;
                    }
                    else

                    //Check Duedate with phase start time
                    if (!ValidationHelper.IsValidDeadlineTwoDate(mongoDAO.Get_PlanPhase_By_PlanID(planPhaseID).StartTime, duedate))
                    {
                        err += Error.DUEDATE_INVALID_START_PLANPHASE + " Giai đoạn bắt đầu ngày: " + mongoDAO.Get_PlanPhase_By_PlanID(planPhaseID).StartTime.ToString("dd/MM/yyy hh:mm") + Environment.NewLine;
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        ViewBag.Message = err;
                        ViewBag.PlanPhaseID = planPhaseID;
                        ViewBag.MainTaskID = mainTaskID;

                        return PartialView("_PlanError");
                    }

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            // create DAO instance
                            Mongo_Plan_DAO mongo_Plan_DAO = new Mongo_Plan_DAO();
                            SQL_Plan_DAO sql_Plan_DAO = new SQL_Plan_DAO();

                            //write data to db
                            mongo_Plan_DAO.Update_MainTaskDuedate(planPhaseID, mainTaskID, txtMainTaskDuedate);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("MainTaskDetail", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// タスクを数える
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        //Task Count
        public ActionResult TaskCount(string projectID)
        {
            try
            {
                ViewBag.ProjectID = projectID;
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                TaskCount result = mongoDAO.Get_TaskCount(projectID);

                return PartialView("_TaskCount", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// サーブタスクリストを取得
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        // View All Sub Task
        public ActionResult ListSubTask(string planPhaseID, string mainTaskID)
        {
            try
            {
                ViewBag.PlanPhaseID = planPhaseID;
                ViewBag.MainTaskID = mainTaskID;

                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                List<SubTask> result = mongoDAO.Get_SubTaskList(planPhaseID, mainTaskID);

                return PartialView("_ListSubTask", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 当日のタスクリストを取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        // View List Task of Day
        public ActionResult ListTaskOfThisDay(string projectID)
        {
            try
            {
                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.ProjectID = projectID;
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                List<SubTask> result = mongoDAO.Get_SubTaskListThisDay(projectID, DateTime.Today);

                return PartialView("_TaskOnThisDay", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 正確的タスクを取得
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        // View My Task
        public ActionResult MyTask(string projectID)
        {
            try
            {
                ViewBag.ProjectID = projectID;
                string userID = "";
                if (Session["UserID"] != null)
                {
                    userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                List<SubTask> result = mongoDAO.Get_SubTaskOfUser(projectID, userID);

                return PartialView("_MyTask", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// サーブタスクを追加画面を表示
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        //Add Sub Task
        [HttpGet]
        public ActionResult AddSubTask(string planPhaseID, string mainTaskID)
        {
            // check if parameter valid
            if (String.IsNullOrEmpty(planPhaseID) || Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                ViewBag.PlanPhaseID = planPhaseID;
                ViewBag.MainTaskID = mainTaskID;

                return PartialView("_AddSubTask");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }

        }
        /// <summary>
        /// サーブタスクを追加
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="taskInfo"></param>
        /// <param name="txtUserID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSubTask(string planPhaseID, string mainTaskID, SubTask taskInfo, string txtUserID)
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
                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, projectID))
                {
                    string errContent = "";
                    string errDeadline = "";
                    bool isValid = true;


                    if (!ModelState.IsValid)
                    {
                        ViewBag.PlanPhaseID = planPhaseID;
                        ViewBag.MainTaskID = mainTaskID;
                        return PartialView("_AddSubTask", taskInfo);
                        //return RedirectToAction("AddSubTask", "Plan", new { planPhaseID = planPhaseID, isLeader = isLeader, mainTaskID = mainTaskID });
                    }

                    //set new info
                    SubTask task = new SubTask(taskInfo);

                    //create user id
                    if (!txtUserID.Equals(""))
                    {
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        SDLink user = userDAO.Get_SDLink(txtUserID);
                        task.AssignPeople = user;
                    }

                    ///Check Valid
                    // Exist Content
                    List<SubTask> subTaskList = mongoDAO.Get_SubTaskList(planPhaseID, mainTaskID);
                    for (int i = 0; i < subTaskList.Count(); i++)
                    {
                        if (subTaskList[i].Content.Equals(task.Content))
                        {
                            errContent = Error.SUBTASKNAME_EXIST + Environment.NewLine;
                            isValid = false;
                        }
                    }
                    // Deadline 
                    if (!ValidationHelper.IsValidDeadlineWithToday(task.Deadline))
                    {
                        errDeadline = Error.DEADLINE_INVALID_TODAY + Environment.NewLine;
                        isValid = false;
                    }

                    //Check Deadline with main task duedate
                    if (!ValidationHelper.IsValidDeadlineTwoDate(task.Deadline, mongoDAO.Get_MainTaskDuedate(planPhaseID, mainTaskID)))
                    {
                        errDeadline = Error.DEADLINE_INVALID_MAINTASK + Environment.NewLine;
                        isValid = false;
                    }

                    if (!isValid)
                    {
                        ViewBag.MessageContent = errContent;
                        ViewBag.MessageDeadline = errDeadline;
                        ViewBag.PlanPhaseID = planPhaseID;
                        ViewBag.MainTaskID = mainTaskID;

                        return PartialView("_AddSubTask", taskInfo);
                    }

                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            // create DAO instance
                            SQL_Plan_DAO sql_Plan_DAO = new SQL_Plan_DAO();

                            //write data to db
                            mongoDAO.Add_SubTask(planPhaseID, mainTaskID, task);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }
                    return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// サーブタスクを削除
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <returns></returns>
        //Delete Sub Task
        public ActionResult DeleteSubTask(string planPhaseID, string mainTaskID, string subTaskID)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
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

                    return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// サーブタスクの内容を変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="txtSubTaskContent"></param>
        /// <returns></returns>
        //Update Sub Task Content
        public ActionResult UpdateSubTaskContent(string planPhaseID, string mainTaskID, string subTaskID, string txtSubTaskContent)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            mongoDAO.Update_SubTaskContent(planPhaseID, mainTaskID, subTaskID, txtSubTaskContent);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// サーブタスクの締め切りを変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="txtSubTaskDateline"></param>
        /// <returns></returns>
        //Update Sub Task Dateline
        public ActionResult UpdateSubTaskDateline(string planPhaseID, string mainTaskID, string subTaskID, string txtSubTaskDateline)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            mongoDAO.Update_SubTaskDealine(planPhaseID, mainTaskID, subTaskID, txtSubTaskDateline);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// サーブタスクの状態を変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="slcStatus"></param>
        /// <returns></returns>
        //Change Task Status
        public ActionResult ChangeTaskStatus(string planPhaseID, string mainTaskID, string subTaskID, string slcStatus)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                //Check task is current user's or not
                string currentAssignPeople = mongoDAO.Get_AssignPeople(planPhaseID, mainTaskID, subTaskID);

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID) || userID.Equals(currentAssignPeople))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            mongoDAO.Change_TaskStatus(planPhaseID, mainTaskID, subTaskID, slcStatus);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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

        public ActionResult UpdateSubTaskPriolity(string planPhaseID, string mainTaskID, string subTaskID, string slcPriolity)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            mongoDAO.Update_SubTaskPriolity(planPhaseID, mainTaskID, subTaskID, slcPriolity);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// サーブタスクの担当者を変更
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="txtSubTaskAssignID"></param>
        /// <returns></returns>
        //Update Sub Task Assign
        public ActionResult UpdateSubTaskAssign(string planPhaseID, string mainTaskID, string subTaskID, string txtSubTaskAssignID)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            //create user id
                            if (!txtSubTaskAssignID.Equals(""))
                            {
                                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                                SDLink user = new SDLink();
                                user = userDAO.Get_SDLink(txtSubTaskAssignID);

                                mongoDAO.Update_SubTaskAssign(planPhaseID, mainTaskID, subTaskID, user);

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

                    return RedirectToAction("ListSubTask", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// サーブタスク状態を変更
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="slcStatus"></param>
        /// <returns></returns>
        //Update Sub Task Status in My Task
        public ActionResult ChangeTaskStatusInMyTask(string projectID, string subTaskID, string slcStatus)
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
                string thisprojectID = "";

                //get project id
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();

                string mainTaskID = mongoDAO.Get_MainTaskIDFromSubTaskID(projectID, subTaskID);
                string planPhaseID = mongoDAO.Get_PlanPhaseIDFromMainTaskID(projectID, mainTaskID);

                thisprojectID = mongoDAO.Get_ProjectID(planPhaseID);

                // Check is leader or not
                string userID = Session["UserID"].ToString();

                SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                if (relationDAO.Is_Leader(userID, thisprojectID))
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            mongoDAO.Change_TaskStatusInMyTask(planPhaseID, mainTaskID, subTaskID, slcStatus);

                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            ViewBag.Message = Error.UNEXPECT_ERROR;
                            return PartialView("ErrorMessage");
                        }
                    }

                    return RedirectToAction("MyTask", "Plan", new { projectID = projectID });
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
        /// コメントリストを取得
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        // View All Comment
        public ActionResult ListComment(string planPhaseID, string mainTaskID)
        {
            ViewBag.PlanPhaseID = planPhaseID;
            ViewBag.MainTaskID = mainTaskID;
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                List<Comment> result = mongoDAO.Get_CommentList(planPhaseID, mainTaskID);

                return PartialView("_PlanComment", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// コメントを記載
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="cmtContent"></param>
        /// <returns></returns>
        // Add Comment
        public ActionResult AddComment(string planPhaseID, string mainTaskID, string cmtContent)
        {

            if (!ModelState.IsValid) return View();

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

                        //Get User SDLink
                        Mongo_User_DAO userDAO = new Mongo_User_DAO();
                        SDLink user = userDAO.Get_SDLink(Session["UserID"].ToString());

                        //Create Comment
                        Comment cmt = new Comment(user, cmtContent);

                        //write data to db
                        mongo_Plan_DAO.Add_Comment(planPhaseID, mainTaskID, cmt);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }
                return RedirectToAction("ListComment", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
            }
            catch (Exception e)
            {
                ViewBag.Message = e.ToString();
                return PartialView("ErrorMessage");
                throw;
            }

        }
        /// <summary>
        /// コメントを削除
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="cmtID"></param>
        /// <returns></returns>
        public ActionResult DeleteComment(string planPhaseID, string mainTaskID, string cmtID)
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
                        mongoDAO.Delete_Comment(planPhaseID, mainTaskID, cmtID);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return PartialView("ErrorMessage");
                    }
                }

                return RedirectToAction("ListComment", "Plan", new { planPhaseID = planPhaseID, mainTaskID = mainTaskID });
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
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public ActionResult PhaseCompletedListSubTask(string planPhaseID, string mainTaskID)
        {
            try
            {
                ViewBag.PlanPhaseID = planPhaseID;
                ViewBag.MainTaskID = mainTaskID;

                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                string projectID = mongoDAO.Get_ProjectID(planPhaseID);

                if (Session["UserID"] != null)
                {
                    string userID = Session["UserID"].ToString();
                    SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                    ViewBag.IsLeader = relationDAO.Is_Leader(userID, projectID);
                }

                List<SubTask> result = mongoDAO.Get_SubTaskList(planPhaseID, mainTaskID);

                return PartialView("_PhaseCompletedSubTaskList", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public ActionResult PhaseCompletedListComment(string planPhaseID, string mainTaskID)
        {
            ViewBag.PlanPhaseID = planPhaseID;
            ViewBag.MainTaskID = mainTaskID;
            try
            {
                Mongo_Plan_DAO mongoDAO = new Mongo_Plan_DAO();
                List<Comment> result = mongoDAO.Get_CommentList(planPhaseID, mainTaskID);

                return PartialView("_PhaseCompletedComment", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
                throw;
            }
        }
    }
}