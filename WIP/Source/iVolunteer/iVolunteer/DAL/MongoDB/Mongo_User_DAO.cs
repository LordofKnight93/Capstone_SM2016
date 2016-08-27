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
        /// Search Acount Information For Project
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<AccountInformation> Search_AccountsInformationForProject(List<string> listID, string name)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(gr => gr.AccountInformation.UserID, listID);
                var result = collection.Find(filter).ToList().Select(gr => gr.AccountInformation).Where(u => u.DisplayName.ToLower().Contains(name.ToLower())).ToList();
                if (result.Count != 0)
                {
                    return result;
                }
                else
                {
                    return null;
                }
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
        /// <summary>
        /// inscrease groupCount by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Join_Group(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
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
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
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
        /// inscrease JoinedProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Join_Project(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.JoinedProjectCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// inscrease SponsoredProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Sponsor_Project(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.SponsoredProjectCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// inscrease JOrganizedProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Organize_Project(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.OrganizedProjectCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// decrease JoinedProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Out_Project(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.JoinedProjectCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decrease SponsoredProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Not_Sponsor_Project(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.SponsoredProjectCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decrease JOrganizedProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Not_Organize_Project(string userID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.OrganizedProjectCount, -1);
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
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
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
        //public bool Delete_Friend(string userID)
        //{
        //    try
        //    {
        //        var filter = Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.UserID, userID);
        //        var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.FriendCount, -1);
        //        var result = collection.UpdateOne(filter, update);
        //        return result.IsAcknowledged;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        // <summary>
        /// decrease FriendCount by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_Friend(string userID, string friendID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var friend_filter = Builders<SDLink>.Filter.Eq(s => s.ID, friendID);
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.FriendList, friend_filter)
                                                        .Inc(u => u.AccountInformation.FriendCount, -1); ;
                var result = collection.UpdateOne(user_filter, update);
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
        /// udpate user personal information, only phone and address, interest, skills, experience, email
        /// </summary>
        /// <param name="userID"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool Update_PersonalInformation(string userID, PersonalInformation newInfo)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(u => u.PersonalInformation.UserID, userID)
                           & Builders<Mongo_User>.Filter.Eq(u => u.AccountInformation.IsActivate, Status.IS_ACTIVATE);
                var update = Builders<Mongo_User>.Update.Set(u => u.PersonalInformation.Phone, newInfo.Phone)
                                                        .Set(u => u.PersonalInformation.Address, newInfo.Address)
                                                        .Set(u => u.PersonalInformation.Email, newInfo.Email)
                                                        .Set(u => u.PersonalInformation.Skills, newInfo.Skills)
                                                        .Set(u => u.PersonalInformation.Experience, newInfo.Experience)
                                                        .Set(u => u.PersonalInformation.Interest, newInfo.Interest);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// search active user, for user usage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<AccountInformation> Active_User_Search(string name, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().Where(ac => ac.AccountInformation.DisplayName.ToLower().Contains(name.ToLower())
                                                               && ac.AccountInformation.IsActivate == Status.IS_ACTIVATE)
                                                     .Skip(skip).Take(number)
                                                     .Select(ac => ac.AccountInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// search all user, for admin usage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<AccountInformation> User_Search(string name, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().Where(ac => ac.AccountInformation.DisplayName.ToLower().Contains(name.ToLower()))
                                                     .Skip(skip).Take(number)
                                                     .Select(ac => ac.AccountInformation).ToList();
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
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.NotificationList, notify);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set a notification is seen
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="notifyID"></param>
        /// <returns></returns>
        public bool Set_Notification_IsSeen(string userID, string notifyID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Where(u => u.AccountInformation.UserID == userID && u.NotificationList.Any(no => no.NotifyID == notifyID));
                var update = Builders<Mongo_User>.Update.Set(u => u.NotificationList.ElementAt(-1).IsSeen, Status.IS_SEEN);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
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
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                return result.NotificationList.OrderByDescending(p => p.DateNotice).Skip(skip).Take(number).ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<Notification> Get_Old_FriendAcceptedNotification(string userID)
        {
            try
            {
                var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.IsSeen == true && item.Type == Notify.FRIEND_REQUEST_ACCEPTED);
                return result.OrderByDescending(p => p.DateNotice).ToList();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Delete Notificaiton
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        /// <returns></returns>
        public bool Delete_Notification(string userID, string notifyID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var notify_filter = Builders<Notification>.Filter.Eq(nt => nt.NotifyID, notifyID);
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.NotificationList, notify_filter);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Count number of Notification for UserID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int Count_Notifications(string userID)
        {
            try
            {
                var num = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.IsSeen == false && item.Type != Notify.FRIEND_REQUEST_ACCEPTED).Count();
                //var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                //int count = result.NotificationList.Count(nt => nt.IsSeen == false);
                return num;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Count number of Friend Accepted notification
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int Count_FriendAccepted(string userID)
        {
            try
            {
                var num = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.IsSeen == false && item.Type == Notify.FRIEND_REQUEST_ACCEPTED).Count();
                //var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                //int count = result.NotificationList.Count(nt => nt.IsSeen == false);
                return num;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get friend accepted notification
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Notification> Get_FriendAcceptedNotification(string userID, int skip, int number)
        {
            try
            {
                var result1 = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.IsSeen == false && item.Type == Notify.FRIEND_REQUEST_ACCEPTED);
                return result1.ToList();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get all unseen notificaiton of user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Notification> Get_UnSeen_Notifications(string userID, int skip, int number)
        {
            try
            {
                var result1 = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.IsSeen == false && item.Type != Notify.FRIEND_REQUEST_ACCEPTED);
                return result1.OrderByDescending(n => n.DateNotice).ToList();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <returns></returns>
        public bool Is_Post_Has_Unseen_Notify(string userID, string postID)
        {
            try
            {
                var count = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.Target.ID == postID && item.IsSeen == false).Count();
                return count != 0;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get notify about post commented
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <returns></returns>
        public string Get_PostCmted_NotifyID(string userID, string postID, int type)
        {
            try
            {
                var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.Target.ID == postID && item.Type == type && item.IsSeen == false).ToList();
                return result.ElementAt(0).NotifyID;

            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get Join Group request's notification 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requestor"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public string Get_JoinGroup_NotifyID(string userID, string requestor, string groupID)
        {
            try
            {
                var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.Target.ID == requestor && item.Destination.ID == groupID && item.Type == Notify.JOIN_GROUP_REQUEST && item.IsSeen == false).ToList();
                if (result.Count == 0) return null;
                return result.ElementAt(0).NotifyID;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get Join Project's notification
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requestor"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public string Get_JoinProject_NotifyID(string userID, string requestor, string projectID)
        {
            try
            {
                var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.Target.ID == requestor && item.Destination.ID == projectID && item.Type == Notify.JOIN_PROJECT_REQUEST && item.IsSeen == false).ToList();
                if (result.Count == 0) return null;
                return result.ElementAt(0).NotifyID;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get all notification of post that have unseen comment
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <returns></returns>
        public string Get_PostCmted_Unseen_NotifyID(string userID, string postID)
        {
            try
            {
                var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.Target.ID == postID && item.IsSeen == false);
                var notifyID = result.ElementAt(0).NotifyID;
                return notifyID;

            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Add actor to list commentor of a post
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="notifyID"></param>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool Add_Actor_To_PostCmted_Notify(string userID, string notifyID, SDLink actor)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Where(u => u.AccountInformation.UserID == userID && u.NotificationList.Any(no => no.NotifyID == notifyID));
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.NotificationList.ElementAt(-1).Actors, actor);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// If Actor already in actor list of PostCommted Notification
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="notifyID"></param>
        /// <param name="actorID"></param>
        /// <returns></returns>
        public bool Is_Actor_In_Notify(string userID, string notifyID, string actorID)
        {
            try
            {
                //var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(ntl => ntl.NotificationList).Where( nt => nt.NotifyID == notifyID).SelectMany(acl => acl.Actors).Where(ac => ac.ID == actorID).ToList();
                var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(ntl => ntl.NotificationList).Where(nt => nt.NotifyID == notifyID).ToList();
                List<SDLink> actors = new List<SDLink>();
                for (int i = 0; i < result.Count(); i++)
                {
                    if (result[i].Actors.Where(ac => ac.ID == actorID).Count() == 0)
                    {
                        return false;
                    }
                }
                return true;

            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// increase each user groupcount by 1
        /// </summary>
        /// <param name="listID"></param>
        /// <returns></returns>
        public bool Batch_Join_Group(IEnumerable<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(u => u.AccountInformation.UserID, listID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.GroupCount, 1);
                var result = collection.UpdateMany(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// decrease each user groupCount by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Batch_Out_Group(IEnumerable<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(u => u.AccountInformation.UserID, listID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.GroupCount, -1);
                var result = collection.UpdateMany(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// inscrease each user JoinedProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Batch_Join_Project(IEnumerable<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(u => u.AccountInformation.UserID, listID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.JoinedProjectCount, 1);
                var result = collection.UpdateMany(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// inscrease each user SponsoredProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Batch_Sponsor_Project(IEnumerable<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(u => u.AccountInformation.UserID, listID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.SponsoredProjectCount, 1);
                var result = collection.UpdateMany(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// inscrease each user OrganizedProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Batch_Organize_Project(IEnumerable<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(u => u.AccountInformation.UserID, listID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.OrganizedProjectCount, 1);
                var result = collection.UpdateMany(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// decrease each user JoinedProjectCOunt by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Batch_Out_Project(IEnumerable<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.In(u => u.AccountInformation.UserID, listID);
                var update = Builders<Mongo_User>.Update.Inc(u => u.AccountInformation.JoinedProjectCount, -1);
                var result = collection.UpdateMany(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public string Get_TaskAssign_NotifyID(string userID, string taskID)
        {
            try
            {
                //var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.Target.ID == taskID && item.Destination.ID == projectID && item.Type == Notify.TASK_ASSIGN && item.IsSeen == false).ToList();
                var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.Target.ID == taskID && item.Type == Notify.TASK_ASSIGN && item.IsSeen == false).ToList();
                if (result.Count == 0) return null;
                return result.ElementAt(0).NotifyID;
            }
            catch
            {
                throw;
            }
        }
        public string Get_Sponsor_NotifyID(string userID, string requestor, string projectID, int type)
        {
            try
            {
                var result = collection.AsQueryable().Where(u => u.AccountInformation.UserID == userID).SelectMany(nt => nt.NotificationList).Where(item => item.Target.ID == requestor && item.Destination.ID == projectID && item.Type == type && item.IsSeen == false).ToList();
                if (result.Count == 0) return null;
                return result.ElementAt(0).NotifyID;
            }
            catch
            {
                throw;
            }
        }
    }
}