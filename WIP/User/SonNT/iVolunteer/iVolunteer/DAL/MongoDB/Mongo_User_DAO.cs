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
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_User_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_User> collection = db.GetCollection<Mongo_User>("User");
        /// <summary>
        /// add new user to mongoDB
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add_User(Mongo_User user)
        {
            try
            {
                collection.InsertOne(user);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get information of a user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserInformation Get_UserInformation(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u._id == new ObjectId(userID));
                return result.UserInformation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get all user information, for admin only
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<UserInformation> Get_All_UserInformations(int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().Select(u => u.UserInformation).Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get account information
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public AccountInformation Get_AccountInformation(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(us => us._id == new ObjectId(userID));
                return result.AccountInformation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get all account information, admin use
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<AccountInformation> Get_All_AccountInformation(int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().Select(u => u.AccountInformation)
                                                     .Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get user's joined group
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<SDLink> Get_JoinedGroups(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u._id == new ObjectId(userID));
                return result.JoinedGroup;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get user's friends
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<Member> Get_FriendList(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u._id == new ObjectId(userID));
                return result.FriendList;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get user's activity history 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActivityInformation Get_ActivityHistory(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u._id == new ObjectId(userID));
                return result.ActivityHistory;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get user's current projects
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<SDLink> Get_CurrentProjects(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u._id == new ObjectId(userID));
                return result.CurrentProjects;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a number of user's notification
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Notification> Get_Notifications(string userID, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u._id == new ObjectId(userID));
                return result.NotificationList.Skip(skip).Take(number).ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// set activation statuc for an account in mongDB
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <returns>true if success</returns>
        public bool Set_Activation_Status(string userID, bool status)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc._id, new ObjectId(userID));
                var update = Builders<Mongo_User>.Update.Set(acc => acc.AccountInformation.IsActivate, status);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set user display name
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public bool Set_DisplayName(string userID, string displayName)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc._id, new ObjectId(userID));
                var update = Builders<Mongo_User>.Update.Set(acc => acc.AccountInformation.DisplayName, displayName);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set user password
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Set_Password(string userID, string password)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc._id, new ObjectId(userID));
                var update = Builders<Mongo_User>.Update.Set(acc => acc.AccountInformation.Password, password);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Update user information 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public bool Update_UserInformation(string userID, UserInformation userInfo)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc._id, new ObjectId(userID));
                var update = Builders<Mongo_User>.Update.Set(acc => acc.UserInformation, userInfo);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get userSD
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public SDLink Get_SDLink(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u._id == new ObjectId(userID));
                return new SDLink(result.AccountInformation);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add new notification to user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public bool Add_Notification(string userID, Notification notify)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u._id, new ObjectId(userID));
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.NotificationList, notify);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
    }
}