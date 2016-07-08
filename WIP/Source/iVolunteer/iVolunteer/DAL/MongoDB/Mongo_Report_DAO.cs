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
using System.Threading.Tasks;

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
        public List<Mongo_Report> Get_Report()
        {
            try {
                var filter = Builders<Mongo_Report>.Filter.Eq(re => re.Destination.Handler, "Group");
                var result = collection.Find(filter).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        public List<SDLink> Get_ReportedGroup()
        {
            try {
                var filter = Builders<Mongo_Report>.Filter.Eq(re => re.Destination.Handler, "Group");
                List<SDLink> reportedGroups = new List<SDLink>();
                reportedGroups = collection.Distinct(rs => rs.Destination, filter).ToList();
                return reportedGroups;
            }
            catch
            {
                throw;
            }

            
        }
    }
}