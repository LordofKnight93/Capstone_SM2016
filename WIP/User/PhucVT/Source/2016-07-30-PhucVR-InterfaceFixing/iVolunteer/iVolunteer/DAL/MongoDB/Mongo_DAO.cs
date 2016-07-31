using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_DAO
    {
        public static IMongoClient client = new MongoClient();
        public static IMongoDatabase db = client.GetDatabase("iVolunteer");
    }
}