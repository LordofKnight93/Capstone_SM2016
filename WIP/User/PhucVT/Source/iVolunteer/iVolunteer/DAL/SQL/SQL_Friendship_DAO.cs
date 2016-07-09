using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_Friendship_DAO
    {
        /// <summary>
        /// add a relation between 2 users
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public bool Add_Relation(SQL_Friendship relation)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Friendship.Add(relation);
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
        /// set status of a relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool Update_Relation(string userID, string friendID, bool status)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Friendship.FirstOrDefault(rl => rl.UserID == userID
                                                                          && rl.FriendID == friendID);
                    result.Status = status;
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
        /// delete a relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        /// <returns></returns>
        public bool Delete_Specific_Relation(string userID, string friendID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Friendship.FirstOrDefault(rl => rl.UserID == userID
                                                                            && rl.FriendID == friendID);
                    if (result != null)
                    {
                        dbEntities.SQL_Friendship.Remove(result);
                        dbEntities.SaveChanges();
                    }
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if 2 users are friend
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        /// <returns></returns>
        public bool Is_Friend(string userID, string friendID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Friendship.FirstOrDefault(rl => rl.UserID == userID
                                                                            && rl.FriendID == friendID
                                                                            && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a friend request is send
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        /// <returns></returns>
        public bool Is_Requested(string userID, string friendID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Friendship.FirstOrDefault(rl => rl.UserID == userID
                                                                            && rl.FriendID == friendID 
                                                                            && rl.Status == Status.PENDING);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a user is being requested by otheruser
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        /// <returns></returns>
        public bool Is_Be_Requested(string userID, string friendID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Friendship.FirstOrDefault(rl => rl.UserID == friendID
                                                                            && rl.FriendID == userID
                                                                            && rl.Status == Status.PENDING);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}