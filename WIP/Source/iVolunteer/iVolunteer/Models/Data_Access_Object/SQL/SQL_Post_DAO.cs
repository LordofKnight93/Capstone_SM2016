using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public class SQL_Post_DAO
    {
        iVolunteerEntities dbEntities = new iVolunteerEntities();
        /// <summary>
        /// add new post to SQL DB
        /// </summary>
        /// <param name="post"></param>
        /// <returns>true if success</returns>
        public bool Add_Post(SQL_Post post)
        {
            try
            {
                dbEntities.SQL_Post.Add(post);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a post
        /// </summary>
        /// <param name="postID"></param>
        /// <returns>true if success</returns>
        public bool Delete_Post(string postID)
        {
            try
            {
                SQL_Post post = dbEntities.SQL_Post.FirstOrDefault(p => p.PostID == postID);
                dbEntities.SQL_Post.Remove(post);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set permission to post, often use on post type image
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool Set_Permission(string postID, bool permission)
        {
            try
            {
                SQL_Post post = dbEntities.SQL_Post.FirstOrDefault(p => p.PostID == postID);
                post.Permission = permission;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set last activity date to post
        /// </summary>
        /// <param name="postID"></param>
        /// <param name="date"></param>
        /// <returns>true if success</returns>
        public bool Set_DateLastActivity(string postID, DateTime date)
        {
            try
            {
                SQL_Post post = dbEntities.SQL_Post.FirstOrDefault(p => p.PostID == postID);
                post.DateLastActivity = date;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete all post belong to a album
        /// </summary>
        /// <param name="albumID"></param>
        /// <returns>true if success</returns>
        public bool Delete_Post_By_AlbumID(string albumID)
        {
            try
            {
                var result = dbEntities.SQL_Post.Where(p => p.AlbumID == albumID);
                dbEntities.SQL_Post.RemoveRange(result);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a user can access to 1 post
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="postID"></param>
        /// <returns></returns>
        public bool IsAccessable(string userID, string postID)
        {
            try
            {
                //get the post
                SQL_Post post = dbEntities.SQL_Post.FirstOrDefault(p => p.PostID == postID);
                //check post is public or not
                if(post.Permission == Constant.IS_PUBLIC) return true;
                //if not check user has direct relation with [roject or group post belong to
                if (post.ProjectID != null)
                {
                    SQL_User_Project project_relation = dbEntities.SQL_User_Project.FirstOrDefault(pr => pr.ProjectID == post.ProjectID && pr.UserID == userID
                                                                                                            && pr.RelationType == Constant.DIRECT_RELATION);
                    if (project_relation != null) return true;
                }
                else
                {
                    SQL_User_Group group_relation = dbEntities.SQL_User_Group.FirstOrDefault(gr => gr.GroupID == post.GroupID && gr.UserID == userID
                                                                                                            && gr.RelationType == Constant.DIRECT_RELATION);
                    if (group_relation != null) return true;
                }

                // case else is not have permission
                return false;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get post for user newfeed sort by DateCreate
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<string> Get_Post_For_Newfeed_By_DateCreate(string userID, int number, int skip)
        {
            List<string> posts = new List<string>();
            // get user project relation and proejct is activate
            var related_project = from rl in dbEntities.SQL_User_Project
                                  join p in dbEntities.SQL_Project on rl.ProjectID equals p.ProjectID
                                  where p.IsActivate == Constant.IS_ACTIVATE
                                  select rl;

            //get user group relation and group is activate
            var related_group = from rl in dbEntities.SQL_User_Group
                                join g in dbEntities.SQL_Group on rl.GroupID equals g.GroupID
                                where g.IsActivate == Constant.IS_ACTIVATE
                                select rl;

            //get posts accessable
            var accessable_posts = from p in dbEntities.SQL_Post
                                  join pr in related_project on p.ProjectID equals pr.ProjectID
                                  join gr in related_group on p.GroupID equals gr.GroupID
                                  where p.Permission == Constant.IS_PUBLIC 
                                        || p.Permission == Constant.IS_PRIVATE && pr.RelationType == Constant.DIRECT_RELATION
                                        || p.Permission == Constant.IS_PRIVATE && gr.RelationType == Constant.DIRECT_RELATION
                                  orderby p.DateCreate descending
                                  select p.PostID;
            posts = accessable_posts.Skip(skip).Take(number).ToList() ;
            return posts;
        }
    }
}