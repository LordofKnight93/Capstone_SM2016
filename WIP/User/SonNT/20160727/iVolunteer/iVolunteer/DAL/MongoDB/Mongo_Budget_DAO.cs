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
    public class Mongo_Budget_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Budget> collection = db.GetCollection<Mongo_Budget>("Budget");
        /// <summary>
        /// Add new budget to mongoDB
        /// </summary>
        /// <param name="budget">Mongo_Post instance</param>
        /// <returns>true if success</returns>
        public bool Add_Budget(Mongo_Budget budget)
        {
            try
            {
                collection.InsertOne(budget);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Delete a budget record in mongoDB
        /// </summary>
        /// <param name="budget">Mongo_Post instance</param>
        /// <returns>true if success</returns>
        public bool Delete_Budget(string budgetID)
        {
            try
            {
                var filter = Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID);
                collection.DeleteOne(filter);
                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get a post by postID
        /// </summary>
        /// <param name="postID"></param>
        /// <returns>Mongo_Post instance</returns>
        public BudgetRecordInformation Get_BudgetRecord_By_BudgetID(string budgetID)
        {
            try
            {
                //var filter = Builders<Mongo_Budget>.Filter.Eq(bg => bg.Project.ID, projectID);
                //var result = collection.AsQueryable().FirstOrDefault(bg => bg.BudgetRecordInformation.Project.ID == projectID);
                //return result.BudgetRecordInformation;
                var filter = Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID);
                var result = collection.Find(filter).FirstOrDefault();
                return result.BudgetRecordInformation;
            }
            catch
            {
                throw;
            }
        }

        //Get List Budget Record
        public List<BudgetRecordInformation> Get_BudgetAllRecord(string projectID)
        {
            try
            {
                 var result = collection.AsQueryable().Where(bg => bg.BudgetRecordInformation.Project.ID == projectID)
                    .Select(bg => bg.BudgetRecordInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        //Get Budget Item
        public double Get_BudgetItem(string budgetID, string itemContent)
        {
            try
            {
                var filter = Builders<Mongo_Budget>.Filter.And(
                                                    Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID),
                                                    Builders<Mongo_Budget>.Filter.ElemMatch(bg => bg.Item, it => it.Content == itemContent));
                var result = collection.Find(filter).FirstOrDefault();
                return result.Item[-1].Cost;
            }
            catch
            {
                throw;
            }
        }

        //Get list budget item
        public List<BudgetItem> Get_BudgetItemList(string budgetID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(bg => bg.BudgetRecordInformation.BudgetRecordID == budgetID);
                return result.Item.ToList();
            }
            catch
            {
                throw;
            }
        }

        //Add Budget Item
        public bool Add_BudgetItem(string budgetID, BudgetItem item)
        {
            try
            {
                var filter = Builders<Mongo_Budget>.Filter.Eq(bg => bg._id, new ObjectId(budgetID));
                var update = Builders<Mongo_Budget>.Update.AddToSet<BudgetItem>(bg => bg.Item, item).Inc(bg => bg.BudgetRecordInformation.Total, item.Cost);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Delete budget item
        public bool Delete_BudgetItem(string budgetID, string itemName)
        {
            try
            {
                double cost = 0;
                List<BudgetItem> bitem = Get_BudgetItemList(budgetID);
                for (int i = 0; i < bitem.Count(); i++)
                {
                    if (bitem[i].Content.Equals(itemName))
                    {
                        cost = bitem[i].Cost;
                        break;
                    }
                }
                var budget_filter = Builders<Mongo_Budget>.Filter.Eq(bg => bg._id, new ObjectId(budgetID));
                var item_filter = Builders<BudgetItem>.Filter.Eq(it => it.Content, itemName);
                var update = Builders<Mongo_Budget>.Update.PullFilter(bg => bg.Item, item_filter).Inc(bg => bg.BudgetRecordInformation.Total, -cost);
                var result = collection.UpdateOne(budget_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Update Budget Record Infor
        public bool Update_BudgetRecord(string budgetID, BudgetRecordInformation newInfo)
        {
            try
            {
                var filter = Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID);
                var update = Builders<Mongo_Budget>.Update.Set(bg => bg.BudgetRecordInformation.Name, newInfo.Name);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Update Budget Item Infor
        public bool Update_BudgetItem(string budgetID, string itemName, BudgetItem item)
        {
            try
            {
                double cost = 0;
                List<BudgetItem> bitem = Get_BudgetItemList(budgetID);
                for (int i = 0; i< bitem.Count(); i++)
                {
                    if (bitem[i].Content.Equals(itemName))
                    {
                        cost = bitem[i].Cost;
                        break;
                    }
                }
                var filter = Builders<Mongo_Budget>.Filter.And(
                                                    Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID),
                                                    Builders<Mongo_Budget>.Filter.ElemMatch(bg => bg.Item, it => it.Content == itemName));
                var update =Builders<Mongo_Budget>.Update.Set(bg => bg.Item[-1].Content, item.Content)
                                                         .Set(bg => bg.Item[-1].UnitPrice, item.UnitPrice)
                                                         .Set(bg => bg.Item[-1].Quatity, item.Quatity)
                                                         .Set(bg => bg.Item[-1].Unit, item.Unit)
                                                         .Set(bg => bg.Item[-1].Cost, item.Cost)
                                                         .Inc(bg => bg.BudgetRecordInformation.Total, item.Cost - cost);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Update Budget Item Content
        public bool Update_BudgetItemContent(string budgetID, string itemName, string newContent)
        {
            try
            {
                var filter = Builders<Mongo_Budget>.Filter.And(
                                                    Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID),
                                                    Builders<Mongo_Budget>.Filter.ElemMatch(bg => bg.Item, it => it.Content == itemName));
                var update = Builders<Mongo_Budget>.Update.Set(bg => bg.Item[-1].Content, newContent);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        //Update Budget Item Unit Price
        public bool Update_BudgetItemUnitPrice(string budgetID, string itemName, double unitPrice)
        {
            try
            {
                double cost = 0;
                int quatity = 0;
                List<BudgetItem> bitem = Get_BudgetItemList(budgetID);
                for (int i = 0; i < bitem.Count(); i++)
                {
                    if (bitem[i].Content.Equals(itemName))
                    {
                        cost = bitem[i].Cost;
                        quatity = bitem[i].Quatity;
                        break;
                    }
                }
                var filter = Builders<Mongo_Budget>.Filter.And(
                                                    Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID),
                                                    Builders<Mongo_Budget>.Filter.ElemMatch(bg => bg.Item, it => it.Content == itemName));
                var update = Builders<Mongo_Budget>.Update.Set(bg => bg.Item[-1].UnitPrice, unitPrice)
                                                         .Set(bg => bg.Item[-1].Cost, unitPrice * quatity)
                                                         .Inc(bg => bg.BudgetRecordInformation.Total, unitPrice * quatity - cost);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        //Update Budget Item Quatity
        public bool Update_BudgetItemQuatity(string budgetID, string itemName, int quatity)
        {
            try
            {
                double cost = 0;
                double unitPrice = 0;
                List<BudgetItem> bitem = Get_BudgetItemList(budgetID);
                for (int i = 0; i < bitem.Count(); i++)
                {
                    if (bitem[i].Content.Equals(itemName))
                    {
                        cost = bitem[i].Cost;
                        unitPrice = bitem[i].UnitPrice;
                        break;
                    }
                }
                var filter = Builders<Mongo_Budget>.Filter.And(
                                                    Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID),
                                                    Builders<Mongo_Budget>.Filter.ElemMatch(bg => bg.Item, it => it.Content == itemName));
                var update = Builders<Mongo_Budget>.Update.Set(bg => bg.Item[-1].Quatity, quatity)
                                                         .Set(bg => bg.Item[-1].Cost, quatity * unitPrice)
                                                         .Inc(bg => bg.BudgetRecordInformation.Total, quatity * unitPrice - cost);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        //Update Budget Item Unit
        public bool Update_BudgetItemUnit(string budgetID, string itemName, string unit)
        {
            try
            {
                var filter = Builders<Mongo_Budget>.Filter.And(
                                                    Builders<Mongo_Budget>.Filter.Eq(bg => bg.BudgetRecordInformation.BudgetRecordID, budgetID),
                                                    Builders<Mongo_Budget>.Filter.ElemMatch(bg => bg.Item, it => it.Content == itemName));
                var update = Builders<Mongo_Budget>.Update.Set(bg => bg.Item[-1].Unit, unit);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        //Get Comment
        /// <summary>
        /// get a numbers of comment or a post
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Comment> Get_Comments(string id, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(bg => bg._id.ToString() == id);
                return result.Comment.Skip(skip).Take(number).ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add comment to post and inc cmt count by 1
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="cmt"></param>
        /// <returns></returns>
        public bool Add_Comment(string id, Comment comment)
        {
            try
            {
                var filter = Builders<Mongo_Budget>.Filter.Eq(p => p._id, new ObjectId(id));
                var update = Builders<Mongo_Budget>.Update.AddToSet<Comment>(bg => bg.Comment, comment);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete a comment and decrease cmt count by 1
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="cmtID"></param>
        /// <returns></returns>
        public bool Delete_Comment(string budgetID, string commentID)
        {
            try
            {
                var post_filter = Builders<Mongo_Budget>.Filter.Eq(p => p._id, new ObjectId(budgetID));
                var cmt_filter = Builders<Comment>.Filter.Eq(cmt => cmt.CommentID, commentID);
                var update = Builders<Mongo_Budget>.Update.PullFilter(p => p.Comment, cmt_filter);
                var result = collection.UpdateOne(post_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        public double Get_TotalBudget(string projectID)
        {
            try
            {
                List<BudgetRecordInformation> listBudget = Get_BudgetAllRecord(projectID);
                double totalBudget = 0;
                for( int i = 0; i<listBudget.Count(); i++)
                {
                    totalBudget += listBudget[i].Total;
                }
                return totalBudget;
            }
            catch
            {
                throw;
            }
        }
    }
}