using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.IO;
using MongoDB.Driver.Builders;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Common;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_Fund_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Fund> collection = db.GetCollection<Mongo_Fund>("Fund");

        //Add Fund
        public bool Add_Fund(Mongo_Fund fund)
        {
            try
            {
                collection.InsertOne(fund);
                return true;
            }
            catch
            {
                throw;
            }
        }

        // Delete Fund
        public bool Delete_Fund(string fundID)
        {
            try
            {
                var filter = Builders<Mongo_Fund>.Filter.Eq(fu => fu._id.ToString(), fundID);
                collection.DeleteOne(filter);
                return true;
            }
            catch
            {
                throw;
            }
        }

        //Get Fund
        public Mongo_Fund Get_Fund(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(fu => fu.Project.ID == projectID);
                return result;
            }
            catch
            {
                throw;
            }
        }

        //Get Project ID
        public string Get_ProjectID(string fundID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(fu => fu._id == ObjectId.Parse(fundID));
                return result.Project.ID;
            }
            catch
            {
                throw;
            }
        }

        //Get Team Found Donator List
        public List<TeamFoundDonator> Get_TeamFoundDonatorList(string fundID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(fu => fu._id == ObjectId.Parse(fundID));
                return result.TeamFoundDonator.ToList();
            }
            catch
            {
                throw;
            }
        }

        //Get Outside Donator List
        public List<OutsideDonator> Get_OutsideDonatorList(string fundID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(fu => fu._id == ObjectId.Parse(fundID));
                return result.OutsideDonator.ToList();
            }
            catch
            {
                throw;
            }
        }

        //Get Inside Donator List
        public List<InsideDonator> Get_InsideDonatorList(string fundID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(fu => fu._id == ObjectId.Parse(fundID));
                return result.InsideDonator.ToList();
            }
            catch
            {
                throw;
            }
        }

        //Add Team Found Donator
        public bool Add_TeamFoundDonator(string fundID, TeamFoundDonator donator)
        {
            try
            {
                var filter = Builders<Mongo_Fund>.Filter.Eq(bg => bg._id, new ObjectId(fundID));
                var update = Builders<Mongo_Fund>.Update.AddToSet<TeamFoundDonator>(fu => fu.TeamFoundDonator, donator);
                if (donator.IsReceived == true)
                {
                    update = update.Inc(fu => fu.ReceivedMoney, donator.Amount)
                                   .Inc(fu => fu.TotalMoney, donator.Amount);
                }
                else
                {
                    update = update.Inc(fu => fu.PendingMoney, donator.Amount)
                                   .Inc(fu => fu.TotalMoney, donator.Amount);
                }
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Delete Team Found Donator
        public bool Delete_TeamFoundDonator(string fundID, string teamFoundDonatorID)
        {
            try
            {
                double Amount = 0;
                bool isReceived = false;
                List<TeamFoundDonator> list = Get_TeamFoundDonatorList(fundID);
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].TeamFoundDonatorID.ToString().Equals(teamFoundDonatorID))
                    {
                        Amount = list[i].Amount;
                        isReceived = list[i].IsReceived;
                        break;
                    }
                }
                var fund_filter = Builders<Mongo_Fund>.Filter.Eq(fu => fu._id, new ObjectId(fundID));
                var donator_filter = Builders<TeamFoundDonator>.Filter.Eq(tfd => tfd.TeamFoundDonatorID, ObjectId.Parse(teamFoundDonatorID));
                var update = Builders<Mongo_Fund>.Update.PullFilter(fu => fu.TeamFoundDonator, donator_filter)
                                                 .Inc(fu => fu.PendingMoney, -Amount)
                                                 .Inc(fu => fu.TotalMoney, -Amount);
                var result = collection.UpdateOne(fund_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Set is receice money
        public bool Update_IsReceiveMoney(string fundID, string teamFoundDonatorID)
        {
            try
            {
                double Amount = 0;
                List<TeamFoundDonator> list = Get_TeamFoundDonatorList(fundID);
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].TeamFoundDonatorID.ToString().Equals(teamFoundDonatorID))
                    {
                        Amount = list[i].Amount;
                        break;
                    }
                }
                var filter = Builders<Mongo_Fund>.Filter.And(
                                                    Builders<Mongo_Fund>.Filter.Eq(fu => fu._id, ObjectId.Parse(fundID)),
                                                    Builders<Mongo_Fund>.Filter.ElemMatch(fu => fu.TeamFoundDonator, tfd => tfd.TeamFoundDonatorID == ObjectId.Parse(teamFoundDonatorID)));
                var update = Builders<Mongo_Fund>.Update.Set(bg => bg.TeamFoundDonator[-1].IsReceived, true)
                                                         .Inc(fu => fu.PendingMoney, -Amount)
                                                         .Inc(fu => fu.ReceivedMoney, Amount);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        public double Get_ReceivedMoney(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(fu => fu.Project.ID == projectID);
                return result.ReceivedMoney;
            }
            catch
            {
                throw;
            }
        }

    }
}