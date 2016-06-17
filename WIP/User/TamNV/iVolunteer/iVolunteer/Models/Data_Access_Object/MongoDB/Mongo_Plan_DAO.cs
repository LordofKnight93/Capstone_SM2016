using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public class Mongo_Plan_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Plan> collection = db.GetCollection<Mongo_Plan>("Plan");
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
    }
}