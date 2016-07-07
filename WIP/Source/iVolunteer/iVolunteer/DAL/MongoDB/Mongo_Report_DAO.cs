using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.IO;
using MongoDB.Driver.Builders;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_Report_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Report> collection = db.GetCollection<Mongo_Report>("Report");
        public bool Add_Report(Mongo_Report report)
        {
            try
            {
                collection.InsertOne(report);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_Report(string userID, string destinationID)
        {
            try
            {
                var result = collection.DeleteOne(rp => rp.Actor.ID == userID && rp.Destination.ID == destinationID);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
    }
}