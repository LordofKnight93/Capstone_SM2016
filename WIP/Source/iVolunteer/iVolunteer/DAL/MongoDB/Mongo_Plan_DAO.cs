using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using System.Data.Entity.Core.Objects;
using iVolunteer.Common;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_Plan_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Plan> collection = db.GetCollection<Mongo_Plan>("Plan");

        /// <summary>
        /// add new plan to mongoDB
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        public bool Add_Plan(Mongo_Plan plan)
        {
            try
            {
                collection.InsertOne(plan);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete plan
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public bool Delete_Plan(string planID)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl._id, new ObjectId(planID));
                var result = collection.DeleteOne(filter);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get Project ID
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <returns></returns>
        public string Get_ProjectID(string planPhaseID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(pl => pl.PlanPhaseInformation.PlanPhaseID == planPhaseID);
                return result.PlanPhaseInformation.Project.ID;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get PlanPhaseID From Main Task 
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public string Get_PlanPhaseIDFromMainTaskID(string projectID, string mainTaskID)
        {
            try
            {
                var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.Project.ID == projectID).ToList();
                int count = 0;
                for (int i = 0; i < result.Count(); i++)
                {
                    for(int j=0; i< result[i].MainTask.Count(); j++)
                    {
                        if (result[i].MainTask[j].MainTaskID == ObjectId.Parse(mainTaskID))
                        {
                            count = i;
                            break;
                        }
                    }
                }
                return result[count].PlanPhaseInformation.PlanPhaseID;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get Plan Phase Information
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public PlanPhaseInformation Get_PlanPhase_By_PlanID(string planID)
        {
            try
            {
                //var filter = Builders<Mongo_Budget>.Filter.Eq(bg => bg.Project.ID, projectID);
                //var result = collection.AsQueryable().FirstOrDefault(bg => bg.BudgetRecordInformation.Project.ID == projectID);
                //return result.BudgetRecordInformation;
                var filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planID);
                var result = collection.Find(filter).FirstOrDefault();
                return result.PlanPhaseInformation;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get List Plan Phase Record
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<PlanPhaseInformation> Get_PlanPhaseOfAProject(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.Project.ID == projectID)
                   .Select(pl => pl.PlanPhaseInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get maintask detail
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public MainTask Get_MainTaskDetail(string planPhaseID, string mainTaskID)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.PlanPhaseID == planPhaseID).SelectMany(mt => mt.MainTask).Where(mt => mt.MainTaskID == ObjectId.Parse(mainTaskID)).ToList();
            return result.ElementAt(0);
        }

        /// <summary>
        /// Get Main Task Duedate
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public DateTime Get_MainTaskDuedate(string planPhaseID, string mainTaskID)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.PlanPhaseID == planPhaseID).SelectMany(mt => mt.MainTask).Where(mt => mt.MainTaskID == ObjectId.Parse(mainTaskID)).ToList();
            return result.ElementAt(0).Duedate;
        }

        /// <summary>
        /// Get Main Task Name
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public string Get_MainTaskName(string planPhaseID, string mainTaskID)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.PlanPhaseID == planPhaseID).SelectMany(mt => mt.MainTask).Where(mt => mt.MainTaskID == ObjectId.Parse(mainTaskID)).ToList();
            return result.ElementAt(0).Name;
        }

        /// <summary>
        /// Get list Maintask
        /// </summary>
        /// <param name="planID"></param>
        /// <returns></returns>
        public List<MainTask> Get_MainTaskList(string planID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(bg => bg.PlanPhaseInformation.PlanPhaseID == planID);
                return result.MainTask.ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add Main task
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Add_MainTask(string planID, MainTask task)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl._id, new ObjectId(planID));
                var update = Builders<Mongo_Plan>.Update.AddToSet<MainTask>(pl => pl.MainTask, task);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete Main Task
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public bool Delete_MainTask(string planPhaseID, string mainTaskID)
        {
            try
            {
                var plan_filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl._id, new ObjectId(planPhaseID));
                var maintask_filter = Builders<MainTask>.Filter.Eq(mt => mt.MainTaskID, ObjectId.Parse(mainTaskID));
                var update = Builders<Mongo_Plan>.Update.PullFilter(pl => pl.MainTask, maintask_filter);
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Plan Phase Info
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="newInfo"></param>
        /// <returns></returns>
        public bool Update_PlanPhaseInfo(string planPhaseID, PlanPhaseInformation newInfo)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID);
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.PlanPhaseInformation.Name, newInfo.Name)
                                                        .Set(pl => pl.PlanPhaseInformation.StartTime, newInfo.StartTime)
                                                        .Set(pl => pl.PlanPhaseInformation.EndTime, newInfo.EndTime)
                                                        .Set(pl => pl.PlanPhaseInformation.Description, newInfo.Description);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Phase Name
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool Update_PlanPhaseName(string planPhaseID, string newName)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID);
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.PlanPhaseInformation.Name, newName);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Phase Time
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool Update_PlanPhaseTime(string planPhaseID, string startTime, string endTime)
        {
            try
            {
                DateTime StartTime = Convert.ToDateTime(startTime);
                DateTime EndTime = Convert.ToDateTime(endTime);
                var filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID);
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.PlanPhaseInformation.StartTime, StartTime)
                                                        .Set(pl => pl.PlanPhaseInformation.EndTime, EndTime);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Main Task Info
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="maintaskName"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Update_MainTask(string planPhaseID, string maintaskName, MainTask task)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.Name == maintaskName));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask[-1].Name, task.Name)
                                                         .Set(pl => pl.MainTask[-1].Description, task.Description)
                                                         .Set(pl => pl.MainTask[-1].Assign, task.Assign)
                                                         .Set(pl => pl.MainTask[-1].Duedate, task.Duedate);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Main Task Name
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="maintaskName"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Update_MainTaskName(string planPhaseID, string mainTaskID, string newName)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask[-1].Name, newName);

                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Main Task Description
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public bool Update_MainTaskDescription(string planPhaseID, string mainTaskID, string taskDescription)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask[-1].Description, taskDescription);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Main Task Due Date
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="taskDuedate"></param>
        /// <returns></returns>
        public bool Update_MainTaskDuedate(string planPhaseID, string mainTaskID, string taskDuedate)
        {
            try
            {
                DateTime newDuedate = Convert.ToDateTime(taskDuedate);
                var filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask[-1].Duedate, newDuedate);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add Sub Task
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Add_SubTask(string planPhaseID, string mainTaskID, SubTask task)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.AddToSet<SubTask>(pl => pl.MainTask[-1].Subtask, task).Inc(pl => pl.MainTask[-1].SubTaskCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get List Sub Task
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public List<SubTask> Get_SubTaskList(string planPhaseID, string mainTaskID)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.PlanPhaseID == planPhaseID).SelectMany(mt => mt.MainTask).Where(mt => mt.MainTaskID == ObjectId.Parse(mainTaskID)).Select(mt => mt.Subtask).ToList();
            return result.ElementAt(0);
        }

        /// <summary>
        /// Get Task Count
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public TaskCount Get_TaskCount(string projectID)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.Project.ID == projectID).SelectMany(mt => mt.MainTask).ToList();
            List<SubTask> subtask = new List<SubTask>();
            int all = 0;
            int pending = 0;
            int doing = 0;
            int done = 0;
            int rework = 0;

            for (int i = 0; i < result.Count(); i++)
            {
                for (int j = 0; j < result[i].Subtask.Count(); j++)
                {
                    all += 1;
                    if (result[i].Subtask[j].IsDone == 1)
                    {
                        pending += 1;
                    }
                    else if (result[i].Subtask[j].IsDone == 2)
                    {
                        doing += 1;
                    }
                    else if(result[i].Subtask[j].IsDone == 3)
                    {
                        done += 1;
                    }
                    else
                    {
                        rework += 1;
                    }
                }
            }
            return new TaskCount(all, done, doing, pending, rework);
        }

        /// <summary>
        /// Get Main Task ID From Sub Task ID
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="subTaskID"></param>
        /// <returns></returns>
        public string Get_MainTaskIDFromSubTaskID(string projectID, string subTaskID)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.Project.ID == projectID).SelectMany(mt => mt.MainTask).ToList();
            int count = 0;

            for (int i = 0; i < result.Count(); i++)
            {
                for (int j = 0; j < result[i].Subtask.Count(); j++)
                {
                    if (result[i].Subtask[j].SubTaskID == ObjectId.Parse(subTaskID))
                    {
                        count = i;
                        break;
                    }
                }
            }
            return result[count].MainTaskID.ToString();
        }

        /// <summary>
        /// Get List Sub Task on this day
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<SubTask> Get_SubTaskListThisDay(string projectID, DateTime date)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.Project.ID == projectID).SelectMany(mt => mt.MainTask).ToList();
            List<SubTask> subtask = new List<SubTask>();
            for(int i = 0; i<result.Count(); i++)
            {
                subtask.AddRange(result[i].Subtask.Where(mt => mt.Deadline >= date).ToList());
            }
            subtask = subtask.OrderBy(st => st.Deadline).ToList();
            return subtask;
        }

        /// <summary>
        /// Get List Sub Task of User
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<SubTask> Get_SubTaskOfUser(string projectID, string userID)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.Project.ID == projectID).SelectMany(mt => mt.MainTask).ToList();
            List<SubTask> subtask = new List<SubTask>();
            for (int i = 0; i < result.Count(); i++)
            {
                subtask.AddRange(result[i].Subtask.Where(mt => mt.AssignPeople.ID == userID).ToList());
            }
            subtask = subtask.OrderBy(st => st.Deadline).ToList();
            return subtask;
        }

        /// <summary>
        /// Delete Sub Task
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <returns></returns>
        public bool Delete_SubTask(string planPhaseID, string mainTaskID, string subTaskID)
        {
            try
            {
                List<SubTask> subtasklist = Get_SubTaskList(planPhaseID, mainTaskID);
                int i = subtasklist.FindIndex(st => st.SubTaskID == ObjectId.Parse(subTaskID));
                int taskstatus = subtasklist[i].IsDone;

                var plan_filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var subtask_filter = Builders<SubTask>.Filter.Eq(st => st.SubTaskID, ObjectId.Parse(subTaskID));
                var update = Builders<Mongo_Plan>.Update.PullFilter(pl => pl.MainTask[-1].Subtask, subtask_filter).Inc(pl => pl.MainTask[-1].SubTaskCount, -1);

                if (taskstatus == SubTaskIsDone.DONE)
                {
                    update = update.Inc(pl => pl.MainTask.ElementAt(-1).TaskDoneCount, -1);
                }

                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Change Task Status
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool Change_TaskStatus(string planPhaseID, string mainTaskID, string subTaskID, string status)
        {
            try
            {
                List<SubTask> subtasklist = Get_SubTaskList(planPhaseID, mainTaskID);
                int i = subtasklist.FindIndex(st => st.SubTaskID == ObjectId.Parse(subTaskID));
                int taskstatus = subtasklist[i].IsDone;

                var plan_filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask.ElementAt(-1).Subtask.ElementAt(i).IsDone, Int32.Parse(status));
                if(taskstatus != SubTaskIsDone.DONE && Int32.Parse(status) == SubTaskIsDone.DONE)
                {
                    update = update.Inc(pl => pl.MainTask.ElementAt(-1).TaskDoneCount, 1);
                }
                else if(taskstatus == SubTaskIsDone.DONE && Int32.Parse(status) != SubTaskIsDone.DONE)
                {
                    update = update.Inc(pl => pl.MainTask.ElementAt(-1).TaskDoneCount, -1);
                }
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Sub Task Content
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="newContent"></param>
        /// <returns></returns>
        public bool Update_SubTaskContent(string planPhaseID, string mainTaskID, string subTaskID, string newContent)
        {
            try
            {
                List<SubTask> subtasklist = Get_SubTaskList(planPhaseID, mainTaskID);
                int i = subtasklist.FindIndex(st => st.SubTaskID == ObjectId.Parse(subTaskID));

                var plan_filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask.ElementAt(-1).Subtask.ElementAt(i).Content, newContent);
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Sub Task Priolity
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="priolity"></param>
        /// <returns></returns>
        public bool Update_SubTaskPriolity(string planPhaseID, string mainTaskID, string subTaskID, string priolity)
        {
            try
            {
                List<SubTask> subtasklist = Get_SubTaskList(planPhaseID, mainTaskID);
                int i = subtasklist.FindIndex(st => st.SubTaskID == ObjectId.Parse(subTaskID));

                var plan_filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask.ElementAt(-1).Subtask.ElementAt(i).Priolity, Int32.Parse(priolity));
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Sub Task Deadline
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="newDateline"></param>
        /// <returns></returns>
        public bool Update_SubTaskDealine(string planPhaseID, string mainTaskID, string subTaskID, string newDateline)
        {
            try
            {
                DateTime newDate = Convert.ToDateTime(newDateline);
                List<SubTask> subtasklist = Get_SubTaskList(planPhaseID, mainTaskID);
                int i = subtasklist.FindIndex(st => st.SubTaskID == ObjectId.Parse(subTaskID));

                var plan_filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask.ElementAt(-1).Subtask.ElementAt(i).Deadline, newDate);
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Change Assign People
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Update_SubTaskAssign(string planPhaseID, string mainTaskID, string subTaskID, SDLink user)
        {
            try
            {
                List<SubTask> subtasklist = Get_SubTaskList(planPhaseID, mainTaskID);
                int i = subtasklist.FindIndex(st => st.SubTaskID == ObjectId.Parse(subTaskID));

                var plan_filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask.ElementAt(-1).Subtask.ElementAt(i).AssignPeople, user);
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Change Task Status
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="subTaskID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool Change_TaskStatusInMyTask(string planPhaseID, string mainTaskID, string subTaskID, string status)
        {
            try
            {
                List<SubTask> subtasklist = Get_SubTaskList(planPhaseID, mainTaskID);
                int i = subtasklist.FindIndex(st => st.SubTaskID == ObjectId.Parse(subTaskID));
                int taskstatus = subtasklist[i].IsDone;

                var plan_filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask.ElementAt(-1).Subtask.ElementAt(i).IsDone, Int32.Parse(status));
                if (taskstatus != SubTaskIsDone.DONE && Int32.Parse(status) == SubTaskIsDone.DONE)
                {
                    update = update.Inc(pl => pl.MainTask.ElementAt(-1).TaskDoneCount, 1);
                }
                else if (taskstatus == SubTaskIsDone.DONE && Int32.Parse(status) != SubTaskIsDone.DONE)
                {
                    update = update.Inc(pl => pl.MainTask.ElementAt(-1).TaskDoneCount, -1);
                }
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get List Comment
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <returns></returns>
        public List<Comment> Get_CommentList(string planPhaseID, string mainTaskID)
        {
            var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.PlanPhaseID == planPhaseID).SelectMany(mt => mt.MainTask).Where(mt => mt.MainTaskID == ObjectId.Parse(mainTaskID)).Select(mt => mt.Comment).ToList().ElementAt(0);
            return result.OrderByDescending(cm => cm.DateCreate).ToList();
        }

        /// <summary>
        /// Add New comment
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Add_Comment(string planPhaseID, string mainTaskID, Comment cmt)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var update = Builders<Mongo_Plan>.Update.AddToSet<Comment>(pl => pl.MainTask[-1].Comment, cmt).Inc(pl => pl.MainTask[-1].CommentCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete Own Comment
        /// </summary>
        /// <param name="planPhaseID"></param>
        /// <param name="mainTaskID"></param>
        /// <param name="cmtID"></param>
        /// <returns></returns>
        public bool Delete_Comment(string planPhaseID, string mainTaskID, string cmtID)
        {
            try
            {
                var plan_filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.MainTaskID == ObjectId.Parse(mainTaskID)));
                var cmt_filter = Builders<Comment>.Filter.Eq(cmt => cmt.CommentID, cmtID);
                var update = Builders<Mongo_Plan>.Update.PullFilter(pl => pl.MainTask[-1].Comment, cmt_filter).Inc(pl => pl.MainTask[-1].CommentCount, -1);
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

    }
}