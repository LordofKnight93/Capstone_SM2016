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
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
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
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.PostID, postID);
                collection.DeleteOne(filter);
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
        public PostInformation Get_Post_By_ID(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.PostID, postID);
                var result = collection.Find(filter).FirstOrDefault();
                return result.PostInfomation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a numbers of post
        /// </summary>
        /// <param name="destinationID">ID of where post is</param>
        /// <param name="handler"> where post is</param>
        /// <param name="permission">is public or not</param>
        /// <param name="skip">number skip</param>
        /// <param name="number">number get</param>
        /// <returns></returns>
        public List<PostInformation> Get_PostInformations(string destinationID, string handler, bool permission, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().Where(p => p.PostInfomation.Destination.ID == destinationID
                                                              && p.PostInfomation.Destination.Handler == handler
                                                              && p.PostInfomation.IsPublic == permission
                                                              && p.PostInfomation.IsPinned == Status.IS_NOT_PINNED)
                                                     .Select(p => p.PostInfomation).Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get pinned post
        /// </summary>
        /// <param name="destinationID"></param>
        /// <param name="handler"></param>
        /// <param name="permission"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public PostInformation Get_PinnedPost(string destinationID, string handler, bool permission, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.PostInfomation.Destination.ID == destinationID
                                                              && p.PostInfomation.Destination.Handler == handler
                                                              && p.PostInfomation.IsPublic == permission
                                                              && p.PostInfomation.IsPinned == Status.IS_PINNED);
                return result.PostInfomation;
            }
            catch
            {
                throw;
            }
        }
        public Mongo_Post Get_Mg_PinnedPost(string destinationID, bool permission)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.PostInfomation.Destination.ID == destinationID
                                                              && p.PostInfomation.IsPublic == permission
                                                              && p.PostInfomation.IsPinned == Status.IS_PINNED);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public string Get_PinnedPost_ID(string destinationID, bool permission)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.PostInfomation.Destination.ID == destinationID
                                                              && p.PostInfomation.IsPublic == permission
                                                              && p.PostInfomation.IsPinned == Status.IS_PINNED);
                if(result == null)
                {
                    return null;
                }
                else return result.PostInfomation.PostID;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a numbers of comment or a post
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Comment> Get_Comments(string postID, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.PostInfomation.PostID == postID);
                return result.CommentList.OrderByDescending(cm => cm.DateCreate).Skip(skip).Take(number).ToList();
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
        /// add comment to post and inc cmt count by 1
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="cmt"></param>
        /// <returns></returns>
        public bool Add_Comment(string postID, Comment cmt)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var update = Builders<Mongo_Post>.Update.AddToSet<Comment>(p => p.CommentList, cmt)
                                                 .Inc(p => p.PostInfomation.CommentCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a comment and decrease cmt count by 1
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
                var update = Builders<Mongo_Post>.Update.PullFilter(p => p.CommentList, cmt_filter)
                                                        .Inc(p => p.PostInfomation.CommentCount, -1); ;
                var result = collection.UpdateOne(post_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add a liker and increase like count by 1
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add_LikerList(string postID, SDLink user)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p._id, new ObjectId(postID));
                var update = Builders<Mongo_Post>.Update.AddToSet<SDLink>(p => p.LikerList, user)
                                                        .Inc(p => p.PostInfomation.LikeCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a liker and decrease likecout by 1
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
                var update = Builders<Mongo_Post>.Update.PullFilter(p => p.LikerList, user_filter)
                                                        .Inc(p => p.PostInfomation.LikeCount, -1); ;
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
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPinned, Status.IS_NOT_PINNED)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, Status.IS_PUBLIC);
                var sort = Builders<Mongo_Post>.Sort.Descending(p => p.PostInfomation.DateCreate);
                var result = collection.Find(filter).Sort(sort).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get Post in Public section of Project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_Public_Post_By_ProjectID(string projectID, int skip, int number)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.ID, projectID)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.Handler, Handler.PROJECT)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPinned, Status.IS_NOT_PINNED)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, Status.IS_PUBLIC);
                var sort = Builders<Mongo_Post>.Sort.Descending(p => p.PostInfomation.DateCreate);
                var result = collection.Find(filter).Sort(sort).Skip(skip).Limit(number).ToList();
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
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPinned, Status.IS_NOT_PINNED)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, Status.IS_PRIVATE);
                var sort = Builders<Mongo_Post>.Sort.Descending(p => p.PostInfomation.DateLastActivity);
                var result = collection.Find(filter).Sort(sort).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get Post in Discussion Section of Project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="skip"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_Private_Post_By_ProjectID(string projectID, int skip, int number)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.ID, projectID)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.Destination.Handler, Handler.PROJECT)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPinned, Status.IS_NOT_PINNED)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, Status.IS_PRIVATE);
                var sort = Builders<Mongo_Post>.Sort.Descending(p => p.PostInfomation.DateLastActivity);
                var result = collection.Find(filter).Sort(sort).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        public bool Is_User_Liked(string userID, string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Where(p => p.PostInfomation.PostID == postID && p.LikerList.Any(u => u.ID == userID));
                var result = collection.Find(filter).ToList().Count;
                return result != 0;
            }
            catch
            {
                throw;
            }
        }
        public Mongo_Post Get_Mg_Post_By_ID(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.PostID, postID);
                var result = collection.Find(filter).FirstOrDefault();
                return result;
            }
            catch
            {
                throw;
            }
        }
        public bool Is_Pinned(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.PostID, postID )
                             & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPinned, true);
                var result = collection.Find(filter).FirstOrDefault();
                return result != null;
            }
            catch
            {
                throw;
            }
        }
        public bool Is_InPublic(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.PostID, postID)
                             & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, true);
                var result = collection.Find(filter).FirstOrDefault();
                return result != null;
            }
            catch
            {
                throw;
            }
        }
        public bool Set_IsPin(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.PostID, postID);
                var update = Builders<Mongo_Post>.Update.Set(p => p.PostInfomation.IsPinned, true);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public bool Set_IsNotPin(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.PostID, postID);
                var update = Builders<Mongo_Post>.Update.Set(p => p.PostInfomation.IsPinned, false);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public List<Mongo_Post> Get_NewFeed_Post(List<string> destinationIDs, int skip, int no)
        {
            try
            {
                List<Mongo_Post> posts = new List<Mongo_Post>();
                var filter = Builders<Mongo_Post>.Filter.In(p => p.PostInfomation.Destination.ID, destinationIDs);
                posts = collection.Find(filter).SortByDescending(p => p.PostInfomation.DateLastActivity).Skip(skip).Limit(no).ToList();
                return posts;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Only get post in Public of each destination
        /// </summary>
        /// <param name="destinationIDs"></param>
        /// <param name="skip"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_NewFeed_Post_Follower(List<string> destinationIDs, int skip, int no)
        {
            try
            {
                List<Mongo_Post> posts = new List<Mongo_Post>();
                var filter = Builders<Mongo_Post>.Filter.In(p => p.PostInfomation.Destination.ID, destinationIDs)
                           & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, true);
                posts = collection.Find(filter).SortByDescending(p => p.PostInfomation.DateLastActivity).Skip(skip).Limit(no).ToList();  
                return posts;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Create whole posts
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public List<Mongo_Post> Get_NewFeed_Post_All(List<string> destinationIDs, List<string> followDestinationIDs, int skip, int no)
        {
            try
            {
                //Follow destination's post
                List<Mongo_Post> posts = new List<Mongo_Post>();
                var filter = Builders<Mongo_Post>.Filter.In(p => p.PostInfomation.Destination.ID, followDestinationIDs)
                            & Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.IsPublic, true)
                            | Builders<Mongo_Post>.Filter.In(p => p.PostInfomation.Destination.ID, destinationIDs);

                var sort = Builders<Mongo_Post>.Sort.Descending(p => p.PostInfomation.DateCreate);
                //Normal 
                //var filter2 = Builders<Mongo_Post>.Filter.In(p => p.PostInfomation.Destination.ID, destinationIDs);
                //posts.AddRange(collection.Find(filter2).ToList());
                //posts.AddRange(collection.Find(filter1).ToList());
                posts = collection.Find(filter).Sort(sort).Skip(skip).Limit(no).ToList();
                //posts.OrderBy(p => p.PostInfomation.DateLastActivity).Skip(skip).Take(no);

                return posts;

            }
            catch
            {
                throw;
            }
        }
        public bool Is_Exist(string postID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.PostInfomation.PostID == postID);
                return result != null;
            }
            catch
            {
                throw;
            }
        }
        public int Get_Cmt_Count(string postID)
        {
            try
            {
                var filter = Builders<Mongo_Post>.Filter.Eq(p => p.PostInfomation.PostID, postID);
                var result = collection.Find(filter).FirstOrDefault();
                return result.PostInfomation.CommentCount;
            }
            catch
            {
                throw;
            }
        }
    }
}