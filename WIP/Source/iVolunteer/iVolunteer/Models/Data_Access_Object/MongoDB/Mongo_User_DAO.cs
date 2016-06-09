using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using MongoDB.Bson;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public static class Mongo_User_DAO
    {
        static IMongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("iVolunteer");
        static IMongoCollection<Mongo_User> collection = db.GetCollection<Mongo_User>("User");
        /// <summary>
        /// add new user to mongoDB
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool Add_User(Mongo_User user)
        {
            try
            {
                collection.InsertOne(user);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public static bool Set_Activation_Status(string userID, bool status)
        {
            try
            {
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}