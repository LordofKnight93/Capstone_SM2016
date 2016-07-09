using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;

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
        /// add new item to plan
        /// </summary>
        /// <param name="planID"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add_PlanItem(string planID, PlanItem item)
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
        }
    }
}