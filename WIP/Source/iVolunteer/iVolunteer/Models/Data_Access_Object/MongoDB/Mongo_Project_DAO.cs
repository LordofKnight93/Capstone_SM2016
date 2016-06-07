using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public static class Mongo_Project_DAO
    {
        static IMongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("iVolunteer");
        static IMongoCollection<Mongo_Project> collection = db.GetCollection<Mongo_Project>("Project");
        public static bool Add_Project(Mongo_Project project)
        {
            try
            {
                collection.InsertOneAsync(project);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}