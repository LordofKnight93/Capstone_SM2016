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
        public bool Delete_Reports(string destinationID)
        {
            try
            {
                var result = collection.DeleteMany(rp => rp.Destination.ID == destinationID);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public List<Mongo_Report> Get_GroupReport()
        {
            try
            {
                var filter = Builders<Mongo_Report>.Filter.Eq(re => re.Destination.Handler, Handler.GROUP);
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
            try
            {
                var filter = Builders<Mongo_Report>.Filter.Eq(re => re.Destination.Handler, Handler.GROUP);
                List<SDLink> reportedGroups = new List<SDLink>();
                reportedGroups = collection.Distinct(rs => rs.Destination, filter).ToList();
                return reportedGroups;
            }
            catch
            {
                throw;
            }
        }
        public List<Mongo_Report> Get_ProjectReport()
        {
            try
            {
                var filter = Builders<Mongo_Report>.Filter.Eq(re => re.Destination.Handler, Handler.PROJECT);
                var result = collection.Find(filter).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        public List<SDLink> Get_ReportedProject()
        {
            try
            {
                var filter = Builders<Mongo_Report>.Filter.Eq(re => re.Destination.Handler, Handler.PROJECT);
                List<SDLink> reportedProjects = new List<SDLink>();
                reportedProjects = collection.Distinct(rs => rs.Destination, filter).ToList();
                return reportedProjects;
            }
            catch
            {
                throw;
            }
        }
        public List<Mongo_Report> Get_UserReport()
        {
            try
            {
                var filter = Builders<Mongo_Report>.Filter.Eq(re => re.Destination.Handler, Handler.USER);
                var result = collection.Find(filter).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        public List<SDLink> Get_ReportedUser()
        {
            try
            {
                var filter = Builders<Mongo_Report>.Filter.Eq(re => re.Destination.Handler, Handler.USER);
                List<SDLink> reportedUsers = new List<SDLink>();
                reportedUsers = collection.Distinct(rs => rs.Destination, filter).ToList();
                return reportedUsers;
            }
            catch
            {
                throw;
            }
        }
    }

}