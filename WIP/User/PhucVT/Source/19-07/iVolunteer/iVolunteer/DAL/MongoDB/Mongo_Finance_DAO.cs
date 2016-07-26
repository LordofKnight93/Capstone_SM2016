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
    public class Mongo_Finance_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Finance> collection = db.GetCollection<Mongo_Finance>("Finance");
        //Add Fund
        public bool Add_Finance(Mongo_Finance finance)
        {
            try
            {
                collection.InsertOne(finance);
                return true;
            }
            catch
            {
                throw;
            }
        }

        // Delete Fund
        public bool Delete_Finance(string financeID)
        {
            try
            {
                var filter = Builders<Mongo_Finance>.Filter.Eq(fn => fn._id.ToString(), financeID);
                collection.DeleteOne(filter);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public Mongo_Finance Get_Finance(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(fn => fn.Project.ID == projectID);
                return result;
            }
            catch
            {
                throw;
            }
        }

        //Get list finance item
        public List<FinanceItem> Get_FinanceItemList(string financeID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(fn => fn._id == ObjectId.Parse(financeID));
                return result.FinanceItem.ToList();
            }
            catch
            {
                throw;
            }
        }

        //Add Budget Item
        public bool Add_FinanceItem(string financeID, FinanceItem item)
        {
            try
            {
                var filter = Builders<Mongo_Finance>.Filter.Eq(fn => fn._id, new ObjectId(financeID));
                var update = Builders<Mongo_Finance>.Update.AddToSet<FinanceItem>(fn => fn.FinanceItem, item).Inc(fn => fn.Total, item.Amount);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Delete finance item
        public bool Delete_FinanceItem(string financeID, string financeItemID)
        {
            try
            {
                double amount = 0;
                List<FinanceItem> fnitem = Get_FinanceItemList(financeID);
                for (int i = 0; i < fnitem.Count(); i++)
                {
                    if (fnitem[i].FinanceItemID.ToString().Equals(financeItemID))
                    {
                        amount = fnitem[i].Amount;
                        break;
                    }
                }
                var budget_filter = Builders<Mongo_Finance>.Filter.Eq(fn => fn._id, new ObjectId(financeID));
                var item_filter = Builders<FinanceItem>.Filter.Eq(it => it.FinanceItemID, ObjectId.Parse(financeItemID));
                var update = Builders<Mongo_Finance>.Update.PullFilter(fn => fn.FinanceItem, item_filter).Inc(fn => fn.Total, -amount);
                var result = collection.UpdateOne(budget_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Update finance item
        public bool Update_FinanceItem(string financeID, string financeItemID, FinanceItem item)
        {
            try
            {
                double amount = 0;
                List<FinanceItem> fnitem = Get_FinanceItemList(financeID);
                for (int i = 0; i < fnitem.Count(); i++)
                {
                    if (fnitem[i].FinanceItemID.ToString().Equals(financeItemID))
                    {
                        amount = fnitem[i].Amount;
                        break;
                    }
                }
                var filter = Builders<Mongo_Finance>.Filter.And(
                                                    Builders<Mongo_Finance>.Filter.Eq(fn => fn._id.ToString(), financeID),
                                                    Builders<Mongo_Finance>.Filter.ElemMatch(fn => fn.FinanceItem, it => it.FinanceItemID.ToString() == financeItemID));
                var update = Builders<Mongo_Finance>.Update.Set(fn => fn.FinanceItem[-1].Term, item.Term)
                                                         .Set(fn => fn.FinanceItem[-1].Amount, item.Amount)
                                                         .Set(fn => fn.FinanceItem[-1].Date, item.Date)
                                                         .Inc(fn => fn.Total, item.Amount - amount);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Update finance item term
        public bool Update_FinanceItemTerm(string financeID, string financeItemID, FinanceItem item)
        {
            try
            {
                var filter = Builders<Mongo_Finance>.Filter.And(
                                                    Builders<Mongo_Finance>.Filter.Eq(fn => fn._id.ToString(), financeID),
                                                    Builders<Mongo_Finance>.Filter.ElemMatch(fn => fn.FinanceItem, it => it.FinanceItemID.ToString() == financeItemID));
                var update = Builders<Mongo_Finance>.Update.Set(fn => fn.FinanceItem[-1].Term, item.Term);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Update finance amount
        public bool Update_FinanceItemAmount(string financeID, string financeItemID, FinanceItem item)
        {
            try
            {
                double amount = 0;
                List<FinanceItem> fnitem = Get_FinanceItemList(financeID);
                for (int i = 0; i < fnitem.Count(); i++)
                {
                    if (fnitem[i].FinanceItemID.ToString().Equals(financeItemID))
                    {
                        amount = fnitem[i].Amount;
                        break;
                    }
                }
                var filter = Builders<Mongo_Finance>.Filter.And(
                                                    Builders<Mongo_Finance>.Filter.Eq(fn => fn._id.ToString(), financeID),
                                                    Builders<Mongo_Finance>.Filter.ElemMatch(fn => fn.FinanceItem, it => it.FinanceItemID.ToString() == financeItemID));
                var update = Builders<Mongo_Finance>.Update.Set(fn => fn.FinanceItem[-1].Amount, item.Amount)
                                                           .Inc(fn => fn.Total, item.Amount - amount);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Update finance item date
        public bool Update_FinanceItemDate(string financeID, string financeItemID, FinanceItem item)
        {
            try
            {
                var filter = Builders<Mongo_Finance>.Filter.And(
                                                    Builders<Mongo_Finance>.Filter.Eq(fn => fn._id.ToString(), financeID),
                                                    Builders<Mongo_Finance>.Filter.ElemMatch(fn => fn.FinanceItem, it => it.FinanceItemID.ToString() == financeItemID));
                var update = Builders<Mongo_Finance>.Update.Set(fn => fn.FinanceItem[-1].Date, item.Date);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Get Project ID
        public string Get_ProjectID(string financeID)
        {
            var result = collection.AsQueryable().FirstOrDefault(fn => fn._id == ObjectId.Parse(financeID));
            return result.Project.ID;
        }
    }
}