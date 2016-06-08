using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public static class Mongo_Post_DAO
    {
        static IMongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("iVolunteer");
        static IMongoCollection<Mongo_Post> collection = db.GetCollection<Mongo_Post>("Post");
        public static bool Add_Post(Mongo_Post post)
        {
            try
            {
                collection.InsertOne(post);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}