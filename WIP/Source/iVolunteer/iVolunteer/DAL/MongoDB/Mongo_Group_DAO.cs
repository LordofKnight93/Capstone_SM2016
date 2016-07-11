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
        /// get group infofmation of an group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public GroupInformation Get_GroupInformation(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(gr => gr.GroupInformation.GroupID == groupID);
                return result.GroupInformation;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get SDlink of a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public SDLink Get_SDLink(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(gr => gr.GroupInformation.GroupID == groupID);
                return new SDLink(result.GroupInformation);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get list group information from list ID
        /// </summary>
        /// <param name="listID"></param>
        /// <returns></returns>
        public List<GroupInformation> Get_GroupsInformation(List<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.In(gr => gr.GroupInformation.GroupID, listID);
                var result = collection.Find(filter).ToList().Select(gr => gr.GroupInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// udpate group information, only update description, email, phone
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="newInfo"></param>
        /// <returns></returns>
        public bool Update_GroupInformation(string groupID, GroupInformation newInfo)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr.GroupInformation.GroupID, groupID);
                var update = Builders<Mongo_Group>.Update.Set(gr => gr.GroupInformation.GroupDescription, newInfo.GroupDescription)
                                                         .Set(gr => gr.GroupInformation.Email, newInfo.Email)
                                                         .Set(gr => gr.GroupInformation.Phone, newInfo.Phone);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// increase member count by 1
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Member_Join(string groupID)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr.GroupInformation.GroupID, groupID);
                var update = Builders<Mongo_Group>.Update.Inc(gr => gr.GroupInformation.MemberCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decrease member count by 1
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Member_Out(string groupID)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr.GroupInformation.GroupID, groupID);
                var update = Builders<Mongo_Group>.Update.Inc(gr => gr.GroupInformation.MemberCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// search active and confirmed group
        /// </summary>
        /// <param name="name"></param>
        /// <param name="option">true to include deactive group, false to find active group only</param>
        /// <returns></returns>
        public List<GroupInformation> Group_Search(string name, bool allStatus)
        {
            try
            {
                var preResult = collection.AsQueryable().Where(gr => gr.GroupInformation.GroupName.ToLower().Contains(name.ToLower()));
                if (allStatus == false)
                {
                    preResult = preResult.Where(gr => gr.GroupInformation.IsActivate == Status.IS_ACTIVATE);
                }
                var result = preResult.Select(gr => gr.GroupInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        public List<SDLink> Get_Banned_Groups()
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr.GroupInformation.IsActivate, Status.IS_BANNED);
                var result = collection.Find(filter).ToList();
                List<SDLink> BannedGroups = new List<SDLink>();
                foreach (var item in result)
                {
                    BannedGroups.Add(Get_SDLink(item.GroupInformation.GroupID));
                }
                return BannedGroups;
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
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr.GroupInformation.GroupID, groupID);
                var update = Builders<Mongo_Group>.Update.Set(gr => gr.GroupInformation.IsActivate, status);
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