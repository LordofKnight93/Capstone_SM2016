using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
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
        public List<MessageItem> Get_Recent_Messages(string messageID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(m => m.MessageID == messageID);
                return result.ItemList;
            }
            catch
            {
                throw;
            }
        }
        public DateTime Add_Message_Item(string messageID, string sender, string content)
        {
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink senderSDLink = userDAO.Get_SDLink(sender);
                //create message item
                MessageItem mssItem = new MessageItem(senderSDLink, content);

                var filter = Builders<Mongo_Message>.Filter.Eq(mss => mss.MessageID, messageID);
                var update = Builders<Mongo_Message>.Update.AddToSet(u => u.ItemList, mssItem);
                var result = collection.UpdateOne(filter, update);
                return mssItem.DateSend;
            }
            catch
            {
                throw;
            }
        }
    }
}