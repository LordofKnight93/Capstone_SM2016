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
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                return result.UserInformation;
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
                var result = collection.AsQueryable().FirstOrDefault(us => us.AccountInformation.UserID == userID);
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
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                return result.JoinedGroups;
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
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
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
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
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
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
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
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
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
        /// set confirmation statuc for an account in mongDB
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <returns>true if success</returns>
        public bool Set_Comfirmation_Status(string userID, bool status)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.Set(acc => acc.AccountInformation.IsConfirmed, status);
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
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
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
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
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
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
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
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID);
                return new SDLink(result.AccountInformation);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add joined group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool Add_JoinedGroup(string userID, SDLink group)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.JoinedGroups, group);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete joined group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_JoinedGroup(string userID, string groupID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var group_filter = Builders<SDLink>.Filter.Eq(g => g.ID, groupID);
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.JoinedGroups, group_filter);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add new project to current projects
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool Add_CurrentProject(string userID, SDLink project)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.CurrentProjects, project);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete project from current projects
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_CurrentProject(string userID, string projectID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var project_filter = Builders<SDLink>.Filter.Eq(p => p.ID, projectID );
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.CurrentProjects, project_filter);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add proejct to joined projects
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool Add_JoinedProject(string userID, SDLink project)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.ActivityHistory.JoinedProjects, project);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete project from joined projects
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_JoinedProject(string userID, string projectID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var project_filter = Builders<SDLink>.Filter.Eq(p => p.ID, projectID);
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.ActivityHistory.JoinedProjects, project_filter);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add proejct to organize projects
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool Add_OrganizedProject(string userID, SDLink project)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.ActivityHistory.OrganizedProjects, project);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete project from organize projects
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_OrganizedProject(string userID, string projectID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var project_filter = Builders<SDLink>.Filter.Eq(p => p.ID, projectID);
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.ActivityHistory.OrganizedProjects, project_filter);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add proejct to sponsored projects
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool Add_SponsoredProject(string userID, SDLink project)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.ActivityHistory.SponsoredProjects, project);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete project from sponsored projects
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_SponsoredProject(string userID, string projectID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var project_filter = Builders<SDLink>.Filter.Eq(p => p.ID, projectID);
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.ActivityHistory.SponsoredProjects , project_filter);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add friend and increase friendcoutn by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friend"></param>
        /// <returns></returns>
        public bool Add_Friend(string userID, SDLink user)
        {
            try
            {
                Member friend = new Member(user);
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.FriendList, friend)
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
        /// delete a friend and decrease fienr count by 1
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        /// <returns></returns>
        public bool Delete_Friend(string userID, string friendID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var friend_filter = Builders<Member>.Filter.Eq(f => f.SDInfo.ID, friendID);
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.FriendList, friend_filter)
                                                        .Inc(u => u.AccountInformation.FriendCount, 1); ;
                var result = collection.UpdateOne(user_filter, update);
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
        /// add a request to usser request list
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Add_Request(string userID, RequestItem request)
        {
            try
            {
                var filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var update = Builders<Mongo_User>.Update.AddToSet(u => u.RequestList, request);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Delete_Request(string userID, string requestID)
        {
            try
            {
                var user_filter = Builders<Mongo_User>.Filter.Eq(acc => acc.AccountInformation.UserID, userID);
                var request_filter = Builders<RequestItem>.Filter.Eq(rq => rq.RequestID, requestID);
                var update = Builders<Mongo_User>.Update.PullFilter(u => u.RequestList, request_filter);
                var result = collection.UpdateOne(user_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a number of user request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<RequestItem> Get_RequestList(string userID, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID)
                                                     .RequestList.Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a specific request of user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public RequestItem Get_Request(string userID, string requestID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(u => u.AccountInformation.UserID == userID)
                                                     .RequestList.FirstOrDefault(rq => rq.RequestID == requestID);
                return result;
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
        /// function search user for user
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<AccountInformation> Search_Account(string displayName, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable()
                                       .Where(u => u.AccountInformation.DisplayName.Contains(displayName))
                                       .Select(u => u.AccountInformation).Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// search user with status and displayname
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="status"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<AccountInformation> Search_Account(string displayName,bool status, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable()
                                       .Where(u => u.AccountInformation.DisplayName.Contains(displayName) && u.AccountInformation.IsActivate == status)
                                       .Select(u => u.AccountInformation).Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}