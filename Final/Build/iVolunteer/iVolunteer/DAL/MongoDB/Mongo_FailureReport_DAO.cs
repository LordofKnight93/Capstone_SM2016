using iVolunteer.Models.MongoDB.CollectionClass;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_FailureReport_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_FailureReport> collection = db.GetCollection<Mongo_FailureReport>("FailureReport");

        public bool Add_FailureReport(Mongo_FailureReport info)
        {
            try
            {
                collection.InsertOne(info);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool Delete_FailureReport(string failureID)
        {
            try
            {
                var filter = Builders<Mongo_FailureReport>.Filter.Eq(fr => fr._id, ObjectId.Parse(failureID));
                collection.DeleteOne(filter);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public List<Mongo_FailureReport> Get_Advise()
        {
            try
            {
                var result = collection.AsQueryable().Where(fr => fr.Type == 1).Select(fr => fr).ToList();
                return result.OrderByDescending(fr => fr.SendDate).ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<Mongo_FailureReport> Get_FailureReport()
        {
            try
            {
                var result = collection.AsQueryable().Where(fr => fr.Type == 2).Select(fr => fr).ToList();
                return result.OrderByDescending(fr => fr.SendDate).ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}