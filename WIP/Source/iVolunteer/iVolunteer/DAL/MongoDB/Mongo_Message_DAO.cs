using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using iVolunteer.Models.MongoDB.CollectionClass;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_Message_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Message> collection = db.GetCollection<Mongo_Message>("Message");
        public bool Add_Message(Mongo_Message message)
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