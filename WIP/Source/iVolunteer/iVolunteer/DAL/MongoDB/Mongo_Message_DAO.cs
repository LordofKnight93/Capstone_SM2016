using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.DAL.SQL;

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
        public DateTime Add_Message_Item(string messageID, string sender, string content, string friendID)
        {
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink senderSDLink = userDAO.Get_SDLink(sender);
                //create message item
                MessageItem mssItem = new MessageItem(senderSDLink, content);

                var filter = Builders<Mongo_Message>.Filter.Eq(mss => mss.MessageID, messageID);
                var update = Builders<Mongo_Message>.Update.AddToSet(u => u.ItemList, mssItem).Set(ms => ms.DateLastActivity, DateTime.Now).Set(ms => ms.UnreadItem.UnreadUser, friendID).Set(ms => ms.UnreadItem.UnreadMessage, 0);
                var result = collection.UpdateOne(filter, update);
                return mssItem.DateSend;
            }
            catch
            {
                throw;
            }
        }
        public DateTime Add_Message_Item_Unread(string messageID, string sender, string content, string unreadUser)
        {
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink senderSDLink = userDAO.Get_SDLink(sender);
                //create message item
                MessageItem mssItem = new MessageItem(senderSDLink, content);

                var filter = Builders<Mongo_Message>.Filter.Eq(mss => mss.MessageID, messageID);
                //if friend off line (delete own unread message, create freind unread message)
                UnreadItem unread = collection.AsQueryable().FirstOrDefault(ms => ms.MessageID == messageID).UnreadItem;
                //If current unreaduser is User => change unread to friend
                if (unread.UnreadUser == sender)
                {
                    var update = Builders<Mongo_Message>.Update.AddToSet(u => u.ItemList, mssItem).Set(ms => ms.DateLastActivity, DateTime.Now).Set(ms => ms.UnreadItem.UnreadUser, unreadUser).Set(ms => ms.UnreadItem.UnreadMessage, 1);
                    collection.UpdateOne(filter, update);
                }
                else
                {
                    var update = Builders<Mongo_Message>.Update.AddToSet(u => u.ItemList, mssItem).Set(ms => ms.DateLastActivity, DateTime.Now).Set(ms => ms.UnreadItem.UnreadUser, unreadUser).Inc(ms => ms.UnreadItem.UnreadMessage, 1);
                    collection.UpdateOne(filter, update);
                }
                //Else: increase more unread item message

                return mssItem.DateSend;
            }
            catch
            {
                throw;
            }
        }
        public bool Set_LastActivities(string messageID)
        {
            try
            {
                var filter = Builders<Mongo_Message>.Filter.Eq(ms => ms.MessageID, messageID);
                var update = Builders<Mongo_Message>.Update.Set(ms => ms.DateLastActivity, DateTime.Now);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public bool Set_UnreadMess(string messageID, int no)
        {
            try
            {
                var filter = Builders<Mongo_Message>.Filter.Eq(ms => ms.MessageID, messageID);
                var update = Builders<Mongo_Message>.Update.Set(ms => ms.UnreadItem.UnreadMessage, no);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        public bool Set_UnreadUser(string messageID, string userID)
        {
            try
            {
                var filter = Builders<Mongo_Message>.Filter.Eq(ms => ms.MessageID, messageID);
                var update = Builders<Mongo_Message>.Update.Set(ms => ms.UnreadItem.UnreadUser, userID);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public UnreadItem Get_UnreadMss(string userID, string friendID)
        {
            try
            {
                SQL_Message_DAO DAO = new SQL_Message_DAO();
                string messageID = DAO.Get_MessageID(userID, friendID);
                if (messageID == null) return null;
                var result = collection.AsQueryable().FirstOrDefault(ms => ms.MessageID == messageID);
                return result.UnreadItem;
            }
            catch
            {
                throw;
            }
        }

        public string Get_UnreadUser(string userID, string friendID)
        {
            try
            {
                SQL_Message_DAO DAO = new SQL_Message_DAO();
                string messageID = DAO.Get_MessageID(userID, friendID);
                if (messageID == null) return null;
                var result = collection.AsQueryable().FirstOrDefault(ms => ms.MessageID == messageID);
                return result.UnreadItem.UnreadUser;
            }
            catch
            {
                throw;
            }
        }

        public bool Get_UnreadMssNoti(string userID)
        {
            try
            {
                SQL_Message_DAO DAO = new SQL_Message_DAO();
                List<string> messageIDs = DAO.Get_MessageIDs(userID);
                if (messageIDs.Count() == 0) return false;
                bool haveUnread = false;
                for (int i=0; i< messageIDs.Count(); i++)
                {
                    var result = collection.AsQueryable().FirstOrDefault(ms => ms.MessageID == messageIDs[i]).UnreadItem;
                    if(result.UnreadMessage > 0 && result.UnreadUser.Equals(userID))
                    {
                        haveUnread = true;
                        break;
                    }
                    else
                    {
                        haveUnread = false;
                    }
                }
                return haveUnread;
            }
            catch
            {
                throw;
            }
        }

        public List<SDLink> Get_Recent_Chat_Users(List<string> messageIDs, string userID)
        {
            try
            {
                List<Mongo_Message> messages = new List<Mongo_Message>();
                var filter = Builders<Mongo_Message>.Filter.In(p => p.MessageID, messageIDs);
                messages = collection.Find(filter).SortByDescending(p => p.DateLastActivity).ToList();
                List<SDLink> users = new List<SDLink>();
                foreach (var message in messages)
                {
                    if (message.Senders[0].ID != userID)
                        users.Add(message.Senders[0]);
                    else users.Add(message.Senders[1]);
                }
                return users;
            }
            catch
            {
                throw;
            }
        }
    }
}