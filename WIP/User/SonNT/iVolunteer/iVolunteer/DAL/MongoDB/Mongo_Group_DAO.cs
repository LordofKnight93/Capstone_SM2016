﻿using System;
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
    public class Mongo_Group_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Group> collection = db.GetCollection<Mongo_Group>("Group");
        /// <summary>
        /// Add new group to mongoDB
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true if success</returns>
        public bool Add_Group(Mongo_Group group)
        {
            try
            {
                collection.InsertOne(group);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a group, admin user
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_Group(string groupID)
        {
            try
            {
                var result = collection.DeleteOne(gr => gr._id == new ObjectId(groupID));
                return result.IsAcknowledged; ;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a number of group infomation,
        /// </summary>
        /// <param name="number">number of information get</param>
        /// <param name="skip">number of group skip</param>
        /// <returns></returns>
        public List<GroupInformation> Get_All_GroupInformation(int skip,int number)
        {
            try
            {
                var result = collection.AsQueryable().Select(g => g.GroupInformation).Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get a group information
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public GroupInformation Get_GroupInformation(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(g => g._id == new ObjectId(groupID));
                return result.GroupInformation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// search group by name, only activate group, for user use
        /// </summary>
        /// <param name="name">keyword</param>
        /// <param name="skip">number of result skip</param>
        /// <param name="number">number of result get</param>
        /// <returns></returns>
        public List<GroupInformation> User_Search_Group_By_Name(string name, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().Where(gr => gr.GroupInformation.GroupName.Contains(name)
                                                               && gr.GroupInformation.IsActivate == Status.IS_ACTIVATE)
                                                     .Select(gr => gr.GroupInformation)
                                                     .Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// search group by name, for admin use
        /// </summary>
        /// <param name="name">keyword</param>
        /// <param name="skip">number of result skip</param>
        /// <param name="number">number of result get</param>
        /// <returns></returns>
        public List<GroupInformation> Admin_Search_Group_By_Name(string name, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().Where(gr => gr.GroupInformation.GroupName.Contains(name))
                                                     .Select(gr => gr.GroupInformation)
                                                     .Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a group structure
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public GroupStructure Get_GroupStructure(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(g => g._id == new ObjectId(groupID));
                return result.GroupStructure;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a group structure
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public ActivityInformation Get_HistoryInfomation(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(g => g._id == new ObjectId(groupID));
                return result.HistoryInformation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a group structure
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<SDLink> Get_CurrentProjects(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(g => g._id == new ObjectId(groupID));
                return result.CurrentProjects;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get all group join request
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<RequestItem> Get_RequestList(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(g => g._id == new ObjectId(groupID));
                return result.RequestList;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set activatiom status for a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool Set_Activation_Status(string groupID, bool status)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id,new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.Set(gr => gr.GroupInformation.IsActivate, status);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Add project to current project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool Add_CurrentProject(string groupID, SDLink project)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<SDLink>(gr => gr.CurrentProjects,project);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// after a project complete, remove tproject from current project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_CurrentProjects(string groupID, string projectID)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var project_filter = Builders<SDLink>.Filter.Eq(p => p.ID,projectID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.CurrentProjects, project_filter);
                var result = collection.UpdateOne(group_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set a member in group to group leader
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="user"></param>
        public bool Add_Leader(string groupID, Member user)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<Member>(gr => gr.GroupStructure.Leaders, user);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// dismiss a leader of a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_Leader(string groupID, string userID)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var user_filter = Builders<Member>.Filter.Eq(u => u.SDInfo.ID, userID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.GroupStructure.Leaders, user_filter);
                var result = collection.UpdateOne(group_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Add a user to group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add_JoinedUser(string groupID, SDLink user)
        {
            try
            {
                Member member = new Member(user);
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<Member>(gr => gr.GroupStructure.JoinedUsers, member);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// expell a user from a group's joineduser list
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_JoinedUser(string groupID, string userID)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var user_filter = Builders<Member>.Filter.Eq(m => m.SDInfo.ID, userID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.GroupStructure.JoinedUsers, user_filter);
                var result = collection.UpdateOne(group_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add joined project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="project"></param>
        public bool Add_JoinedProject(string groupID, SDLink project)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<SDLink>(gr => gr.HistoryInformation.JoinedProjects, project);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// remove joined project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_JoinedProject(string groupID, string projectID)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var project_filter = Builders<SDLink>.Filter.Eq(p => p.ID, projectID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.HistoryInformation.JoinedProjects, projectID);
                var result = collection.UpdateOne(group_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add organized project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="project"></param>
        public bool Add_OrganizedProject(string groupID, SDLink project)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<SDLink>(gr => gr.HistoryInformation.OrganizedProjects, project);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// remove organized project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_OrganizedProject(string groupID, string projectID)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var project_filter = Builders<SDLink>.Filter.Eq(p => p.ID, projectID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.HistoryInformation.OrganizedProjects, projectID);
                var result = collection.UpdateOne(group_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add sponsored project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="project"></param>
        public bool Add_SponsoredProject(string groupID, SDLink project)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<SDLink>(gr => gr.HistoryInformation.SponsoredProjects, project);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// remove sponsored project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_SponsoredProject(string groupID, string projectID)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var project_filter = Builders<SDLink>.Filter.Eq(p => p.ID, projectID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.HistoryInformation.SponsoredProjects, projectID);
                var result = collection.UpdateOne(group_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add join request to request list
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Add_Request(string groupID, RequestItem request)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<RequestItem>(gr => gr.RequestList, request);
                var result = collection.UpdateOne(group_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a request after accept or deny 
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool Delete_Request(string groupID, string requestID)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var request_filter = Builders<RequestItem>.Filter.Eq(rq => rq.RequestID, requestID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.RequestList, requestID);
                var result = collection.UpdateOne(group_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// update group information
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="groupInfo"></param>
        /// <returns></returns>
        public bool Update_GroupInfomarion(string groupID, GroupInformation groupInfo)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.Set(gr => gr.GroupInformation, groupInfo);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get groupSD
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public SDLink Get_SDLink(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(g => g._id == new ObjectId(groupID));
                return new SDLink(result.GroupInformation);
            }
            catch
            {
                throw;
            }
        }
    }
}