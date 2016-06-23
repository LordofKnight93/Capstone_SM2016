using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.IO;
using MongoDB.Driver.Builders;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;

namespace iVolunteer.DAL.MongoDB
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
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.ID, projectID)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.Handler, Handler.PROJECT)
                           & Builders<Mongo_Post>.Filter.Eq(p=>p.PostInfomation.IsPublic, Status.IS_PUBLIC);
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
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.ID, projectID)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.Handler, Handler.PROJECT)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, Status.IS_PRIVATE);
                var result = collection.Find(filter).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set last activity date for post
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool Set_DateLastActivity(string postID, DateTime date)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var update = Builders<Mongo_Post>.Update.Set(p => p.PostInfomation.DateLastActivity, date);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add comment to post
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="cmt"></param>
        /// <returns></returns>
        public bool Add_Comment(string postID, Comment cmt)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var update = Builders<Mongo_Post>.Update.AddToSet<Comment>(p => p.CommentList, cmt);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a comment 
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="cmtID"></param>
        /// <returns></returns>
        public bool Delete_Comment(string postID, string cmtID)
        {
            try
            {
                var post_filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var cmt_filter = Builders<Comment>.Filter.Eq(cmt => cmt.CommentID, cmtID);
                var update = Builders<Mongo_Post>.Update.PullFilter(p => p.CommentList, cmt_filter);
                var result = collection.UpdateOne(post_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// like a post
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public bool Like(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var update = Builders<Mongo_Post>.Update.Inc(p => p.PostInfomation.LikeCount,1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// unlike a post
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public bool Unlike(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var update = Builders<Mongo_Post>.Update.Inc(p => p.PostInfomation.LikeCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add a liker
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add_LikerList(string postID, SDLink user)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var update = Builders<Mongo_Post>.Update.AddToSet<SDLink>(p => p.LikerList, user);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a liker
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_LikerList(string postID, string userID)
        {
            try
            {
                var post_filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var user_filter = Builders<SDLink>.Filter.Eq(u => u.ID, userID);
                var update = Builders<Mongo_Post>.Update.PullFilter(p => p.LikerList, user_filter);
                var result = collection.UpdateOne(post_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add a follower
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Add_FollowerID(string postID, string userID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var update = Builders<Mongo_Post>.Update.AddToSet<string>(p => p.FollowerIDs, userID);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a liker
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_FollowerID(string postID, string userID)
        {
            try
            {
                var post_filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var user_filter = Builders<string>.Filter.Eq(s => s, userID);
                var update = Builders<Mongo_Post>.Update.PullFilter(p => p.FollowerIDs, user_filter);
                var result = collection.UpdateOne(post_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get public post of a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_Public_Post_By_GroupID(string groupID, int skip, int number)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.ID, groupID)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.Handler, Handler.GROUP)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, Status.IS_PUBLIC);
                var result = collection.Find(filter).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get private post of a group
        /// </summary>
        /// <param name="albumID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_Private_Post_By_GroupID(string groupID, int skip, int number)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.ID, groupID)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.Handler, Handler.GROUP)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, Status.IS_PRIVATE);
                var result = collection.Find(filter).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get private post of a album
        /// </summary>
        /// <param name="albumID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_Post_By_AlbumID(string albumID, int skip, int number)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.AlbumLink.ID, albumID);
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