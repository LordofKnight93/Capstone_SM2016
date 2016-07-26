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
        /// get personal infofmation of an user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public PersonalInformation Get_PersonalInformation(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u.PersonalInformation.UserID == userID);
                return result.PersonalInformation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get account infofmation of an user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public AccountInformation Get_AccountInformation(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                return result.AccountInformation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get SDlink of a user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public SDLink Get_SDLink(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                return new SDLink(result.AccountInformation);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get list accoutn information from list ID
        /// </summary>
        /// <param name="listID"></param>
        /// <returns></returns>
        public List<AccountInformation> Get_AccountsInformation(List<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(gr => gr.AccountInformation.UserID, listID);
                var result = collection.Find(filter).ToList().Select(gr => gr.AccountInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        public List<AccountInformation> Search_AccountsInformationForProject(List<string> listID, string name)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(gr => gr.AccountInformation.UserID, listID);
                var result = collection.Find(filter).ToList().Select(gr => gr.AccountInformation).Where(u => u.DisplayName.ToLower().Contains(name.ToLower())).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// inscrease groupCount by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Join_Group(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.GroupCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decrease groupCount by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Out_Group(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.GroupCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// inscrease ProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Join_Project(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.ProjectCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// decrease ProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Out_Project(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.ProjectCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// inscrease FriendCount by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Add_Friend(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.FriendCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// decrease FriendCount by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_Friend(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.FriendCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// confirmed account
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Confirmed(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsConfirmed, Status.IS_NOT_CONFIRMED);
                var update = Builders<Mongo_User>.Update.Set(u => u.AccountInformation.IsConfirmed, Status.IS_CONFIRMED);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// udpate user personal information, only phone and address
        /// </summary>
        /// <param name="userID"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool Update_PersonalInformation(string userID, string newPhone, string newAddress)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.PersonalInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE);
                var update = Builders<Mongo_User>.Update.Set(u => u.PersonalInformation.Phone, newPhone)
                                                        .Set(u => u.PersonalInformation.Address, newAddress)
                                                        .Set(u => u.AccountInformation.Address, newAddress);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// search active and confirmed user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="option">true to include deactive user, false to find active user only</param>
        /// <returns></returns>
        public List<AccountInformation> User_Search(string name, bool allStatus)
        {
            try
            {
                var preResult = collection.AsQueryable().Where(u => u.AccountInformation.DisplayName.ToLower().Contains(name.ToLower()));
                if(allStatus == false)
                {
                    preResult = preResult.Where(u => u.AccountInformation.IsActivate == Status.IS_ACTIVATE);
                }
                var result = preResult.Select(u => u.AccountInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get users who is deactivated
        /// </summary>
        /// <returns></returns>
        public List<SDLink> Get_Banned_Users()
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(gr => gr.AccountInformation.IsActivate, Status.IS_BANNED);
                var result = collection.Find(filter).ToList();
                List<SDLink> BannedUsers = new List<SDLink>();
                foreach (var item in result)
                {
                    BannedUsers.Add(Get_SDLink(item.AccountInformation.UserID));
                }
                return BannedUsers;
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
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
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
        /// Get friend list ***NEED FOR CHAT FUNCTION
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<SDLink> Get_FriendList(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                return result.FriendList;
            }
            catch
            {
                throw;
            }
        }
        public bool Add_Friend_To_List(string userID, SDLink user)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.FriendList, user)
                                                        .Inc(u => u.AccountInformation.FriendCount, 1);
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