using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public static class Mongo_User_DAO
    {
        static IMongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("iVolunteer");
        static IMongoCollection<Mongo_User> collection = db.GetCollection<Mongo_User>("User");
        public static bool Add_User(Mongo_User user)
        {
            try
            {
                collection.InsertOne(user);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}