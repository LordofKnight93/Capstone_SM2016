using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.IO;
using MongoDB.Driver.Builders;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public class Mongo_Post_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Post> collection = db.GetCollection<Mongo_Post>("Post");
        /// <summary>
        /// Add new post to mongoDB
        /// </summary>
        /// <param name="post">Mongo_Post instance</param>
        /// <returns>true if success</returns>
        public bool Add_Post(Mongo_Post post)
        {
            try
            {
                collection.InsertOne(post);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public bool Delete_Post(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                collection.FindOneAndDelete(filter);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get a post by postID
        /// </summary>
        /// <param name="postID"></param>
        /// <returns>Mongo_Post instance</returns>
        public Mongo_Post Get_Post_By_ID(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var result = collection.Find(filter).FirstOrDefault();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get public post of a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="number"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_Public_Post_By_ProjectID(string projectID, int number, int skip)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.ProjectLink.ProjectID, projectID)
                          & Builders<Mongo_Post>.Filter.Eq(p=>p.PostInfomation.IsPublic, Constant.IS_PUBLIC);
                var result = collection.Find(filter).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get private post of a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="number"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_Private_Post_By_ProjectID(string projectID, int number, int skip)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.ProjectLink.ProjectID, projectID)
                          & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, Constant.IS_PRIVATE);
                var result = collection.Find(filter).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}