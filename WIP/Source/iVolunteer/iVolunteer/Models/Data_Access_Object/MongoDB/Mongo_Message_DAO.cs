﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public class Mongo_Message_DAO
    {
        static IMongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("iVolunteer");
        static IMongoCollection<Mongo_Message> collection = db.GetCollection<Mongo_Message>("Message");
        public static bool Add_Message(Mongo_Message message)
        {
            try
            {
                collection.InsertOne(message);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}