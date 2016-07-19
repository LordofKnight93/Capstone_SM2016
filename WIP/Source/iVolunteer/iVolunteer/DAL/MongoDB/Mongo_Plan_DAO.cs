using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;

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

        //Get Plan Phase Information
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

        //Get List Plan Phase Record
        public List<PlanPhaseInformation> Get_PlanPhaseOfAProject(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().Where(pl => pl.PlanPhaseInformation.Project.ID == projectID)
                   .Select(bg => bg.PlanPhaseInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        //Get list Maintask
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

        //Add Main task
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

        //Delete Main Task
        public bool Delete_BudgetItem(string planID, string taskName)
        {
            try
            {
                var plan_filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl._id, new ObjectId(planID));
                var maintask_filter = Builders<MainTask>.Filter.Eq(mt => mt.Name, taskName);
                var update = Builders<Mongo_Plan>.Update.PullFilter(pl => pl.MainTask, maintask_filter);
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Update Plan Phase Info
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

        //Update Main Task Info
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

        //Add Sub Task
        public bool AddSubTask(string planPhaseID, string maintaskName, SubTask task)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.And(
                                                    Builders<Mongo_Plan>.Filter.Eq(pl => pl.PlanPhaseInformation.PlanPhaseID, planPhaseID),
                                                    Builders<Mongo_Plan>.Filter.ElemMatch(pl => pl.MainTask, it => it.Name == maintaskName));
                /*var update = Builders<Mongo_Plan>.Update.Set(pl => pl.MainTask[-1].Name, task.Name)
                                                         .Set(pl => pl.MainTask[-1].Description, task.Description)
                                                         .Set(pl => pl.MainTask[-1].Assign, task.Assign)
                                                         .Set(pl => pl.MainTask[-1].Duedate, task.Duedate);*/
                var update = Builders<Mongo_Plan>.Update.AddToSet<SubTask>(pl => pl.MainTask[-1].Subtask, task);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add new item to plan
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        /*public bool Add_PlanItem(string planID, PlanItem item)
        {
            try
            {
                var filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl._id, new ObjectId(planID));
                var update = Builders<Mongo_Plan>.Update.AddToSet(pl => pl.ItemList, item);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete an item
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public bool Delete_PlanItem(string planID, string itemID)
        {
            try
            {
                var plan_filter = Builders<Mongo_Plan>.Filter.Eq(pl => pl._id, new ObjectId(planID));
                var item_filter = Builders<PlanItem>.Filter.Eq(it => it.ItemID, itemID);
                var update = Builders<Mongo_Plan>.Update.PullFilter(pl => pl.ItemList, item_filter);
                var result = collection.UpdateOne(plan_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }*/
    }
}