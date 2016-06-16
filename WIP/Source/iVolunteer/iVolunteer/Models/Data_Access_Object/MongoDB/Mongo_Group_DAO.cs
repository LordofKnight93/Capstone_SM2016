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
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public class Mongo_Group_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Group> collection = db.GetCollection<Mongo_Group>("Group");
        /// <summary>
        /// Add new group to mongoDB
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true ì success</returns>
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
        /// delete a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_Group(string groupID)
        {
            try
            {
                collection.DeleteOne(gr => gr._id == new ObjectId(groupID));
                return true;
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
        /// search group by name, only activate group
        /// </summary>
        /// <param name="name">keyword</param>
        /// <param name="skip">number of result skip</param>
        /// <param name="number">bumber of result get</param>
        /// <returns></returns>
        public List<GroupInformation> Search_Group_By_Name(string name, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().Where(gr => gr.GroupInformation.GroupName.Contains(name)
                                                               && gr.GroupInformation.IsActivate == Constant.IS_ACTIVATE)
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
        public HistoryInformation Get_HistoryInfomation(string groupID)
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
        public List<ProjectSD> Get_CurrentProjects(string groupID)
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
                collection.UpdateOne(filter, update);
                return true;
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
        public bool Add_Project_To_CurrentProjects(string groupID, ProjectSD project)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<ProjectSD>(gr => gr.CurrentProjects,project);
                collection.UpdateOne(filter, update);
                return true;
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
        public bool Remove_Project_From_CurrentProjects(string groupID, string projectID)
        {
            try
            {
                var group_filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var project_filter = Builders<ProjectSD>.Filter.Eq(p => p.ProjectID,projectID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.CurrentProjects, project_filter);
                collection.UpdateOne(group_filter, update);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set avatar image link for a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="imgLink"></param>
        /// <returns></returns>
        public bool Set_AvtImgLink(string groupID, string imgLink)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.Set(gr => gr.GroupInformation.AvtImgLink, imgLink);
                collection.FindOneAndUpdate(filter, update);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Set cover image link for a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="imgLink"></param>
        /// <returns></returns>
        public bool Set_CoverImgLink(string groupID, string imgLink)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.Set(gr => gr.GroupInformation.CoverImgLink, imgLink);
                collection.FindOneAndUpdate(filter, update);
                return true;
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
        public bool Add_Leader(string groupID, UserSD user)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr._id, new ObjectId(groupID));
                var update = Builders<Mongo_Group>.Update.AddToSet<UserSD>(gr => gr.GroupStructure.Leaders, user);
                collection.FindOneAndUpdate(filter, update);
                return true;
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
                var user_filter = Builders<UserSD>.Filter.Eq(u => u.UserID, userID);
                var update = Builders<Mongo_Group>.Update.PullFilter(gr => gr.GroupStructure.Leaders, user_filter);
                collection.FindOneAndUpdate(group_filter, update);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}