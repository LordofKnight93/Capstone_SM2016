using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_Post_DAO
    {
        /// <summary>
        /// add new post to SQL DB
        /// </summary>
        /// <param name="post"></param>
        /// <returns>true if success</returns>
        public bool Add_Post(SQL_Post post)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Post.Add(post);
                    dbEntities.SaveChanges();
                    return true;
                }
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
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Post post = dbEntities.SQL_Post.FirstOrDefault(p => p.PostID == postID);
                    dbEntities.SQL_Post.Remove(post);
                    dbEntities.SaveChanges();
                    return true;
                }
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
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Post post = dbEntities.SQL_Post.FirstOrDefault(p => p.PostID == postID);
                    post.Permission = permission;
                    dbEntities.SaveChanges();
                    return true;
                }
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
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Post post = dbEntities.SQL_Post.FirstOrDefault(p => p.PostID == postID);
                    post.DateLastActivity = date;
                    dbEntities.SaveChanges();
                    return true;
                }
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
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Post.Where(p => p.AlbumID == albumID);
                    dbEntities.SQL_Post.RemoveRange(result);
                    dbEntities.SaveChanges();
                    return true;
                }
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
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    //get the post
                    SQL_Post post = dbEntities.SQL_Post.FirstOrDefault(p => p.PostID == postID);
                    //check post is public or not
                    if (post.Permission == Status.IS_PUBLIC) return true;
                    //if not check user has direct relation with [roject or group post belong to
                    if (post.ProjectID != null)
                    {
                        SQL_User_Project project_relation = dbEntities.SQL_User_Project.FirstOrDefault(pr => pr.ProjectID == post.ProjectID && pr.UserID == userID
                                                                                                                && pr.RelationType != Relation.FOLLOW_RELATION);
                        if (project_relation != null) return true;
                    }
                    else
                    {
                        SQL_User_Group group_relation = dbEntities.SQL_User_Group.FirstOrDefault(gr => gr.GroupID == post.GroupID && gr.UserID == userID
                                                                                                                && gr.RelationType != Relation.FOLLOW_RELATION);
                        if (group_relation != null) return true;
                    }

                    // case else is not have permission
                    return false;
                }
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
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    List<string> posts = new List<string>();
                    // get user project relation and proejct is activate
                    var related_project = from rl in dbEntities.SQL_User_Project
                                          join p in dbEntities.SQL_Project on rl.ProjectID equals p.ProjectID
                                          where p.IsActivate == Status.IS_ACTIVATE
                                          select rl;

                    //get user group relation and group is activate
                    var related_group = from rl in dbEntities.SQL_User_Group
                                        join g in dbEntities.SQL_Group on rl.GroupID equals g.GroupID
                                        where g.IsActivate == Status.IS_ACTIVATE
                                        select rl;

                    //get posts accessable
                    var accessable_posts = from p in dbEntities.SQL_Post
                                           join pr in related_project on p.ProjectID equals pr.ProjectID
                                           join gr in related_group on p.GroupID equals gr.GroupID
                                           where p.Permission == Status.IS_PUBLIC
                                                 || p.Permission == Status.IS_PRIVATE && pr.RelationType != Relation.FOLLOW_RELATION
                                                 || p.Permission == Status.IS_PRIVATE && gr.RelationType != Relation.FOLLOW_RELATION
                                           orderby p.DateCreate descending
                                           select p.PostID;
                    posts = accessable_posts.Skip(skip).Take(number).ToList();
                    return posts;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get post for user newfeed sort by Get_Post_For_Newfeed_By_DateLastActivity
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<string> Get_Post_For_Newfeed_By_DateLastActivity(string userID, int number, int skip)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    List<string> posts = new List<string>();
                    // get user project relation and proejct is activate
                    var related_project = from rl in dbEntities.SQL_User_Project
                                          join p in dbEntities.SQL_Project on rl.ProjectID equals p.ProjectID
                                          where p.IsActivate == Status.IS_ACTIVATE
                                          select rl;

                    //get user group relation and group is activate
                    var related_group = from rl in dbEntities.SQL_User_Group
                                        join g in dbEntities.SQL_Group on rl.GroupID equals g.GroupID
                                        where g.IsActivate == Status.IS_ACTIVATE
                                        select rl;

                    //get posts accessable
                    var accessable_posts = from p in dbEntities.SQL_Post
                                           join pr in related_project on p.ProjectID equals pr.ProjectID
                                           join gr in related_group on p.GroupID equals gr.GroupID
                                           where p.Permission == Status.IS_PUBLIC
                                                 || p.Permission == Status.IS_PRIVATE && pr.RelationType != Relation.FOLLOW_RELATION
                                                 || p.Permission == Status.IS_PRIVATE && gr.RelationType != Relation.FOLLOW_RELATION
                                           orderby p.DateLastActivity descending
                                           select p.PostID;
                    posts = accessable_posts.Skip(skip).Take(number).ToList();
                    return posts;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}