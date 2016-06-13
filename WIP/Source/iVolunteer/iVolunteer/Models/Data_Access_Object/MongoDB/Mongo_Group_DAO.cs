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
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public static class Mongo_Group_DAO
    {
        static IMongoClient client = new MongoClient();
        static IMongoDatabase db = client.GetDatabase("iVolunteer");
        static IMongoCollection<Mongo_Group> collection = db.GetCollection<Mongo_Group>("Group");
        /// <summary>
        /// Add new group to mongoDB
        /// </summary>
        /// <param name="group"></param>
        /// <returns>true ì success</returns>
        public static bool Add_Group(Mongo_Group group)
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
        /// get a number of group infomation,
        /// </summary>
        /// <param name="number">number of information get</param>
        /// <param name="skip">number of group skip</param>
        /// <returns></returns>
        public static List<GroupInformation> Get_All_GroupInformation(int number, int skip)
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
        public static GroupInformation Get_Group_Information(string groupID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(g =>g.GroupInformation.GroupID == groupID);
                return result.GroupInformation;
            }
            catch
            {
                throw;
            }
        }

        public static bool Set_Activation_Status(string groupID, bool status)
        {
            try
            {
                var filter = Builders<Mongo_Group>.Filter.Eq(gr => gr.GroupInformation.GroupID, groupID);
                var update = Builders<Mongo_Group>.Update.Set(gr => gr.GroupInformation.IsActivate, status);
                collection.UpdateOne(filter, update);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}